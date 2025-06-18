using System;
using System.Collections.Generic;
using System.Linq;
using LowEndGames.ObjectTagSystem.Attributes;
using UnityEngine;
using Object = UnityEngine.Object;

namespace LowEndGames.ObjectTagSystem
{
    /// <summary>
    /// initial configuration for a <see cref="TagOwner"/>
    /// </summary>
    [Serializable]
    public class TagOwnerConfiguration 
    {
        [Tooltip("if true, this objects tags cannot change")]
        public bool BlockTagChanges;
       
        [HideLabel] 
        [Tooltip("tags added on init")]
        public List<ObjectTag> DefaultTags = new List<ObjectTag>();
        
        [Tooltip("override time required for a Rule to pass")]
        public List<InteractionRuleTimeOverride> RuleTimeOverrides = new();

        public void OnValidate(Object owner)
        {
            if (DefaultTags.Any(t => string.IsNullOrEmpty(t.ToString())))
            {
                Debug.LogError($"{owner.name} has bad Tag OwnerConfig!", owner);
            }
        }
    }
}