using System;
using System.Collections.Generic;
using System.Linq;
using LowEndGames.ObjectTagSystem.Attributes;

namespace LowEndGames.ObjectTagSystem
{
    /// <summary>
    /// evaluates whether a <see cref="ITagOwner"/> has/does not have, any/all of the provided <see cref="ObjectTag"/>s
    /// </summary>
    [Serializable]
    public class TagsFilter
    {
        public string Title => $"{(Invert ? " NOT" : "")} {string.Join($" {(Comparison is TagComparison.All ? "AND" : "OR")} ", Tags.Where(t => t != null).Select(t => t.name.Split('.').Last()))}";

        [HideLabel]
        public List<ObjectTag> Tags = new();
        public TagComparison Comparison;
        public bool Invert;
        
        public enum TagComparison
        {
            Any,
            All
        }

        /// <summary>
        /// returns true if the filter has no tags, or if the checks pass
        /// </summary>
        /// <param name="tagOwner"></param>
        /// <returns></returns>
        /// <exception cref="ArgumentOutOfRangeException"></exception>
        public bool Check(ITagOwner tagOwner)
        {
            if (Tags.Count == 0)
            {
                return true;
            }

            var result = Comparison switch
            {
                TagComparison.Any => Tags.Any(tagOwner.HasTag),
                TagComparison.All => Tags.All(tagOwner.HasTag),
                _ => throw new ArgumentOutOfRangeException()
            };

            return Invert ? !result : result;
        }
    }
}