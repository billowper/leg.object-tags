using System.Linq;
using UnityEditor;
using UnityEditor.UIElements;
using UnityEngine.UIElements;

namespace LowEndGames.ObjectTagSystem.EditorTools
{
    [CustomEditor(typeof(TaggedObject))]
    public class TaggedObjectEditor : Editor
    {
        public override VisualElement CreateInspectorGUI()
        {
            var styles = AssetDatabase.LoadAssetAtPath("Packages/leg.object-tags/Editor/ObjectTagsUSS.uss", typeof(StyleSheet)) as StyleSheet;
            var root = new VisualElement();
            
            root.styleSheets.Add(styles);
            
            var taggedObject = (TaggedObject)target;

            if (taggedObject.GetActiveTags().Any())
                new Label()
                {
                    text = "<b>Tags:</b>\n" + string.Join("\n", taggedObject.GetActiveTags().Select(t => $"{t.name.Split('.').Last()} : {taggedObject.GetTagTime(t):F2}"))
                }.AddTo(root).AddToClassList("info-box");

            new PropertyField(serializedObject.FindProperty("m_configuration")).AddTo(root).AddToClassList("box");
            new PropertyField(serializedObject.FindProperty("m_tagAdded")).AddTo(root);
            new PropertyField(serializedObject.FindProperty("m_tagRemoved")).AddTo(root);
            new PropertyField(serializedObject.FindProperty("m_tagsChanged")).AddTo(root);

            return root;
        }
    }
}