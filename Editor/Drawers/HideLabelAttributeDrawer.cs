using LowEndGames.ObjectTagSystem.Attributes;
using UnityEditor;
using UnityEditor.UIElements;
using UnityEngine.UIElements;

namespace LowEndGames.ObjectTagSystem.EditorTools
{
    [CustomPropertyDrawer(typeof(HideLabelAttribute))]
    public class HideLabelAttributeDrawer : PropertyDrawer
    {
        public override VisualElement CreatePropertyGUI(SerializedProperty property)
        {
            return new PropertyField(property, "");
        }
    }
}