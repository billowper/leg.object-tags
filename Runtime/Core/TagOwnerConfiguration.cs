using System;
using System.Collections.Generic;
using LowEndGames.ObjectTagSystem.Attributes;
using UnityEngine;

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
    }
}