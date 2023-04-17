using System;
using JetBrains.Annotations;
using Sirenix.OdinInspector;

namespace LowEndGames.ObjectTagSystem
{
    [Serializable]
    public class InteractionRuleTimeOverride
    {
        [InfoBox("$InfoBox")]
        public ObjectTagsInteractionRule Rule;
        public float OverrideRequiredTime;

        [UsedImplicitly] private string InfoBox => Rule != null ? Rule.GetDescriptionString($"{OverrideRequiredTime}") : "";
    }
}