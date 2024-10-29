using System;
using System.Collections.Generic;

namespace LowEndGames.ObjectTagSystem
{
    [Serializable]
    public class TagOwnerConfiguration
    {
        public bool BlockTagChanges;
        public List<ObjectTag> DefaultTags = new List<ObjectTag>();
        public List<InteractionRuleTimeOverride> RuleTimeOverrides = new();
    }
}