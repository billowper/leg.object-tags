using System.Collections.Generic;
using UnityEngine;

namespace LowEndGames.ObjectTagSystem
{
    /// <summary>
    /// Executes a list of TagActions on objects that enter/exit a trigger
    /// </summary>
    public class TagActionsOnTrigger : MonoBehaviour
    {
        [SerializeField] private List<TagAction> m_actionsOnEnter; 
        [SerializeField] private List<TagAction> m_actionsOnExit; 

        // -------------------------------------------------- private
        
        private void OnTriggerEnter(Collider other)
        {
            if (other.TryGetComponentInParent<ITagOwner>(out var tagOwner))
            {
                m_actionsOnEnter.ApplyTo(tagOwner);
            }
        }

        private void OnTriggerExit(Collider other)
        {
            if (other.TryGetComponentInParent<ITagOwner>(out var tagOwner))
            {
                m_actionsOnExit.ApplyTo(tagOwner);
            }
        }
    }
}