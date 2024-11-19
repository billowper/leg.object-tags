using System.Globalization;
using UnityEditor;
using UnityEditor.UIElements;
using UnityEngine;
using UnityEngine.UIElements;

namespace LowEndGames.ObjectTagSystem.EditorTools
{
    [CustomEditor(typeof(TagChangeRule))]
    public class TagChangeRuleEditor : Editor
    {
        [SerializeField] private VisualTreeAsset m_treeAsset;
        
        private TemplateContainer m_tree;

        public override VisualElement CreateInspectorGUI()
        {
            m_tree = m_treeAsset.Instantiate();

            void OnValueChange(SerializedObject o)
            {
                var label = m_tree.Q<Label>("DescriptionLabel");
                var rule = (TagChangeRule)target;
                label.text = rule.GetDescriptionString(rule.RequiredTime.ToString(CultureInfo.InvariantCulture));
            }
            
            m_tree.TrackSerializedObjectValue(serializedObject, OnValueChange);
            OnValueChange(serializedObject);
            return m_tree;
        }
    }
}