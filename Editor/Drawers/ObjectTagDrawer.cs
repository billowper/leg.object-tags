using System.Linq;
using UnityEditor;
using UnityEngine.UIElements;

namespace LowEndGames.ObjectTagSystem.EditorTools
{
    [CustomPropertyDrawer(typeof(ObjectTag))]
    public class ObjectTagDrawer : PropertyDrawer
    {
        public override VisualElement CreatePropertyGUI(SerializedProperty property)
        {
            var valueProp = property.FindPropertyRelative("m_value");
            var tagName = valueProp.stringValue;

            var allTags = ObjectTags.GetAll().OrderBy(t => t).Select(t => t.ToString()).ToList();

            if (!ObjectTags.IsRegisteredTag(tagName))
            {
                tagName = allTags.First();
                valueProp.stringValue = tagName;
                property.serializedObject.ApplyModifiedProperties();
            }
            
            var element = new DropdownField(property.displayName)
            {
                value = tagName,
                choices = allTags
            };

            element.RegisterValueChangedCallback(evt =>
            {
                valueProp.stringValue = evt.newValue;
                property.serializedObject.ApplyModifiedProperties();
                element.label = evt.newValue;
            });
            
            return element;
        }
    }
}