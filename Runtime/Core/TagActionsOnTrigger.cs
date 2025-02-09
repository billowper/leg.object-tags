﻿using System.Collections.Generic;
using UnityEngine;

namespace LowEndGames.ObjectTagSystem
{
    /// <summary>
    /// Executes a list of TagActions on objects that enter/exit a trigger, with optional filter
    /// </summary>
    public class TagActionsOnTrigger : MonoBehaviour
    {
        [SerializeField] private TagsFilter m_filter = new TagsFilter();
        [SerializeField] private List<TagAction> m_actionsOnEnter; 
        [SerializeField] private List<TagAction> m_actionsOnExit; 
        [SerializeField] private bool m_force; 

        // -------------------------------------------------- private
        
        private void OnTriggerEnter(Collider other)
        {
            if (other.TryGetComponentInParent<ITagOwner>(out var tagOwner) && m_filter.Check(tagOwner))
            {
                m_actionsOnEnter.ApplyTo(tagOwner, m_force);
            }
        }

        private void OnTriggerExit(Collider other)
        {
            if (other.TryGetComponentInParent<ITagOwner>(out var tagOwner) && m_filter.Check(tagOwner))
            {
                m_actionsOnExit.ApplyTo(tagOwner, m_force);
            }
        }
    }
}