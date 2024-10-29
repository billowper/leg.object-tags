using System.Linq;
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
            var root = new VisualElement();

            var title = new Label(GetFilterTitle(property)).AddTo(root);
            
            new PropertyField(property.FindPropertyRelative(nameof(TagsFilter.Tags))).AddTo(root).RegisterValueChangeCallback(UpdateTitle);
            new PropertyField(property.FindPropertyRelative(nameof(TagsFilter.Comparison))).AddTo(root).RegisterValueChangeCallback(UpdateTitle);
            new PropertyField(property.FindPropertyRelative(nameof(TagsFilter.Invert))).AddTo(root).RegisterValueChangeCallback(UpdateTitle);
            
            return root;
            
            void UpdateTitle(SerializedPropertyChangeEvent evt)
            {
                title.text = GetFilterTitle(property);
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