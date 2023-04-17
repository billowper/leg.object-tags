using System.Collections.Generic;
using LowEndGames.Utils;
using Sirenix.OdinInspector;
using UnityEngine;

namespace LowEndGames.ObjectTagSystem
{
    [TypeInfoBox("Evaluates Global Interaction Rules via Collision and Trigger events. Tags are removed on Exit.")]
    public class ObjectTagsCollisions : MonoBehaviour
    {   
        [SerializeField] private TaggedObject m_objectTags;
        [SerializeField] private bool m_trigger = true;
        [SerializeField] private bool m_collision = true;

        private readonly List<TaggedObject> m_objectsAffected = new(32);
        private float m_tickTime;

        private void OnCollisionEnter(Collision collision)
        {
            if (m_collision)
            {
                OnEnter(collision.collider);
            }
        }

        private void OnTriggerEnter(Collider other)
        {
            if (m_trigger)
            {
                OnEnter(other);
            }
        }

        private void OnCollisionExit(Collision collision)
        {
            if (m_collision)
            {
                OnExit(collision.collider);
            }
        }

        private void OnTriggerExit(Collider other)
        {
            if (m_trigger)
            {
                OnExit(other);
            }
        }
        
        private void OnEnter(Collider other)
        {
            if (other.isTrigger)
            {
                return;
            }
            
            if (other.TryGetComponentFromCollider<TaggedObject>(out var target))
            {
                m_objectsAffected.Add(target);

                ApplyTagActionsFromRules(target);
            }
        }

        private void OnExit(Collider other)
        {
            if (other.isTrigger)
            {
                return;
            }
            
            if (other.TryGetComponentFromCollider<TaggedObject>(out var tagsComponent))
            {
                if (m_objectsAffected.Remove(tagsComponent))
                {
                    RemoveTagsFromRules(tagsComponent);
                }
            }
        }
        
        private void ApplyTagActionsFromRules(TaggedObject target)
        {
            foreach (var rule in ObjectTagsInteractionRule.Global)
            {
                if (rule.Filters.EvaluateFilters(m_objectTags))
                {
                    rule.Actions.ApplyTo(target);
                }
            }
        }

        private void RemoveTagsFromRules(TaggedObject tagsComponent)
        {
            foreach (var rule in ObjectTagsInteractionRule.Global)
            {
                foreach (var tagAction in rule.Actions)
                {
                    if (tagAction.Action is TagAction.TagActions.Add)
                    {
                        tagsComponent.RemoveTag(tagAction.Tag);
                    }
                }
            }
        }

        private void Update()
        {
            m_tickTime += Time.deltaTime;

            if (m_tickTime > 1)
            {
                m_tickTime = 0;

                for (var index = m_objectsAffected.Count - 1; index >= 0; index--)
                {
                    if (m_objectsAffected[index] == null)
                    {
                        m_objectsAffected.RemoveAt(index);
                    }
                }

                foreach (var tagsComponent in m_objectsAffected)
                {
                    ApplyTagActionsFromRules(tagsComponent);
                }
            }
        }
    }
}