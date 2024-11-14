using System.Collections.Generic;
using UnityEditor;
using UnityEngine;

namespace LowEndGames.ObjectTagSystem.EditorTools
{
    public class ObjectTagsSettings : ScriptableObject
    {
        public string GenerationFolder = "Scripts/Generated/";
        public string GenerationCodeNamespace = "LowEndGames.Generated";
        public bool DetectNewTagsOnImport = true;
        
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
            var provider = new SettingsProvider("Project/LowEndGames/Object Tags", SettingsScope.Project)
            {
                label = "Object Tags",
                guiHandler = (_) =>
                {
                    var settings = GetSerializedSettings();
                    EditorGUILayout.PropertyField(settings.FindProperty(nameof(GenerationFolder)));
                    EditorGUILayout.PropertyField(settings.FindProperty(nameof(GenerationCodeNamespace)));
                    EditorGUILayout.PropertyField(settings.FindProperty(nameof(DetectNewTagsOnImport)));
                    settings.ApplyModifiedPropertiesWithoutUndo();
                },

                keywords = new HashSet<string>(new[] { "ObjectTags" })
            };

            return provider;
        }
    }
}