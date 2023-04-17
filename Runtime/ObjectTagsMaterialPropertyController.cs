using System;
using System.Collections.Generic;
using DG.Tweening;
using DG.Tweening.Core;
using DG.Tweening.Plugins.Options;
using UnityEngine;

namespace LowEndGames.ObjectTagSystem
{
    /// <summary>
    /// animates material float properties when given <see cref="ObjectTag"/> is set on an <see cref="TaggedObject"/>
    /// </summary>
    public class ObjectTagsMaterialPropertyController : MonoBehaviour
    {
        [SerializeField] private TaggedObject m_tagsComponent;
        [SerializeField] private List<TagPropertyMapping> m_propertyMappings;
        [SerializeField] private Renderer[] m_renderers;

        [Serializable]
        public class TagPropertyMapping
        {
            public bool IsAnimating { get; set; }
            public float AnimatedValue { get; set; }

            public ObjectTag Tag;
            public string PropertyName;
            public float Value;
            public float AnimationTime = .5f;
            
            private TweenerCore<float,float,FloatOptions> m_tween;

            public void Animate(float targetValue)
            {
                var initialValue = AnimatedValue;

                m_tween?.Kill();
                m_tween = null;

                m_tween = DOTween.To(() => initialValue, value => AnimatedValue = value, targetValue, AnimationTime)
                    .OnComplete(() =>
                    {
                        IsAnimating = false;
                        m_tween = null;
                    });

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
                UpdateMaterialProperty(mapping.PropertyName, m_tagsComponent.HasTag(mapping.Tag) ? mapping.Value : 0);
            }
        }

        private void OnTagAdded(ObjectTag tagAdded)
        {
            foreach (var mapping in m_propertyMappings)
            {
                if (mapping.Tag == tagAdded)
                {
                    mapping.Animate(mapping.Value);
                }
            }
        }
        private void OnTagRemoved(ObjectTag tagRemoved)
        {
            foreach (var mapping in m_propertyMappings)
            {
                if (mapping.Tag == tagRemoved)
                {
                    mapping.Animate(0);
                }
            }
        }

        private void Update()
        {
            foreach (var mappping in m_propertyMappings)
            {
                if (mappping.IsAnimating)
                {
                    UpdateMaterialProperty(mappping.PropertyName, mappping.AnimatedValue);
                }
            }
        }

        private void UpdateMaterialProperty(string propertyName, float value)
        {
            foreach (var r in m_renderers)
            {
                m_propertyBlock.Clear();
                
                for (int matIndex = 0; matIndex < r.sharedMaterials.Length; matIndex++)
                {
                    r.GetPropertyBlock(m_propertyBlock, matIndex);
                    m_propertyBlock.SetFloat(propertyName, value);
                    r.SetPropertyBlock(m_propertyBlock, matIndex);
                }
            }
        }
    }
}