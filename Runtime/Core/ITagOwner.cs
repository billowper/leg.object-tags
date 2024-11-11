using System.Collections.Generic;
using UnityEngine.Events;

namespace LowEndGames.ObjectTagSystem
{
    public interface ITagOwner
    {
        ObjectTagEvent TagAdded { get; }
        ObjectTagEvent TagRemoved { get; }
        UnityEvent TagsChanged { get; }
        IEnumerable<ObjectTag> Tags { get; }
        
        bool HasTag(ObjectTag objectTag);
        bool AddTag(ObjectTag objectTag, bool runFilters = true, bool force = false);
        void AddTags(IEnumerable<ObjectTag> tags, bool runFilters = true, bool force = false);
        bool RemoveTag(ObjectTag objectTag, bool force = false);
        void RemoveTags(IEnumerable<ObjectTag> tags, bool force = false);

        void AddBehaviour(TagBehaviourSettings tagBehaviourSettings);
        void RemoveBehaviour(TagBehaviourSettings tagBehaviourSettings);
        void BlockChangesWhile(CancelToken cancelToken);

        void ClearAll();
    }
}