using UnityEditor;
using UnityEditor.UIElements;
using UnityEngine.UIElements;

namespace LowEndGames.ObjectTagSystem.EditorTools
{
    [CustomPropertyDrawer(typeof(TagAction))]
    public class TagActionDrawer : PropertyDrawer
    {
        public override VisualElement CreatePropertyGUI(SerializedProperty property)
        {
            var styles = AssetDatabase.LoadAssetAtPath("Packages/leg.object-tags/Editor/ObjectTagsUSS.uss", typeof(StyleSheet)) as StyleSheet;
            
            var root = new VisualElement();
            
            root.styleSheets.Add(styles);
            root.AddToClassList("flex-row");
            
            new PropertyField(property.FindPropertyRelative(nameof(TagAction.Action)), string.Empty).AddTo(root).AddToClassList("fg-30");
            new PropertyField(property.FindPropertyRelative(nameof(TagAction.Tag)), string.Empty).AddTo(root).AddToClassList("fg-100");
            
            return root;
        }
    }
}