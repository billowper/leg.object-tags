using System;
using Sirenix.OdinInspector;

namespace LowEndGames.ObjectTagSystem
{
    [Serializable]
    public class TagAction
    {
        public enum TagActions
        {
            Add,
            Remove
        }

        [HideLabel, HorizontalGroup] public TagActions Action;
        [HideLabel, HorizontalGroup] public ObjectTag Tag;

        public void Apply(ITagOwner target)
        {
            switch (Action)
            {
                case TagActions.Add:
                    target.AddTag(Tag);
                    break;
                case TagActions.Remove:
                    target.RemoveTag(Tag);
                    break;
                default:
                    throw new ArgumentOutOfRangeException();
            }
        }
    }
}