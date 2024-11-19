using System.Collections.Generic;
using System.Linq;
using LowEndGames.ObjectTagSystem.Attributes;
using UnityEngine;

namespace LowEndGames.ObjectTagSystem
{
    /// <summary>
    /// evaluates a set of <see cref="TagsFilter"/>s and executes <see cref="TagAction"/> if all pass
    /// </summary>
    [CreateAssetMenu(menuName = "ObjectTags System/Tag Change Rule")]
    public class TagChangeRule : ScriptableObject
    {
        [Tooltip("how long in seconds that this Rule's conditions must be met before the Interaction is performed (only applicable for EvaluationMode.Constant)")]
        public float RequiredTime;
        
        [Tooltip("these filters must pass for this rule's actions to be performed")]
        public List<TagsFilter> Filters = new();

        [Tooltip("these actions are executed on the tagged object when active")]
        public List<TagAction> Actions = new();

        [Tooltip("reset this rule's timer when this tag is added")]
        [HideLabel]
        public List<ObjectTag> TimerResetWhenAdded = new List<ObjectTag>();
        
        [Tooltip("reset this rule's timer when this tag is removed")]
        [HideLabel]
        public List<ObjectTag> TimerResetWhenRemoved = new List<ObjectTag>();
        
        public bool Evaluate(TaggedObject target)
        {
            return Filters.EvaluateFilters(target);
        }

        public string GetDescriptionString(string timeString)
        {
            if (Filters.Count > 0)
            {
                var conditionStr = $"If{string.Join(" AND ", Filters.Select(t => t.Title))}{($" for {timeString} seconds")}";
                var actionsStr = "No Actions";

                if (Actions.Count(t => t.Tag) > 0)
                {
                    actionsStr = "";

                    var addActions = Actions.Where(a => a.Action is TagAction.TagActions.Add).ToArray();
                    var removeActions = Actions.Where(a => a.Action is TagAction.TagActions.Remove).ToArray();

                    if (addActions.Length > 0)
                    {
                        actionsStr = $"{string.Join(", ", addActions.Where(t => t.Tag != null).Select(t => $"{t.Action} {t.Tag.name.Split(".").Last()}"))}";

                        if (removeActions.Length > 0)
                            actionsStr += "\n";
                    }

                    if (removeActions.Length > 0)
                    {
                        actionsStr += $"{string.Join(", ", removeActions.Where(t => t.Tag != null).Select(t => $"{t.Action} {t.Tag.name.Split(".").Last()}"))}";
                    }
                }

                return $"{conditionStr}\n{actionsStr}";
            }

            return "Rule has no filters.";
        }

        public static TagChangeRule[] All { get; private set; }

        [RuntimeInitializeOnLoadMethod(RuntimeInitializeLoadType.BeforeSceneLoad)]
        private static void Init()
        {
            All = Resources.LoadAll<TagChangeRule>("").ToArray();
            
            Debug.Log($"Loaded {All.Length} Rules");
        }
    }
}