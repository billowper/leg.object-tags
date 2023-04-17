using System;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;

namespace LowEndGames.ObjectTagSystem
{
    public class TagEvents : MonoBehaviour
    {
        [SerializeField] private TaggedObject m_myTagsComponent;
        [SerializeField] private List<EventResponse> m_eventResponses;

        [Serializable]
        public class EventResponse
        {
            public ObjectTag Tag;
            public UnityEvent Added;
            public UnityEvent Removed;
        }

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