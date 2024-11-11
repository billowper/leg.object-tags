using UnityEngine;

namespace LowEndGames.ObjectTagSystem
{
    /// <summary>
    /// trigger that invokes UnityEvents when a tagged object enters or exits and passes the <see cref="TagsFilter"/>
    /// </summary>
    public class TriggerWithTagsFilter : MonoBehaviour
    {
        [SerializeField] private TagsFilter m_filter = new TagsFilter();
        [Tooltip("if true, ignore colliders which are flagged as triggers")]
        [SerializeField] private bool m_ignoreTriggers;
        [SerializeField] private ColliderEvent m_onEnter;
        [SerializeField] private ColliderEvent m_onExit;
    
        // -------------------------------------------------- public

        public ColliderEvent OnEnter => m_onEnter;
        public ColliderEvent OnExit => m_onExit;
        
        // -------------------------------------------------- private
        
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
            return other.TryGetComponent<ITagOwner>(out var tagOwner) && m_filter.Check(tagOwner);
        }
    }
}