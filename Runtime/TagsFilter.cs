using System;
using System.Collections.Generic;
using System.Linq;
using LowEndGames.Utils;
using Sirenix.OdinInspector;

namespace LowEndGames.ObjectTagSystem
{
    [Serializable]
    public class TagsFilter
    {
        public string Title => $"{(Invert ? " NOT" : "")} {string.Join($" {(Comparison is TagComparison.All ? "AND" : "OR")} ", Tags.Select(t => t.name.LastAfterDot()))}";

        [InfoBox("$Title", InfoMessageType.None)]
        public List<ObjectTag> Tags = new();
        [ShowIf("@this.Tags.Count > 1")]
        public TagComparison Comparison;
        public bool Invert;
        
        public enum TagComparison
        {
            Any,
            All
        }

        public bool Check(ITagOwner taggedObject)
        {
            if (Tags.Count == 0)
            {
                return !Invert;
            }

            var result = Comparison switch
            {
                TagComparison.Any => Tags.Any(taggedObject.HasTag),
                TagComparison.All => Tags.All(taggedObject.HasTag),
                _ => throw new ArgumentOutOfRangeException()
            };

            return Invert ? !result : result;
        }
    }
}