using System.Linq;
using UnityEngine;

namespace LowEndGames.ObjectTagSystem
{
    public static class ObjectTagsLoader
    {
        private static ObjectTag[] s_tags;

        [RuntimeInitializeOnLoadMethod(RuntimeInitializeLoadType.BeforeSceneLoad)]
        private static void Init()
        {
            s_tags = Resources.LoadAll<ObjectTag>("").ToArray();
        }

        public static ObjectTag ToAsset(this ObjectTags tagsEnum)
        {
            return s_tags.First(t => t.name.Split('.').Last() == tagsEnum.ToString());
        }
    }
}