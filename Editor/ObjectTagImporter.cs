using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text.RegularExpressions;
using UnityEditor;
using UnityEngine;

namespace LowEndGames.ObjectTagSystem.EditorTools
{
    public class ObjectTagImporter : AssetPostprocessor
    {
        const string TYPE_NAME = "ObjectTags";

        private static void OnPostprocessAllAssets(
            string[] importedAssets,
            string[] deletedAssets,
            string[] movedAssets,
            string[] movedFromAssetPaths)
        {
            var importedTags = importedAssets.Any(a => AssetDatabase.GetMainAssetTypeAtPath(a) == typeof(ObjectTag));
            if (importedTags == false)
            {
                return;
            }
            
            var assetNames = GetAllObjectTags().Select(a => a.GetEnumValueName()).ToList();
            
            string filePath;
            
            var enumType = TypeCache.GetTypesDerivedFrom(typeof(Enum)).FirstOrDefault(t => t.Name == TYPE_NAME);
            if (enumType == null)
            {
                filePath = $"Assets/Generated/{TYPE_NAME}.cs";

                Debug.Log($"Couldn't find '{TYPE_NAME}.cs' in project, generating at '{filePath}'.");
                GenerateEnum(filePath, assetNames);
                return;
            }
            
            var generatedEnumFile = AssetDatabase.FindAssets($"t:TextAsset {TYPE_NAME}");

            filePath = AssetDatabase.GUIDToAssetPath(generatedEnumFile[0]);
            
            var enumNames = Enum.GetNames(enumType);
            if (assetNames.Count != enumNames.Length || assetNames.All(a => enumNames.Contains(a)) == false)
            {
                if (EditorUtility.DisplayDialog("ObjectTags Enum is out of date!", "Do you want to run code-generation now?", "Yes", "No"))
                {
                    Debug.Log($"Re-generating '{filePath}'");

                    GenerateEnum(filePath, assetNames);
                }
            }
            else
            {
                Debug.Log($"Found generated enum at '{filePath}', contains entries for all {assetNames.Count} ObjectTag assets");
            }
        }

        private static List<ObjectTag> GetAllObjectTags()
        {
            var guids = AssetDatabase.FindAssets($"t:{nameof(ObjectTag)}");
            return guids.Select(guid => AssetDatabase.LoadAssetAtPath(AssetDatabase.GUIDToAssetPath(guid), typeof(ObjectTag)) as ObjectTag).ToList();
        }

        private static void GenerateEnum(string projectRelativePath, List<string> values)
        {
            // Ensure the directory exists
            var basePath = Path.Combine(Application.dataPath, "Generated/");
            if (!Directory.Exists(basePath))
                Directory.CreateDirectory(basePath);

            // Generate the enum content as a string
            var enumContent = GenerateEnumContent(values);

            // Write to a .cs file
            var filePath = $"{basePath}{TYPE_NAME}.cs";
            File.WriteAllText(filePath, enumContent);
            
            AssetDatabase.ImportAsset(projectRelativePath);
            AssetDatabase.Refresh();
        }

        private static string GenerateEnumContent(List<string> values)
        {
            var settings = ObjectTagsSettings.GetOrCreateSettings();

            var namespaceDeclaration = string.IsNullOrEmpty(settings.GenerationCodeNamespace) ? "" : $"namespace {settings.GenerationCodeNamespace}\n{{\n";
            var enumDeclaration = $"public enum {TYPE_NAME}\n{{\n";
            
            for (int i = 0; i < values.Count; i++)
            {
                enumDeclaration += $"    {values[i].Replace(" ", "")} = {i},\n";
            }

            enumDeclaration += "}\n";
            var closingBracket = string.IsNullOrEmpty(settings.GenerationCodeNamespace) ? "" : "}";

            return $"{namespaceDeclaration}{enumDeclaration}{closingBracket}";
        }
    }
}