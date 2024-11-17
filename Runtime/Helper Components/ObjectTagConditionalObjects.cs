using UnityEngine;
using UnityEngine.Assertions;

namespace LowEndGames.ObjectTagSystem
{
    
    
    /// <summary>
    /// toggles the state of objects, components and particle systems based on the state of a TagFilter evaluated 
    /// </summary>
    [AddComponentMenu("ObjectTags System/ObjectTags Conditional Objects")]
    public class ObjectTagConditionalObjects : MonoBehaviour
    {
        [Tooltip("TagsFilter will be evaluated for this object. Required, auto-populated in OnValidate.")]
        [SerializeField] private TaggedObject m_taggedObject; 
        [SerializeField] private TagsFilter m_filter = new(); 
        [SerializeField] private Behaviour[] m_components = new Behaviour[] {};
        [SerializeField] private GameObject[] m_gameObjects = new GameObject[] {};
        [SerializeField] private ParticleSystem[] m_particleSystems = new  ParticleSystem[] {};
        [SerializeField] private Renderer[] m_renderers = new  Renderer[] {};
        [SerializeField] private BoolEvent m_onStateChanged = new BoolEvent();

        // -------------------------------------------------- private
        
        private bool m_state;
        private float m_lastUpdateTime;
        
        private void Start()
        {
            Assert.IsNotNull(m_taggedObject, "must have a TaggedObject target");
            
            m_taggedObject.TagsChanged.AddListener(OnTagsChanged);
            
            UpdateState(true);
        }

        private void OnValidate()
        {
            if (m_taggedObject == null)
            {
                m_taggedObject = GetComponentInParent<TaggedObject>();
            }
        }

        private void OnTagsChanged()
        {
            UpdateState();
        }

        private void UpdateState(bool force = false)
        {
            var state = m_filter.Check(m_taggedObject);

            if (state != m_state || force)
            {
                foreach (var c in m_components)
                {
                    c.enabled = state;
                }
                
                foreach (var r in m_renderers)
                {
                    r.enabled = state;
                }
                
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
                m_onStateChanged.Invoke(state);
            }
        }
    }
}