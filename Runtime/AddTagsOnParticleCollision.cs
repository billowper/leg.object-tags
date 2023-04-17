using System.Collections.Generic;
using LowEndGames.Utils;
using Sirenix.OdinInspector;
using UnityEngine;

namespace LowEndGames.ObjectTagSystem
{
    [TypeInfoBox("Executes a list of TagActions on objects that particles collide with")]
    public class AddTagsOnParticleCollision : MonoBehaviour
    {
        [SerializeField] private ParticleSystem m_particleSystem;
        [SerializeField] private List<TagAction> m_actions; 

        private readonly List<ParticleCollisionEvent> m_collisionEvents = new List<ParticleCollisionEvent>();

        private void Awake()
        {
            if (m_particleSystem == null)
            {
                m_particleSystem = GetComponent<ParticleSystem>();
            }
            
            if (m_particleSystem == null)
            {
                Debug.LogError("ElementalParticles has no particleSystem!", this);
                enabled = false;
            }
        }

        private void OnParticleCollision(GameObject other)
        {
            var count = m_particleSystem.GetCollisionEvents(other, m_collisionEvents);
            
            var i = 0;

            while (i < count)
            {
                if (m_collisionEvents[i].colliderComponent.TryGetComponentInParent<ITagOwner>(out var tagOwner))
                {
                    m_actions.ApplyTo(tagOwner);
                }     
                
                i++;
            }        
        }
    }
}