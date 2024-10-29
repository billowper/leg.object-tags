using System;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;

namespace LowEndGames.ObjectTagSystem
{
    /// <summary>
    /// listens to a <see cref="TaggedObject"/>'s Add/Remove events and invokes UnityEvent responses.
    /// </summary>
    public class TaggedObjectEvents : MonoBehaviour
    {
        [SerializeField] private TaggedObject m_myTagsComponent;
        [SerializeField] private List<EventResponse> m_eventResponses;

        // -------------------------------------------------- public
        
        [Serializable]
        public class EventResponse
        {
            public ObjectTag Tag;
            public UnityEvent Added;
            public UnityEvent Removed;
        }
        
        // -------------------------------------------------- private
        
        private void Awake()
        {
            m_myTagsComponent.TagAdded.AddListener(OnTagAdded);
            m_myTagsComponent.TagRemoved.AddListener(OnTagRemoved);
        }

        private void OnTagAdded(ObjectTag objectTag)
        {
            foreach (var response in m_eventResponses)
            {
                if (response.Tag == objectTag)
                {
                    response.Added.Invoke();
                }
            }
        }
        
        private void OnTagRemoved(ObjectTag objectTag)
        {
            foreach (var response in m_eventResponses)
            {
                if (response.Tag == objectTag)
                {
                    response.Removed.Invoke();
                }
            }
        }
    }
}