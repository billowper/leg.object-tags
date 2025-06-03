using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Runtime.CompilerServices;
using UnityEngine;

#if UNITY_EDITOR
using UnityEditor;
#endif

namespace LowEndGames.ObjectTagSystem
{
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
            
            var tag = ObjectTag.Create(tagName);

            for (int i = 0; i < m_tags.Length; i++)
            {
                if (!m_tags[i].IsValid())
                {
                    m_tags[i] = tag;
                    return tag;
                }
            }

            Debug.LogError($"Hit max tags! ({m_tags.Length})");

            return default;
        }

        public static IEnumerable<ObjectTag> GetAll()
        {
            for (int i = 0; i < m_tags.Length; i++)
            {
                if (m_tags[i].IsValid())
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
    
        private static readonly ObjectTag[] m_tags = new ObjectTag[256];
        private static Dictionary<string, ObjectTagSettings> m_tagSettings = new Dictionary<string, ObjectTagSettings>(256);
        
#if UNITY_EDITOR
        [InitializeOnLoadMethod]
#endif
        [RuntimeInitializeOnLoadMethod(RuntimeInitializeLoadType.BeforeSplashScreen)]
        private static void Init()
        {
            var assemblies = AppDomain.CurrentDomain.GetAssemblies();
            var tagProviderTypes = assemblies
                    .SelectMany(a => a.GetTypes()
                    .Where(t => t.GetCustomAttribute(typeof(ObjectTagsProviderAttribute)) != null))
                    .ToArray();
            
            foreach (var type in tagProviderTypes)
            {
                RuntimeHelpers.RunClassConstructor(type.TypeHandle);
            } 
            
            m_tagSettings.Clear();
            
            var tagSettingsArray = Resources.LoadAll<ObjectTagSettings>("Tags");

            foreach (var settings in tagSettingsArray)
            {
                m_tagSettings.Add(settings.Tag, settings);
            }

            Debug.Log($"ObjectTags: {string.Join("\n", m_tags.Where(t => t.IsValid()).Select(t => t.ToString()))}");
        }
    }
}