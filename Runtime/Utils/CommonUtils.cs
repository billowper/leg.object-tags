
using UnityEngine;

namespace LowEndGames.ObjectTagSystem
{
    public static class CommonUtils
    {
        public static bool TryGetComponentInParent<T>(this Component c, out T result, bool includeInactive  = false)
        {
            result = c.GetComponentInParent<T>(includeInactive);
            return result != null;
        }
    }
}