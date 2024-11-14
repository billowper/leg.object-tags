using System;
using System.Linq;

namespace LowEndGames.ObjectTagSystem
{
    public static class ITagOwnerEx
    {
        public static bool HasTag(this ITagOwner owner, Enum enumValue) => owner.HasTag(enumValue.ToAsset());
        public static bool HasAny(this ITagOwner owner, params ObjectTag[] tags) => tags.Any(owner.HasTag);
        public static bool HasAny(this ITagOwner owner, params Enum[] tags) => tags.Any(tag => owner.HasTag(tag.ToAsset()));
        public static bool HasAll(this ITagOwner owner, params ObjectTag[] tags) => tags.All(owner.HasTag);
        public static bool HasAll(this ITagOwner owner, params Enum[] tags) => tags.All(tag => owner.HasTag(tag.ToAsset()));
        
        public static void AddTag(this ITagOwner owner, Enum enumValue, bool runFilters = true, bool force = false) => owner.AddTag(enumValue.ToAsset(), runFilters, force);
        
        public static bool RemoveTag(this ITagOwner owner, Enum enumValue, bool force = false) => owner.RemoveTag(enumValue.ToAsset(), force);
        
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

        public static void SetTag(this ITagOwner owner, Enum objectTag, bool state)
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
    }
}