using System;
using System.Collections.Generic;
using System.Linq;
using Sirenix.OdinInspector;
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
        
        [ShowInInspector, ReadOnly]
        public ObjectTags EnumValue { get; private set; }

        private void OnValidate()
        {
            EnumValue = Enum.Parse<ObjectTags>(name.Split('.').Last());
        }

        public void DrawGizmos(Transform transform)
        {
            foreach (var behaviour in Behaviours)
            {
                behaviour.DrawGizmos(transform);
            }
        }
    }
}