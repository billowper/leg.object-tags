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
            m_tagOwner.ApplyConfig(configuration);
        }

        public bool HasTag(ObjectTag objectTag) => m_tagOwner.HasTag(objectTag);

        public bool AddTag(ObjectTag objectTag, bool runFilters = true, bool force = false) => m_tagOwner.AddTag(objectTag, runFilters, force);

        public bool RemoveTag(ObjectTag objectTag, bool force = false) => m_tagOwner.RemoveTag(objectTag, force);

        public void AddTags(IEnumerable<ObjectTag> tags, bool runFilters = true, bool force = false) => m_tagOwner.AddTags(tags, runFilters, force);

        public void RemoveTags(IEnumerable<ObjectTag> tags, bool force = false) => m_tagOwner.RemoveTags(tags, force);

        public void AddBehaviour(TagBehaviourSettings tagBehaviourSettings) => m_tagOwner.AddBehaviour(tagBehaviourSettings);
        
        public void RemoveBehaviour(TagBehaviourSettings tagBehaviourSettings) => m_tagOwner.RemoveBehaviour(tagBehaviourSettings);
        
        public void BlockChangesWhile(CancelToken cancelToken)
        {
            m_tagOwner.BlockChangesWhile(cancelToken);
        }
        
        public void ClearAll() => m_tagOwner.ClearAll();

        // -------------------------------------------------- private
        
        private readonly TagOwner m_tagOwner = new TagOwner();

        private void Awake()
        {
            m_tagOwner.Init(m_configuration,
                    gameObject.name,
                    gameObject,
                    m_tagAdded,
                    m_tagRemoved,
                    m_tagsChanged);
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
                m_tagOwner.OnDrawGizmos(transform);
            }
        }
#endif
    }
}