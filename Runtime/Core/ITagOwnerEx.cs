using System.Linq;

namespace LowEndGames.ObjectTagSystem
{
    public static class ITagOwnerEx
    {
        public static bool HasTag(this ITagOwner owner, string tag) => owner.HasTag(ObjectTag.Create(tag));
        
        public static bool HasAny(this ITagOwner owner, params string[] tags) => tags.Any(tag => owner.HasTag(ObjectTag.Create(tag)));
        
        public static bool HasAll(this ITagOwner owner, params string[] tags) => tags.All(tag => owner.HasTag(ObjectTag.Create(tag)));
        
        public static void AddTag(this ITagOwner owner, string tag, bool runFilters = true, bool force = false) => owner.AddTag(ObjectTag.Create(tag), runFilters, force);
        
        public static bool RemoveTag(this ITagOwner owner, string tag, bool force = false) => owner.RemoveTag(ObjectTag.Create(tag), force);
        
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

        public static void SetTag(this ITagOwner owner, string tag, bool state)
        {
            if (state)
            {
                owner.AddTag(tag);
            }
            else
            {
                owner.RemoveTag(tag);
            }
        }
        
        public static bool HasAny(this ITagOwner owner, params ObjectTag[] tags) => tags.Any(owner.HasTag);

        public static bool HasAll(this ITagOwner owner, params ObjectTag[] tags) => tags.All(owner.HasTag);
    }
}