using System.Collections.Generic;
using UnityEditor;
using UnityEngine;

namespace LowEndGames.ObjectTagSystem.EditorTools
{
    public class ObjectTagsSettings : ScriptableObject
    {
        public string GenerationCodeNamespace = "ObjectTagsSystem";
        
        public const string AssetPath = "Assets/Editor/ObjectTagsSystemSettings.asset";
        
        internal static ObjectTagsSettings GetOrCreateSettings()
        {
            var settings = AssetDatabase.LoadAssetAtPath<ObjectTagsSettings>(AssetPath);
            if (settings == null)
            {
                settings = CreateInstance<ObjectTagsSettings>();
                
                if (!AssetDatabase.IsValidFolder("Assets/Editor"))
                    AssetDatabase.CreateFolder("Assets", "Editor");
                
                AssetDatabase.CreateAsset(settings, AssetPath);
                AssetDatabase.SaveAssets();
            }
            return settings;
        }

        internal static SerializedObject GetSerializedSettings()
        {
            return new SerializedObject(GetOrCreateSettings());
        }
        
        [SettingsProvider]
        public static SettingsProvider SettingsProvider()
        {
            var provider = new SettingsProvider("Project/ObjectTags System", SettingsScope.Project)
            {
                label = "Object Tags",
                guiHandler = (_) =>
                {
                    var settings = GetSerializedSettings();
                    EditorGUILayout.PropertyField(settings.FindProperty(nameof(GenerationCodeNamespace)));
                    settings.ApplyModifiedPropertiesWithoutUndo();
                },

                keywords = new HashSet<string>(new[] { "ObjectTags" })
            };

            return provider;
        }
    }
}