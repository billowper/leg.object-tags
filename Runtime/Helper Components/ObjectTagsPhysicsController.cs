using System;
using System.Collections.Generic;
using UnityEngine;

namespace LowEndGames.ObjectTagSystem
{
    /// <summary>
    /// applies various Physics changed when an object's tags pass the given filter
    /// </summary>
    [AddComponentMenu("ObjectTags System/ObjectTags Physics Controller")]
    public class ObjectTagsPhysicsController : MonoBehaviour
    {
        [SerializeField] private TaggedObject m_tagsComponent;
        [SerializeField] private List<PhysicsSettings> m_rules;
        [SerializeField] private Rigidbody m_rigidbody;
        [SerializeField] private Collider[] m_colliders;

        // -------------------------------------------------- private
        
        [Serializable]
        private class PhysicsSettings
        {
            public TagsFilter Filter;
            public PhysicsMaterial PhysicMaterial;
            public bool IsKinematic;
            public bool ApplyGravity;
            public bool Freeze;
        }

        private bool m_defaultGravity;
        private bool m_defaultKinematic;
        private readonly Dictionary<Collider, PhysicsMaterial> m_originalPhysicsMaterials = new Dictionary<Collider, PhysicsMaterial>();
        private PhysicsSettings m_lastActiveSettings;

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
            PhysicsSettings newActiveSettings = null;

            for (var index = m_rules.Count - 1; index >= 0; index--)
            {
                var rule = m_rules[index];
                if (rule.Filter.Check(m_tagsComponent))
                {
                    ApplyRule(rule);
                    newActiveSettings = rule;
                    break;
                }
            }

            var activeSettingsDisabled = m_lastActiveSettings != null && newActiveSettings == null;
            if (activeSettingsDisabled)
            {
                RestoreDefaults();
            }
            
            if (newActiveSettings != null && newActiveSettings != m_lastActiveSettings)
            {
                m_lastActiveSettings = newActiveSettings;
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
                m_rigidbody.linearVelocity = Vector3.zero;
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