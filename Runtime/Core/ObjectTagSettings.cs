using System.Collections.Generic;
using UnityEngine;

namespace LowEndGames.ObjectTagSystem
{
    /// <summary>
    /// defines a Tag, will have a corresponding enum value generated
    /// </summary>
    [CreateAssetMenu(menuName = "ObjectTags System/Tag")]
    public class ObjectTagSettings : ScriptableObject
    {
        public ObjectTag Tag;
        
        [Tooltip("objects must pass this filter for this tag to be added")]
        public List<TagsFilter> Filters = new();
        
        [Tooltip("tag actions performed on the owner when this tag is added")]
        public List<TagAction> ActionsOnAdded = new(); 
        
        [Tooltip("tag actions performed on the owner when this tag is removed")]
        public List<TagAction> ActionsOnRemoved = new();

        [Tooltip("tags forced on while this tag is active")]
        public List<ObjectTag> ForcedTagsWhileActive = new();
        
        [Tooltip("tags blocked from being added while this tag is active")]
        public List<ObjectTag> BlockedTagsWhileActive = new();

        [Tooltip("behaviours which are active when this tag is on an object")]
        public List<TagBehaviourSettings> Behaviours = new();
    }
}