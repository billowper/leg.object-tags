using System.Collections.Generic;
using UnityEngine;

namespace LowEndGames.ObjectTagSystem
{
    [CreateAssetMenu(menuName = "Low End Games/Object Tags/Tag")]
    public class ObjectTag : ScriptableObject
    {
        [Tooltip("objects must pass this filter for this tag to be added")]
        public List<TagsFilter> Filters;
        
        [Tooltip("tag actions performed on the owner when this tag is added")]
        public List<TagAction> ActionsOnAdded; 
        
        [Tooltip("behaviours which are active when this tag is on an object")]
        public List<BaseTagBehaviourSettings> Behaviours;

        public void DrawGizmos(Transform transform)
        {
            foreach (var behaviour in Behaviours)
            {
                behaviour.DrawGizmos(transform);
            }
        }
    }
}