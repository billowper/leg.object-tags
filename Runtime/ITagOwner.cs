using System;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;

namespace LowEndGames.ObjectTagSystem
{
    public interface ITagOwner
    {
        ObjectTagEvent TagAdded { get; }
        ObjectTagEvent TagRemoved { get; }
        UnityEvent TagsChanged { get; }
        IEnumerable<ObjectTag> Tags { get; }
        Transform transform { get; }
        bool HasTag(ObjectTag objectTag);
        bool HasTag(Enum enumValue);
        bool AddTag(ObjectTag objectTag, bool runFilters = true);
        void AddTag(Enum enumValue, bool runFilters = true);
        bool RemoveTag(ObjectTag objectTag);
        bool RemoveTag(Enum enumValue);
        void AddTags(IEnumerable<ObjectTag> tags, bool runFilters = true);
        void RemoveTags(IEnumerable<ObjectTag> tags);
        bool HasAny(IEnumerable<ObjectTag> tags);
        void AddBehaviour(BaseTagBehaviourSettings tagBehaviourSettings);
        void RemoveBehaviour(BaseTagBehaviourSettings tagBehaviourSettings);
    }
}