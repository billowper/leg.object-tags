using System;

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