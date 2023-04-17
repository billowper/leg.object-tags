using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Reflection;
using LowEndGames.EditorTools;
using LowEndGames.Utils;
using UnityEditor;
using UnityEngine;

namespace LowEndGames.ObjectTagSystem.EditorTools
{
    public class AssertImporter : AssetPostprocessor
    {
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
            
            const string TYPE_NAME = "ObjectTags";
            
            if (File.Exists(Path.Combine(Application.dataPath, $"Plugins/{TYPE_NAME}.dll")) == false)
            {
                var generator = new EnumGenerator.Generator(TYPE_NAME, new List<string> {"NULL"}, "LowEndGames.ObjectTagSystem");

                Debug.Log($"Generating {TYPE_NAME} enum.");

                EnumGenerator.GenerateEnumDll(TYPE_NAME, new List<EnumGenerator.Generator> {generator});
                return;
            }

            var enumType = TypeCache.GetTypesDerivedFrom(typeof(Enum)).FirstOrDefault(t => t.Name == TYPE_NAME);
            if (enumType != null)
            {
                var assets = AssetStorage.GetAssets(typeof(ObjectTag));
                var enumNames = Enum.GetNames(enumType);
                if (assets.Count != enumNames.Length || assets.All(a => enumNames.Contains(GetEnumValueName(a.name))) == false)
                {
                    Debug.Log($"Found {assets.Count} Object Tag assets, generating new Enum type in code.");

                    if (EditorUtility.DisplayDialog("ObjectTags Enum is out of date!", "Do you want to regenerate the ObjectTags DLL now?", "Yes", "No"))
                    {
                        var assetNames = assets.Select(a => GetEnumValueName(a.name)).ToList();
                        var generator = new EnumGenerator.Generator(TYPE_NAME, assetNames, "LowEndGames.ObjectTagSystem");
                        
                        EnumGenerator.GenerateEnumDll(TYPE_NAME, new List<EnumGenerator.Generator> {generator});
                    }
                }
            }
        }

        private static string GetEnumValueName(string assetName)
        {
            return assetName.WithoutWhitespace().Split('.').Last().StripNonAlphaNumeric();
        }
    }
}