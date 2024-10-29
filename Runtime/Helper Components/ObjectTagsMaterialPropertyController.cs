using System;
using System.Collections.Generic;
using UnityEngine;

namespace LowEndGames.ObjectTagSystem
{
    /// <summary>
    /// animates material float properties when given <see cref="ObjectTag"/> is set on an <see cref="TaggedObject"/>
    /// </summary>
    [AddComponentMenu("ObjectTags System/ObjectTags MaterialProperty Controller")]
    public class ObjectTagsMaterialPropertyController : MonoBehaviour
    {
        [SerializeField] private TaggedObject m_tagsComponent;
        [SerializeField] private List<TagPropertyMapping> m_propertyMappings = new();
        [SerializeField] private List<Renderer> m_renderers = new();
        
        // -------------------------------------------------- private
        
        [Serializable]
        private class TagPropertyMapping
        {
            public bool IsAnimating { get; set; }
            public float Value { get; set; }

            public ObjectTag Tag;
            public string PropertyName;
            public float TargetValue;
            public float Time = .5f;
            public AnimationCurve Curve = AnimationCurve.Linear(0,0,1,1);

            private float m_tweenTime;
            private float m_startValue;
            private float m_endValue;

            public void UpdateTween(float deltaTime)
            {
                m_tweenTime += deltaTime;
                
                Value = Mathf.Lerp(m_startValue, m_endValue, Curve.Evaluate(Mathf.Clamp01(m_tweenTime / Time)));

                if (m_tweenTime >= Time)
                {
                    Value = TargetValue;
                    IsAnimating = false;
                }
            }
            
            public void StartTween(float targetValue)
            {
                m_startValue = Value;
                m_endValue = targetValue;
                m_tweenTime = 0;

                IsAnimating = true;
            }
        }
        
        private MaterialPropertyBlock m_propertyBlock;

        private void Awake()
        {
            m_propertyBlock = new MaterialPropertyBlock();
            
            m_tagsComponent.TagAdded.AddListener(OnTagAdded);
            m_tagsComponent.TagRemoved.AddListener(OnTagRemoved);

            foreach (var mapping in m_propertyMappings)
            {
                SetMaterialValues(mapping, m_tagsComponent.HasTag(mapping.Tag) ? mapping.TargetValue : 0);
            }
        }

        private void Update()
        {
            foreach (var mapping in m_propertyMappings)
            {
                if (mapping.IsAnimating)
                {
                    mapping.UpdateTween(Time.deltaTime);
            
                    SetMaterialValues(mapping, mapping.Value);
                }
            }
        }
        
        private void OnTagAdded(ObjectTag tagAdded)
        {
            foreach (var mapping in m_propertyMappings)
            {
                if (mapping.Tag == tagAdded)
                {
                    mapping.StartTween(mapping.TargetValue);
                }
            }
        }
        
        private void OnTagRemoved(ObjectTag tagRemoved)
        {
            foreach (var mapping in m_propertyMappings)
            {
                if (mapping.Tag == tagRemoved)
                {
                    mapping.StartTween(0);
                }
            }
        }

        private void SetMaterialValues(TagPropertyMapping mapping, float value)
        {
            foreach (var r in m_renderers)
            {
                m_propertyBlock.Clear();
                
                for (int matIndex = 0; matIndex < r.sharedMaterials.Length; matIndex++)
                {
                    r.GetPropertyBlock(m_propertyBlock, matIndex);
                    m_propertyBlock.SetFloat(mapping.PropertyName, value);
                    r.SetPropertyBlock(m_propertyBlock, matIndex);
                }
            }
        }
    }
}