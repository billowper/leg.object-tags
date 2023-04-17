using System;
using System.Collections.Generic;
using UnityEngine;

namespace LowEndGames.ObjectTagSystem
{
    [Serializable]
    public class TagOwnerConfiguration
    {
        public bool BlockTagChanges;
        public List<ObjectTag> DefaultTags = new List<ObjectTag>();
        public List<InteractionRuleTimeOverride> RuleTimeOverrides = new();

        public void DrawGizmos(Transform transform)
        {
            foreach (var tag in DefaultTags)
            {
                tag.DrawGizmos(transform);
            }
        }
    }
}