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
        
        /// <summary>
        /// Initializes the TagOwner with a configuration, name, GameObject, and events for tag changes.
        /// </summary>
        public void Init(TagOwnerConfiguration configuration,
            string name,
            GameObject gameObject,
            ObjectTagEvent tagAdded,
            ObjectTagEvent tagRemoved,
            UnityEvent tagsChanged)
        {
            GameObject = gameObject;
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
        
        public GameObject GameObject { get; private set; }

        public IEnumerable<ObjectTag> Tags => m_tags;
        
        /// <summary>
        /// Event triggered when a new tag is successfully added to the TagOwner.
        /// </summary>
        public ObjectTagEvent TagAdded { get; private set; }

        /// <summary>
        /// Event triggered when a tag is successfully removed from the TagOwner.
        /// </summary>
        public ObjectTagEvent TagRemoved { get; private set; }

        /// <summary>
        /// Event triggered whenever the set of tags on the TagOwner changes.
        /// </summary>
        public UnityEvent TagsChanged { get; private set; }

        public void ApplyConfig(TagOwnerConfiguration configuration)
        {
            m_configuration = configuration;
            
            AddTags(configuration.DefaultTags, false);

            if (m_configuration.BlockTagChanges)
            {
                m_tagChangesBlocked.RequestService(new CancelToken());
            }
        }

        public bool HasTag(ObjectTag objectTag) => m_tags.Contains(objectTag);

        /// <summary>
        /// Attempts to add a tag to the TagOwner, applying <see cref="ObjectTag.ActionsOnAdded"/>
        /// and invoking related events if successful.
        /// </summary>
        public bool AddTag(ObjectTag objectTag, bool runFilters = true)
        {
            if (m_tagChangesBlocked.IsRequested || HasTag(objectTag))
            {
                return false;
            }

            if (runFilters && !objectTag.Filters.EvaluateFilters(this))
            {
                return false;
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
        
        /// <summary>
        /// Attempts to remove a tag from the TagOwner, applying <see cref="ObjectTag.ActionsOnRemoved"/>
        /// and invoking related events if successful.
        /// </summary>
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
                
                objectTag.ActionsOnRemoved.ApplyTo(this);

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

        public bool HasAny(params ObjectTag[] tags) => tags.Any(HasTag);
        
        public bool HasAll(params ObjectTag[] tags) => tags.All(HasTag);

        public bool HasAny(params Enum[] tags) => tags.Any(tag => HasTag(tag.ToAsset()));
        
        public bool HasAll(params Enum[] tags) => tags.All(tag => HasTag(tag.ToAsset()));

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
        
        public void ClearAll() => m_tags.Clear();

        public bool BlockChangesWhile(CancelToken token) => m_tagChangesBlocked.RequestService(token);

        /// <summary>
        /// Updates rule timers and applies <see cref="ObjectTagsInteractionRule.Actions"/> when conditions are met.
        /// </summary>
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
        public void OnDrawGizmos(Transform transform)
        {
            UnityEditor.Handles.Label(transform.position + Vector3.up, new GUIContent(string.Join("\n", m_tags.Select(t => t.name.Split('.').Last()))), new GUIStyle("label") { wordWrap = false, richText = true, stretchWidth = true});
        }
#endif
        
        // -------------------------------------------------- private

        private string m_name;
        private TagOwnerConfiguration m_configuration;
        private readonly HashSet<ObjectTag> m_tags = new(32);
        private readonly Dictionary<ObjectTagsInteractionRule, float> m_ruleTimers = new(128);
        private readonly List<ITagBehaviour> m_behaviours = new();
        private readonly TokenCounter m_tagChangesBlocked = new TokenCounter();
    }
}
