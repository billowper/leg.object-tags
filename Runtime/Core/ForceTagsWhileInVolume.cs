using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Pool;

namespace LowEndGames.ObjectTagSystem
{
    public class ForceTagsWhileInVolume : MonoBehaviour
    {
        [SerializeField] private TagsFilter m_filter = new TagsFilter();
        [SerializeField] private List<ObjectTag> m_objectTags = new List<ObjectTag>();

        // -------------------------------------------------- private
        
        private readonly Dictionary<ITagOwner, CancelToken> m_tokens = new Dictionary<ITagOwner, CancelToken>();
        
        private void OnTriggerEnter(Collider other)
        {
            if (other.TryGetComponentInParent<ITagOwner>(out var tagOwner) && m_filter.Check(tagOwner))
            {
                GenericPool<CancelToken>.Get(out var cancelToken);
                cancelToken.Reset();
                tagOwner.ForceTagsWhile(m_objectTags, cancelToken);
                m_tokens.Add(tagOwner, cancelToken);
            }
        }

        private void OnTriggerExit(Collider other)
        {
            if (other.TryGetComponentInParent<ITagOwner>(out var tagOwner) && m_tokens.TryGetValue(tagOwner, out var token))
            {
                token.Cancel();
                m_tokens.Remove(tagOwner);
                GenericPool<CancelToken>.Release(token);
            }
        }
    }
}