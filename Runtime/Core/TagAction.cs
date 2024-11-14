using System;

namespace LowEndGames.ObjectTagSystem
{
    /// <summary>
    /// Add/Remove <see cref="ObjectTag"/>s from a <see cref="ITagOwner"/>
    /// </summary>
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

        public void Apply(ITagOwner target, bool force)
        {
            switch (Action)
            {
                case TagActions.Add:
                    target.AddTag(Tag, true, force);
                    break;
                case TagActions.Remove:
                    target.RemoveTag(Tag, force);
                    break;
                default:
                    throw new ArgumentOutOfRangeException();
            }
        }
    }
}