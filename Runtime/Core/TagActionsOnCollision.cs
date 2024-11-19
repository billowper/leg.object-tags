using System.Collections.Generic;
using UnityEngine;

namespace LowEndGames.ObjectTagSystem
{
    /// <summary>
    /// Executes a list of TagActions on collision events, with optional filter
    /// </summary>
    public class TagActionsOnCollision : MonoBehaviour
    {
        [SerializeField] private TagsFilter m_filter = new TagsFilter();
        [SerializeField] private List<TagAction> m_actionsOnEnter; 
        [SerializeField] private List<TagAction> m_actionsOnExit; 
        [SerializeField] private bool m_force; 

        // -------------------------------------------------- private

        private void OnCollisionEnter(Collision other)
        {
            if (other.collider.TryGetComponentInParent<ITagOwner>(out var tagOwner) && m_filter.Check(tagOwner)) 
            {
                m_actionsOnEnter.ApplyTo(tagOwner, m_force);
            }
        }

        private void OnCollisionExit(Collision other)
        {
            if (other.collider.TryGetComponentInParent<ITagOwner>(out var tagOwner) && m_filter.Check(tagOwner))
            {
                m_actionsOnExit.ApplyTo(tagOwner, m_force);
            }
        }
    }
}