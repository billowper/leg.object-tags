using System.Globalization;
using UnityEditor;
using UnityEditor.UIElements;
using UnityEngine.UIElements;

namespace LowEndGames.ObjectTagSystem.EditorTools
{
    [CustomPropertyDrawer(typeof(InteractionRuleTimeOverride))]
    public class InteractionRuleTimeOverrideDrawer : PropertyDrawer
    {
        public override VisualElement CreatePropertyGUI(SerializedProperty property)
        {
            var root = new VisualElement();

            var ruleProp = property.FindPropertyRelative(nameof(InteractionRuleTimeOverride.Rule));
            var overrideTimeProp = property.FindPropertyRelative(nameof(InteractionRuleTimeOverride.OverrideRequiredTime));
            
            var labelField = new Label().AddTo(root);
            
            if (ruleProp.objectReferenceValue is TagChangeRule rule)
            {
                labelField.text = rule.GetDescriptionString(overrideTimeProp.floatValue.ToString(CultureInfo.InvariantCulture));
            }
            
            new PropertyField(ruleProp).AddTo(root).RegisterValueChangeCallback(OnValuesChanged);
            new PropertyField(overrideTimeProp).AddTo(root).RegisterValueChangeCallback(OnValuesChanged);
            
            return root;
            
            void OnValuesChanged(SerializedPropertyChangeEvent evt)
            {
                if (ruleProp.objectReferenceValue is TagChangeRule r)
                {
                    labelField.text = r.GetDescriptionString(overrideTimeProp.floatValue.ToString(CultureInfo.InvariantCulture));
                }
            }
        }
    }
}