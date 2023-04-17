using System;
using System.Collections.Generic;
using Sirenix.OdinInspector;
using UnityEngine;
using UnityEngine.Events;

namespace LowEndGames.ObjectTagSystem
{
    [DefaultExecutionOrder(-100)]
    public class TaggedObject : MonoBehaviour, ITagOwner
    {
        [SerializeField] private TagOwnerConfiguration m_configuration;
        
        [FoldoutGroup("Events")]
        [SerializeField] private ObjectTagEvent m_tagAdded;
        [FoldoutGroup("Events")]
        [SerializeField] private ObjectTagEvent m_tagRemoved;
        [FoldoutGroup("Events")]
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

        public void HasTag(Enum enumValue) => m_tagOwner.HasTag(enumValue);

        public bool AddTag(ObjectTag objectTag, bool runFilters = true) => m_tagOwner.AddTag(objectTag, runFilters);

        public void AddTag(Enum enumValue, bool runFilters = true) => m_tagOwner.AddTag(enumValue, runFilters);

        public bool RemoveTag(ObjectTag objectTag) => m_tagOwner.RemoveTag(objectTag);

        public bool RemoveTag(Enum enumValue) => m_tagOwner.RemoveTag(enumValue);

        public void AddTags(IEnumerable<ObjectTag> tags, bool runFilters = true) => m_tagOwner.AddTags(tags, runFilters);

        public void RemoveTags(IEnumerable<ObjectTag> tags) => m_tagOwner.RemoveTags(tags);

        public bool HasAny(IEnumerable<ObjectTag> tags) => m_tagOwner.HasAny(tags);
        
        public void AddBehaviour(BaseTagBehaviourSettings tagBehaviourSettings) => m_tagOwner.AddBehaviour(tagBehaviourSettings);
        
        public void RemoveBehaviour(BaseTagBehaviourSettings tagBehaviourSettings) => m_tagOwner.RemoveBehaviour(tagBehaviourSettings);

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

        private void FixedUpdate()
        {
            m_tagOwner.FixedUpdate();
        }

#if UNITY_EDITOR
        private void OnDrawGizmos()
        {
            if (m_tagOwner != null)
            {
                m_tagOwner.OnDrawGizmos();
            }
            else
            {
                m_configuration.DrawGizmos(transform);
            }
        }
#endif
    }
}