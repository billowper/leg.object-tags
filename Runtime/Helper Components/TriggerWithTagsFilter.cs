using UnityEngine;

namespace LowEndGames.ObjectTagSystem
{
    public class TriggerWithTagsFilter : MonoBehaviour
    {
        [SerializeField] private TagsFilter m_filter = new TagsFilter();
        [SerializeField] private bool m_ignoreTriggers;
        [SerializeField] private ColliderEvent m_onEnter;
        [SerializeField] private ColliderEvent m_onExit;
    
        public ColliderEvent OnEnter => m_onEnter;
        public ColliderEvent OnExit => m_onExit;
        
        private void OnTriggerEnter(Collider other)
        {
            if (other.isTrigger && m_ignoreTriggers)
                return;
            
            if (IsValidCollider(other))
            {
                m_onEnter.Invoke(other);
            }
        }

        private void OnTriggerExit(Collider other)
        {
            if (other.isTrigger && m_ignoreTriggers)
                return;
            
            if (IsValidCollider(other))
            {
                m_onExit.Invoke(other);
            }
        }

        private bool IsValidCollider(Collider other)
        {
            return other.TryGetComponent<TaggedObject>(out var taggedObject) && m_filter.Check(taggedObject);
        }
    }
}