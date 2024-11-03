using System;
using System.Collections.Generic;
using LowEndGames.ObjectTagSystem.Attributes;
using UnityEngine;

namespace LowEndGames.ObjectTagSystem
{
    [Serializable]
    public class TagOwnerConfiguration
    {
        [Tooltip("if true, this objects tags cannot change")]
        public bool BlockTagChanges;
        [HideLabel] 
        public List<ObjectTag> DefaultTags = new List<ObjectTag>();
        public List<InteractionRuleTimeOverride> RuleTimeOverrides = new();
    }
}