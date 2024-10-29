using System.Collections.Generic;
using System.Linq;
using System.Text.RegularExpressions;

namespace LowEndGames.ObjectTagSystem
{
    public static class ObjectTagsUtils
    {
        public static bool EvaluateFilters(this IEnumerable<TagsFilter> filters, ITagOwner tagsComponent)
        {
            foreach (var f in filters)
            {
                if (f.Check(tagsComponent) == false)
                {
                    return false;
                }
            }

            return true;
        }

        public static void ApplyTo(this IEnumerable<TagAction> tagActions, ITagOwner tagsComponent)
        {
            foreach (var tagAction in tagActions)
            {
                tagAction.Apply(tagsComponent);
            }
        }
    }
}