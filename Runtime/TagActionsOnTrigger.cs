using System.Collections.Generic;
using LowEndGames.Utils;
using Sirenix.OdinInspector;
using UnityEngine;

namespace LowEndGames.ObjectTagSystem
{
    [TypeInfoBox("Executes a list of TagActions on objects that enter/exit a trigger")]
    public class TagActionsOnTrigger : MonoBehaviour
    {
        [SerializeField] private List<TagAction> m_actionsOnEnter; 
        [SerializeField] private List<TagAction> m_actionsOnExit; 

        private void OnTriggerEnter(Collider other)
        {
            if (other.TryGetComponentFromCollider<ITagOwner>(out var tagOwner))
            {
                m_actionsOnEnter.ApplyTo(tagOwner);
            }
        }

        private void OnTriggerExit(Collider other)
        {
            if (other.TryGetComponentFromCollider<ITagOwner>(out var tagOwner))
            {
                m_actionsOnExit.ApplyTo(tagOwner);
            }
        }

    }
}