using System;
using UnityEngine;
using UnityEngine.Assertions;

namespace LowEndGames.ObjectTagSystem
{
    public class ObjectTagConditionalObjects : MonoBehaviour
    {
        protected enum Rule
        {
            Any,
            All
        }

        [SerializeField] private Rule m_rule; 
        [SerializeField] private ObjectTag[] m_tags = new ObjectTag[] {}; 
        [SerializeField] private MonoBehaviour[] m_components = new MonoBehaviour[] {};
        [SerializeField] private GameObject[] m_gameObjects = new GameObject[] {};
        [SerializeField] private ParticleSystem[] m_particleSystems = new  ParticleSystem[] {};

        private TaggedObject m_taggedObject;
        private bool m_state;

        private void Start()
        {
            m_taggedObject = GetComponentInParent<TaggedObject>();
            
            Assert.IsNotNull(m_taggedObject, "must have TaggedObject in parent");
            
            m_taggedObject.TagsChanged.AddListener(OnTagsChanged);
            
            UpdateState(true);
        }

        private void OnTagsChanged()
        {
            UpdateState();
        }

        private void LateUpdate()
        {
            UpdateState();
        }

        private void UpdateState(bool force = false)
        {
            var state = m_rule switch
            {
                Rule.Any => m_taggedObject.HasAny(m_tags),
                Rule.All => m_taggedObject.HasAll(m_tags),
                _ => throw new ArgumentOutOfRangeException()
            };

            if (state != m_state || force)
            {
                foreach (var go in m_gameObjects)
                {
                    go.SetActive(state);
                }

                foreach (var ps in m_particleSystems)
                {
                    if (state)
                    {
                        ps.Play();
                    }
                    else
                    {
                        ps.Stop();
                    }
                }

                m_state = state;
            }
        }
    }
}