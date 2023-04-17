using System.Collections.Generic;
using System.Linq;
using LowEndGames.Utils;
using Sirenix.OdinInspector;
using UnityEngine;

namespace LowEndGames.ObjectTagSystem
{
    [CreateAssetMenu(menuName = "Low End Games/Object Tags/Rule")]
    public class ObjectTagsInteractionRule : ScriptableObject
    {
        public enum RuleTypes
        {
            Global,
            Self
        }

        public RuleTypes RuleType;
        
        [InfoBox("$Title", InfoMessageType.None)]
        public List<TagsFilter> Filters;
        public List<TagAction> Actions; 
        [HideIf(nameof(RuleType), RuleTypes.Global)]
        public float RequiredTime;

        public bool Evaluate(TaggedObject target)
        {
            return Filters.EvaluateFilters(target);
        }

        public string Title => GetDescriptionString($"{RequiredTime}").Size(14);
        
        public string GetDescriptionString(string timeString)
        {
            return
                $"{(Filters.Count > 0 ? $"If{string.Join(" AND ", Filters.Select(t => t.Title))}{(RuleType is RuleTypes.Self ? $" for {timeString} seconds" : "")}\n" : "")}" +
                $"{string.Join(", ", Actions.Select(t => $"{t.Action} {t.Tag.name.LastAfterDot()}"))} : {(RuleType is RuleTypes.Global ? "TARGET" : "SELF")}";
        }
        
        public static ObjectTagsInteractionRule[] All { get; private set; }
        public static IEnumerable<ObjectTagsInteractionRule> Global => All.Where(r => r.RuleType is RuleTypes.Global);
        public static IEnumerable<ObjectTagsInteractionRule> Self => All.Where(r => r.RuleType is RuleTypes.Self);

        [RuntimeInitializeOnLoadMethod(RuntimeInitializeLoadType.BeforeSceneLoad)]
        private static void Init()
        {
            All = Resources.LoadAll<ObjectTagsInteractionRule>("").ToArray();
            
            Debug.Log($"Loaded {All.Length} Rules");
        }
    }
}