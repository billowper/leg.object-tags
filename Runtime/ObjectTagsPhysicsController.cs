using System;
using System.Collections.Generic;
using UnityEngine;

namespace LowEndGames.ObjectTagSystem
{
    public class ObjectTagsPhysicsController : MonoBehaviour
    {
        [SerializeField] private TaggedObject m_tagsComponent;
        [SerializeField] private List<PhysicsSettings> m_rules;
        [SerializeField] private Rigidbody m_rigidbody;
        [SerializeField] private Collider[] m_colliders;

        [Serializable]
        public class PhysicsSettings
        {
            public TagsFilter Filter;
            public PhysicMaterial PhysicMaterial;
            public bool IsKinematic;
            public bool ApplyGravity;
            public bool Freeze;
        }

        private bool m_defaultGravity;
        private bool m_defaultKinematic;
        private readonly Dictionary<Collider, PhysicMaterial> m_originalPhysicsMaterials = new Dictionary<Collider, PhysicMaterial>();

        private void Awake()
        {
            m_tagsComponent.TagsChanged.AddListener(OnTagsChanged);

            m_defaultGravity = m_rigidbody.useGravity;
            m_defaultKinematic = m_rigidbody.isKinematic;

            foreach (var col in m_colliders)
            {
                m_originalPhysicsMaterials.Add(col, col.sharedMaterial);
            }

            OnTagsChanged();
        }

        private void OnTagsChanged()
        {
            var appliedAnyRules = false;

            for (var index = m_rules.Count - 1; index >= 0; index--)
            {
                var rule = m_rules[index];
                if (rule.Filter.Check(m_tagsComponent))
                {
                    ApplyRule(rule);
                    appliedAnyRules = true;
                    break;
                }
            }

            if (appliedAnyRules == false)
            {
                RestoreDefaults();
            }
        }

        private void ApplyRule(PhysicsSettings rule)
        {
            foreach (var col in m_colliders)
            {
                col.sharedMaterial = rule.PhysicMaterial;
            }
            
            m_rigidbody.useGravity = rule.ApplyGravity;
            m_rigidbody.isKinematic = rule.IsKinematic;

            if (rule.Freeze)
            {
                m_rigidbody.velocity = Vector3.zero;
                m_rigidbody.angularVelocity = Vector3.zero;
            }
        }

        private void RestoreDefaults()
        {
            foreach (var col in m_colliders)
            {
                col.sharedMaterial = m_originalPhysicsMaterials[col];
            }

            m_rigidbody.useGravity = m_defaultGravity;
            m_rigidbody.isKinematic = m_defaultKinematic;
        }
    }
}