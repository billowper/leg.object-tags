using System.Collections.Generic;
using System.Linq;
using UnityEngine;

namespace LowEndGames.ObjectTagSystem
{
    /// <summary>
    /// evaluates a set of <see cref="TagsFilter"/>s and executes <see cref="TagAction"/> if all pass
    /// </summary>
    [CreateAssetMenu(menuName = "ObjectTags System/Interaction Rule")]
    public class ObjectTagsInteractionRule : ScriptableObject
    {
        public enum EvaluationMethods
        {
            OnObjectInteraction,
            Constant
        }

        [Tooltip("OnObjectInteraction: these rules are evaluated for every interaction between two objects\n" +
                 "Constant: these rules are evaluated constantly by every tagged object")]
        public EvaluationMethods EvaluationMethod;
        
        [Tooltip("how long in seconds that this Rule's conditions must be met before the Interaction is performed (only applicable for EvaluationMode.Constant)")]
        public float RequiredTime;
        
        [Tooltip("these filters must pass for this rule's actions to be performed")]
        public List<TagsFilter> Filters = new();

        [Tooltip("these actions are executed on the tagged object when active")]
        public List<TagAction> Actions = new(); 
        
        public bool Evaluate(TaggedObject target)
        {
            return Filters.EvaluateFilters(target);
        }

        public string GetDescriptionString(string timeString)
        {
            if (Filters.Count > 0)
            {
                var conditionStr = $"If{string.Join(" AND ", Filters.Select(t => t.Title))}{(EvaluationMethod is EvaluationMethods.Constant ? $" for {timeString} seconds" : "")}";
                var actionsStr = "No Actions";

                if (Actions.Count(t => t.Tag) > 0)
                {
                    actionsStr = "";

                    var addActions = Actions.Where(a => a.Action is TagAction.TagActions.Add).ToArray();
                    var removeActions = Actions.Where(a => a.Action is TagAction.TagActions.Remove).ToArray();

                    if (addActions.Length > 0)
                    {
                        actionsStr = $"{string.Join(", ", addActions.Where(t => t.Tag != null).Select(t => $"{t.Action} {t.Tag.name.Split(".").Last()}"))}";
                        actionsStr += $" to {(EvaluationMethod is EvaluationMethods.OnObjectInteraction ? "TARGET" : "SELF")}";

                        if (removeActions.Length > 0)
                            actionsStr += "\n";
                    }

                    if (removeActions.Length > 0)
                    {
                        actionsStr += $"{string.Join(", ", removeActions.Where(t => t.Tag != null).Select(t => $"{t.Action} {t.Tag.name.Split(".").Last()}"))}";
                        actionsStr += $" from {(EvaluationMethod is EvaluationMethods.OnObjectInteraction ? "TARGET" : "SELF")}";
                    }
                }

                return $"{conditionStr}\n{actionsStr}";
            }

            return "Rule has no filters.";
        }

        public static ObjectTagsInteractionRule[] All { get; private set; }
        public static IEnumerable<ObjectTagsInteractionRule> Global => All.Where(r => r.EvaluationMethod is EvaluationMethods.OnObjectInteraction);
        public static IEnumerable<ObjectTagsInteractionRule> Self => All.Where(r => r.EvaluationMethod is EvaluationMethods.Constant);

        [RuntimeInitializeOnLoadMethod(RuntimeInitializeLoadType.BeforeSceneLoad)]
        private static void Init()
        {
            All = Resources.LoadAll<ObjectTagsInteractionRule>("").ToArray();
            
            Debug.Log($"Loaded {All.Length} Rules");
        }
    }
}