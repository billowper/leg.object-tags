using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Pool;

namespace LowEndGames.ObjectTagSystem
{
    public class ForceTagsOnPhysicsEvents : MonoBehaviour
    {
        [SerializeField] private TagsFilter m_filter = new TagsFilter();
        [SerializeField] private List<ObjectTag> m_objectTags = new List<ObjectTag>();
        [SerializeField] private bool m_trigger = true;
        [SerializeField] private bool m_collision;

        // -------------------------------------------------- private
        
        private readonly Dictionary<ITagOwner, CancelToken> m_tokens = new Dictionary<ITagOwner, CancelToken>();

        private void OnCollisionEnter(Collision collision)
        {
            if (m_collision)
            {
                ProcessOtherColliderEnter(collision.collider);
            }
        }

        private void OnCollisionExit(Collision collision)
        {
            if (m_collision)
            {
                ProcessOtherColliderExit(collision.collider);
            }
        }

        private void OnTriggerEnter(Collider other)
        {
            if (m_trigger)
            {
                ProcessOtherColliderEnter(other);
            }
        }
        
        private void OnTriggerExit(Collider other)
        {
            if (m_trigger)
            {
                ProcessOtherColliderExit(other);
            }
        }
        
        private void ProcessOtherColliderEnter(Collider other)
        {
            Debug.Log($"ForceTagsOnPhysicsEvents:ProcessOtherColliderEnter {other.gameObject.name}");

            if (other.TryGetComponentInParent<ITagOwner>(out var tagOwner) && m_filter.Check(tagOwner))
            {
                GenericPool<CancelToken>.Get(out var cancelToken);
                cancelToken.Reset();
                tagOwner.ForceOnWhile(m_objectTags, cancelToken);
                m_tokens.Add(tagOwner, cancelToken);
            }
        }

        private void ProcessOtherColliderExit(Collider other)
        {
            Debug.Log($"ForceTagsOnPhysicsEvents:ProcessOtherColliderExit {other.gameObject.name}");
            
            if (other.TryGetComponentInParent<ITagOwner>(out var tagOwner) && m_tokens.TryGetValue(tagOwner, out var token))
            {
                token.Cancel();
                m_tokens.Remove(tagOwner);
                GenericPool<CancelToken>.Release(token);
            }
        }
    }
}