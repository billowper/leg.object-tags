using System.Collections.Generic;
using System.Linq;
using LowEndGames.Utils;
using Sirenix.OdinInspector;
using UnityEngine;
using UnityEngine.Serialization;

namespace LowEndGames.ObjectTagSystem
{
    [CreateAssetMenu(menuName = "Low End Games/Object Tags/Rule")]
    public class ObjectTagsInteractionRule : ScriptableObject
    {
        public enum EvaluationMethods
        {
            ObjectInteraction,
            Self
        }

        [FormerlySerializedAs("RuleType")]
        [Tooltip("ObjectInteraction: these rules are evaluated for every interaction between two objects\n" +
                 "Self: these rules are evaluated constantly by every tagged object")]
        public EvaluationMethods EvaluationMethod;
        
        [HideIf(nameof(EvaluationMethod), EvaluationMethods.ObjectInteraction)]
        public float RequiredTime;

        [InfoBox("$Title", InfoMessageType.None)]
        public List<TagsFilter> Filters;
        public List<TagAction> Actions; 
        
        public bool Evaluate(TaggedObject target)
        {
            return Filters.EvaluateFilters(target);
        }

        public string Title => GetDescriptionString($"{RequiredTime}").Size(14);
        
        public string GetDescriptionString(string timeString)
        {
            return
                $"{(Filters.Count > 0 ? $"If{string.Join(" AND ", Filters.Select(t => t.Title))}{(EvaluationMethod is EvaluationMethods.Self ? $" for {timeString} seconds" : "")}\n" : "")}" +
                $"{string.Join(", ", Actions.Select(t => $"{t.Action} {t.Tag.name.LastAfterDot()}"))} : {(EvaluationMethod is EvaluationMethods.ObjectInteraction ? "TARGET" : "SELF")}";
        }
        
        public static ObjectTagsInteractionRule[] All { get; private set; }
        public static IEnumerable<ObjectTagsInteractionRule> Global => All.Where(r => r.EvaluationMethod is EvaluationMethods.ObjectInteraction);
        public static IEnumerable<ObjectTagsInteractionRule> Self => All.Where(r => r.EvaluationMethod is EvaluationMethods.Self);

        [RuntimeInitializeOnLoadMethod(RuntimeInitializeLoadType.BeforeSceneLoad)]
        private static void Init()
        {
            All = Resources.LoadAll<ObjectTagsInteractionRule>("").ToArray();
            
            Debug.Log($"Loaded {All.Length} Rules");
        }
    }
}