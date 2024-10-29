using System;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;

namespace LowEndGames.ObjectTagSystem
{
    [DefaultExecutionOrder(-100)]
    public class TaggedObject : MonoBehaviour, ITagOwner
    {
        [SerializeField] private TagOwnerConfiguration m_configuration;
        [SerializeField] private ObjectTagEvent m_tagAdded;
        [SerializeField] private ObjectTagEvent m_tagRemoved;
        [SerializeField] private UnityEvent m_tagsChanged;
        
        // -------------------------------------------------- public
        
        public ObjectTagEvent TagAdded => m_tagOwner.TagAdded;

        public ObjectTagEvent TagRemoved => m_tagOwner.TagRemoved;

        public UnityEvent TagsChanged => m_tagOwner.TagsChanged;

        public IEnumerable<ObjectTag> Tags => m_tagOwner.Tags;

        public void ApplyConfig(TagOwnerConfiguration configuration)
        {
            m_configuration = configuration;

            if (m_tagOwner == null)
            {
                m_tagOwner = new TagOwner(m_configuration,
                    gameObject.name,
                    transform,
                    m_tagAdded,
                    m_tagRemoved,
                    m_tagsChanged);
            }
            else
            {
                m_tagOwner.ApplyConfig(configuration);
            }
        }

        public bool HasTag(ObjectTag objectTag) => m_tagOwner.HasTag(objectTag);

        public bool HasTag(Enum enumValue) => m_tagOwner.HasTag(enumValue);

        public bool AddTag(ObjectTag objectTag, bool runFilters = true) => m_tagOwner.AddTag(objectTag, runFilters);

        public void AddTag(Enum enumValue, bool runFilters = true) => m_tagOwner.AddTag(enumValue, runFilters);

        public bool RemoveTag(ObjectTag objectTag) => m_tagOwner.RemoveTag(objectTag);

        public bool RemoveTag(Enum enumValue) => m_tagOwner.RemoveTag(enumValue);

        public void AddTags(IEnumerable<ObjectTag> tags, bool runFilters = true) => m_tagOwner.AddTags(tags, runFilters);

        public void RemoveTags(params ObjectTag[] tags) => m_tagOwner.RemoveTags(tags);

        public bool HasAny(params ObjectTag[] tags) => m_tagOwner.HasAny(tags);
        
        public bool HasAll(params ObjectTag[] tags) => m_tagOwner.HasAll(tags);
        
        public bool HasAny(params Enum[] tags) => m_tagOwner.HasAny(tags);
        
        public bool HasAll(params Enum[] tags) => m_tagOwner.HasAll(tags);
        
        public void AddBehaviour(TagBehaviourSettings tagBehaviourSettings) => m_tagOwner.AddBehaviour(tagBehaviourSettings);
        
        public void RemoveBehaviour(TagBehaviourSettings tagBehaviourSettings) => m_tagOwner.RemoveBehaviour(tagBehaviourSettings);
        
        public void ClearTags()
        {
            m_tagOwner.ClearAll();
        }

        // -------------------------------------------------- private
        
        private TagOwner m_tagOwner;

        private void Awake()
        {
            if (m_tagOwner == null)
            {
                m_tagOwner = new TagOwner(m_configuration,
                    gameObject.name,
                    transform,
                    m_tagAdded,
                    m_tagRemoved,
                    m_tagsChanged);
            }
        }

        private void Update()
        {
            m_tagOwner.Update();
        }

#if UNITY_EDITOR
        private void OnDrawGizmos()
        {
            if (m_tagOwner != null)
            {
                m_tagOwner.OnDrawGizmos();
            }
        }
#endif
    }
}