using System;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

namespace LowEndGames.ObjectTagSystem
{
    /// <summary>
    /// loads all tags via Resources API 
    /// </summary>
    public static class ObjectTagsLoader
    {
        private static List<ObjectTag> s_tags;

        [RuntimeInitializeOnLoadMethod(RuntimeInitializeLoadType.BeforeSceneLoad)]
        private static void Init()
        {
            s_tags = new List<ObjectTag>(Resources.LoadAll<ObjectTag>(""));
        }
        
        public static IReadOnlyList<ObjectTag> Tags => s_tags;

        public static ObjectTag ToAsset(this Enum tagsEnum)
        {
            return s_tags.First(t => t.name.Split('.').Last() == tagsEnum.ToString());
        }
    }
}