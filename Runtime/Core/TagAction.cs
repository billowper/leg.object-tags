using System;

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

        public TagActions Action;
        public ObjectTag Tag;

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