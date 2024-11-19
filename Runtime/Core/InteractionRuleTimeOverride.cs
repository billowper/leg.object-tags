using System;
using JetBrains.Annotations;

namespace LowEndGames.ObjectTagSystem
{
    [Serializable]
    public class InteractionRuleTimeOverride
    {
        public TagChangeRule Rule;
        public float OverrideRequiredTime;

        [UsedImplicitly] private string InfoBox => Rule != null ? Rule.GetDescriptionString($"{OverrideRequiredTime}") : "";
    }
}