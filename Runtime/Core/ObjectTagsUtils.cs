using System.Collections.Generic;

namespace LowEndGames.ObjectTagSystem
{
    /// <summary>
    /// some extension methods
    /// </summary>
    public static class ObjectTagsUtils
    {
        public static bool EvaluateFilters(this IEnumerable<TagsFilter> filters, ITagOwner tagOwner)
        {
            foreach (var f in filters)
            {
                if (f.Check(tagOwner) == false)
                {
                    return false;
                }
            }

            return true;
        }

        public static void ApplyTo(this IEnumerable<TagAction> tagActions, ITagOwner tagOwner, bool force)
        {
            foreach (var tagAction in tagActions)
            {
                tagAction.Apply(tagOwner, force);
            }
        }
    }
}