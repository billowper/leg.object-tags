using System.Linq;
using LowEndGames.Utils;
using UnityEditor;
using UnityEditor.UIElements;
using UnityEngine.Pool;
using UnityEngine.UIElements;

namespace LowEndGames.ObjectTagSystem.EditorTools
{
    [CustomPropertyDrawer(typeof(TagsFilter))]
    public class TagsFilterDrawer : PropertyDrawer
    {
        public override VisualElement CreatePropertyGUI(SerializedProperty property)
        {
            var styles = AssetDatabase.LoadAssetAtPath("Packages/leg.object-tags/Editor/ObjectTagsUSS.uss", typeof(StyleSheet)) as StyleSheet;
            var root = new VisualElement();
            
            root.styleSheets.Add(styles);
            root.AddToClassList("box");

            new Label("Filter".Bold()).AddTo(root);
            var description = new Label(GetFilterTitle(property)).AddTo(root);
            
            new PropertyField(property.FindPropertyRelative(nameof(TagsFilter.Tags))).AddTo(root).RegisterValueChangeCallback(UpdateDescription);
            new PropertyField(property.FindPropertyRelative(nameof(TagsFilter.Comparison))).AddTo(root).RegisterValueChangeCallback(UpdateDescription);
            new PropertyField(property.FindPropertyRelative(nameof(TagsFilter.Invert))).AddTo(root).RegisterValueChangeCallback(UpdateDescription);
            
            return root;
            
            void UpdateDescription(SerializedPropertyChangeEvent evt)
            {
                description.text = GetFilterTitle(property);
            }
        }

        private string GetFilterTitle(SerializedProperty property)
        {
            var invert = property.FindPropertyRelative(nameof(TagsFilter.Invert)).boolValue;
            var comparison = (TagsFilter.TagComparison)property.FindPropertyRelative(nameof(TagsFilter.Comparison)).enumValueIndex;
            var separator = comparison is TagsFilter.TagComparison.All ? "AND" : "OR";

            var tagNames = ListPool<string>.Get();

            var tagsArrayProp = property.FindPropertyRelative(nameof(TagsFilter.Tags));
            
            for (int i = 0; i < tagsArrayProp.arraySize; i++)
            {
                var tag = tagsArrayProp.GetArrayElementAtIndex(i).objectReferenceValue as ObjectTag;
                if (tag != null)
                {
                    tagNames.Add(tag.name.Split('.').Last());
                }
            }
            
            var result = $"{(invert ? " NOT" : "")} {string.Join($" {separator} ", tagNames)}";
            
            ListPool<string>.Release(tagNames);
            
            return result;
        }
    }
}