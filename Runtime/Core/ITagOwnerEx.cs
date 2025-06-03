using System.Linq;

namespace LowEndGames.ObjectTagSystem
{
    public static class ITagOwnerEx
    {
        public static void SetTag(this ITagOwner owner, ObjectTag objectTag, bool state)
        {
            if (state)
            {
                owner.AddTag(objectTag);
            }
            else
            {
                owner.RemoveTag(objectTag);
            }
        }

        public static bool HasAny(this ITagOwner owner, params ObjectTag[] tags) => tags.Any(owner.HasTag);

        public static bool HasAll(this ITagOwner owner, params ObjectTag[] tags) => tags.All(owner.HasTag);
    }
}