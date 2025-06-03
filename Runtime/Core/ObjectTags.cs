using System;
using System.Collections.Generic;
using System.Linq;
using UnityEditor;
using UnityEngine;

namespace LowEndGames.ObjectTagSystem
{
    #if UNITY_EDITOR
    [InitializeOnLoad]
    #endif
    public static class ObjectTags
    {
        // ------------------------------------------------- public 

        public static ObjectTag Register(string tagName)
        {
            for (int idx = 0; idx < m_tags.Length; idx++)
            {
                if (m_tags[idx] == tagName)
                {
                    return m_tags[idx];
                }
            }
            
            var count = m_count;

            var tag = ObjectTag.Create(tagName);

            m_count++;
            m_tags[count] = tag;

            return tag;
        }

        public static IEnumerable<ObjectTag> GetAll()
        {
            for (int i = 0; i < m_count; i++)
            {
                yield return m_tags[i];
            }
        }

        public static bool IsRegisteredTag(string tag)
        {
            return m_tags.Any(t => t == tag);
        }

        public static bool GetTagSettings(ObjectTag tag, out ObjectTagSettings settings)
        {
            return m_tagSettings.TryGetValue(tag, out settings);
        }
        
        // ------------------------------------------------- private
    
        private static int m_count;
        private static readonly ObjectTag[] m_tags = new ObjectTag[256];
        private static Dictionary<string, ObjectTagSettings> m_tagSettings = new Dictionary<string, ObjectTagSettings>(256);
        
        static ObjectTags()
        {
            m_count = 0;
            m_tagSettings.Clear();
        }

        #if UNITY_EDITOR
        [InitializeOnLoadMethod]
        #endif
        [RuntimeInitializeOnLoadMethod(RuntimeInitializeLoadType.BeforeSceneLoad)]
        private static void Init()
        {
            var tagSettingsArray = Resources.LoadAll<ObjectTagSettings>("Tags");

            foreach (var settings in tagSettingsArray)
            {
                m_tagSettings.Add(settings.Tag, settings);
            }

            Debug.Log($"ObjectTags: {string.Join(",", m_tags.Select(t => t.ToString()))}");
        }
    }
}