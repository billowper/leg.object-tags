using UnityEngine.UIElements;

namespace LowEndGames.ObjectTagSystem.EditorTools
{
    public static class UIElementsUtils
    {
        public static T AddTo<T>(this T element, VisualElement parent)
            where T : VisualElement
        {
            parent.Add(element);
            return element;
        }
    }
}