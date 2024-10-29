using System;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.Events;

namespace LowEndGames.ObjectTagSystem
{
    public class TagOwner : ITagOwner
    {
        // -------------------------------------------------- public

        public TagOwner(TagOwnerConfiguration configuration,
            string name,
            Transform transform,
            ObjectTagEvent tagAdded,
            ObjectTagEvent tagRemoved,
            UnityEvent tagsChanged)
        {
            this.transform = transform;

            m_name = name;
            m_configuration = configuration;
            
            TagAdded = tagAdded;
            TagRemoved = tagRemoved;
            TagsChanged = tagsChanged;
            
            AddTags(configuration.DefaultTags, false);

            if (m_configuration.BlockTagChanges)
            {
                m_tagChangesBlocked.RequestService(new CancelToken());
            }

            foreach (var rule in ObjectTagsInteractionRule.All)
            {
                m_ruleTimers.Add(rule, 0);
            }
        }

        public ObjectTagEvent TagAdded { get; }
        public ObjectTagEvent TagRemoved { get; }
        public UnityEvent TagsChanged { get; }
        
        public IEnumerable<ObjectTag> Tags => m_tags;
        public Transform transform { get; }

        public void ApplyConfig(TagOwnerConfiguration configuration)
        {
            m_configuration = configuration;
            
            AddTags(configuration.DefaultTags, false);

            if (m_configuration.BlockTagChanges)
            {
                m_tagChangesBlocked.RequestService(new CancelToken());
            }
        }

        public bool HasTag(ObjectTag objectTag)
        {
            return m_tags.Contains(objectTag);
        }

        public bool AddTag(ObjectTag objectTag, bool runFilters = true)
        {
            if (m_tagChangesBlocked.IsRequested)
            {
                return false;
            }
            
            if (HasTag(objectTag))
            {
                return false;
            }

            if (runFilters)
            {
                if (objectTag.Filters.EvaluateFilters(this) == false)
                {
                    return false;
                }
            }

            objectTag.ActionsOnAdded.ApplyTo(this);

            foreach (var tagBehaviour in objectTag.Behaviours)
            {
                AddBehaviour(tagBehaviour);
            }
            
            m_tags.Add(objectTag);
            TagAdded.Invoke(objectTag);
            TagsChanged.Invoke();
            
            Debug.Log($"{m_name}:Tag Added '{objectTag.name}'");
            
            return true;
        }
        
        public bool RemoveTag(ObjectTag objectTag)
        {
            if (m_configuration.BlockTagChanges)
            {
                return false;
            }
            
            if (m_tags.Remove(objectTag))
            {
                foreach (var tagBehaviour in objectTag.Behaviours)
                {
                    RemoveBehaviour(tagBehaviour);
                }

                TagRemoved.Invoke(objectTag);
                TagsChanged.Invoke();
                
                Debug.Log($"{m_name}:Tag Removed '{objectTag.name}'");
                return true;
            }
            
            return false;
        }

        public void AddTags(IEnumerable<ObjectTag> tags, bool runFilters = true)
        {
            foreach (var objectTag in tags)
            {
                AddTag(objectTag, runFilters);
            }
        }
        
        public void RemoveTags(params ObjectTag[] tags)
        {
            foreach (var objectTag in tags)
            {
                RemoveTag(objectTag);
            }
        }

        public bool HasAny(params ObjectTag[] tags)
        {
            foreach (var objectTag in tags)
            {
                if (HasTag(objectTag))
                {
                    return true;
                }
            }

            return false;
        }
        
        public bool HasAll(params ObjectTag[] tags)
        {
            foreach (var objectTag in tags)
            {
                if (HasTag(objectTag) == false)
                {
                    return false;
                }
            }

            return true;
        }
        
        public bool HasAny(params Enum[] tags)
        {
            foreach (var objectTag in tags)
            {
                if (HasTag(objectTag))
                {
                    return true;
                }
            }

            return false;
        }
        
        public bool HasAll(params Enum[] tags)
        {
            foreach (var objectTag in tags)
            {
                if (HasTag(objectTag) == false)
                {
                    return false;
                }
            }

            return true;
        }
        
        public void AddTag(Enum enumValue, bool runFilters = true) => AddTag(enumValue.ToAsset(), runFilters);
        
        public bool HasTag(Enum enumValue) => HasTag(enumValue.ToAsset());
        
        public bool RemoveTag(Enum enumValue) => RemoveTag(enumValue.ToAsset());
        
        public void AddBehaviour(TagBehaviourSettings tagBehaviourSettings)
        {
            m_behaviours.Add(tagBehaviourSettings.Create(this));
        }

        public void RemoveBehaviour(TagBehaviourSettings tagBehaviourSettings)
        {
            foreach (var behaviour in m_behaviours)
            {
                if (behaviour.MatchesSettingsType(tagBehaviourSettings))
                {
                    behaviour.Settings.ReturnToPool(behaviour);
                }
            }
        }
        
        public void ClearAll()
        {
            m_tags.Clear();
        }

        public bool BlockChangesWhile(CancelToken token)
        {
            return m_tagChangesBlocked.RequestService(token);
        }

        public void Update()
        {
            foreach (var rule in ObjectTagsInteractionRule.Self)
            {
                if (rule.Filters.EvaluateFilters(this))
                {
                    var timeOverride = m_configuration.RuleTimeOverrides.Find(o => o.Rule == rule);
                    var requiredTime = timeOverride?.OverrideRequiredTime ?? rule.RequiredTime;
                   
                    m_ruleTimers[rule] += Time.deltaTime;

                    if (m_ruleTimers[rule] > requiredTime)
                    {
                        rule.Actions.ApplyTo(this);
                    }
                }
                else
                {
                    m_ruleTimers[rule] = 0;
                }
            }
        }

#if UNITY_EDITOR
        public void OnDrawGizmos()
        {
            UnityEditor.Handles.Label(transform.position + Vector3.up, new GUIContent(string.Join("\n", m_tags.Select(t => t.name.Split('.').Last()))));
        }
#endif
        
        // -------------------------------------------------- private

        private readonly string m_name;
        private TagOwnerConfiguration m_configuration;
        private readonly HashSet<ObjectTag> m_tags = new(32);
        private readonly Dictionary<ObjectTagsInteractionRule, float> m_ruleTimers = new(128);
        private readonly List<ITagBehaviour> m_behaviours = new();
        private readonly TokenCounter m_tagChangesBlocked = new TokenCounter();

    }
}