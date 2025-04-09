using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.Events;

namespace LowEndGames.ObjectTagSystem
{
    /// <summary>
    /// an object that owns tags.
    /// </summary>
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
            if (Initialized)
                return;
            
            GameObject = gameObject;
            
            m_name = name;
            m_configuration = configuration;
            
            TagAdded = tagAdded;
            TagRemoved = tagRemoved;
            TagsChanged = tagsChanged;
            
            foreach (var tag in ObjectTagsLoader.Tags)
            {
                m_tagStates.Add(tag, new TagState(this, tag));
            }

            foreach (var rule in TagChangeRule.All)
            {
                m_ruleTimers.Add(rule, 0);
            }
            
            AddTags(configuration.DefaultTags, false);

            if (m_configuration.BlockTagChanges)
            {
                BlockChangesWhile(m_blockTagChangesWhile);
            }

            Initialized = true;
        }

        public bool Initialized { get; private set; }

        public GameObject GameObject { get; private set; }

        public IEnumerable<ObjectTag> GetActiveTags()
        {
            foreach (var pair in m_tagStates)
            {
                if (pair.Value.IsOn)
                {
                    yield return pair.Key;
                }
            }
        }
        
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
                m_blockTagChangesWhile.Reset();
                BlockChangesWhile(m_blockTagChangesWhile);
            }   
        }

        public bool HasTag(ObjectTag objectTag) => m_tagStates[objectTag].IsOn;

        /// <summary>
        /// Attempts to add a tag to the TagOwner, applying <see cref="ObjectTag.ActionsOnAdded"/>
        /// and invoking related events if successful.
        /// </summary>
        /// <param name="objectTag">the tag to add</param>
        /// <param name="runFilters">if true, run the tag's filters against this owner to make sure it is valid</param>
        /// <param name="force">if true, ignore anything that would prevent the tag being added</param>
        public bool AddTag(ObjectTag objectTag, bool runFilters = true, bool force = false)
        {
            if (!force && m_tagChangesBlocked.IsRequested)
            {
                Debug.Log($"{m_name}:AddTag - cannot add '{objectTag.name}', global tag changes blocked ({m_tagChangesBlocked.TokenIdentifiers}).");
                return false;
            }
            
            var state = m_tagStates[objectTag];
            if (state.IsOn)
            {
                return false;
            }
            
            if (!force && state.IsBlocked)
            {
                Debug.Log($"{m_name}:AddTag - cannot add '{objectTag.name}', changes blocked ({state.IsBlockedIdentifiers})");
                return false;
            }

            if (runFilters && !objectTag.Filters.EvaluateFilters(this))
            {
                return false;
            }

            objectTag.ActionsOnAdded.ApplyTo(this, force);

            foreach (var tagBehaviour in objectTag.Behaviours)
            {
                AddBehaviour(tagBehaviour);
            }
            
            state.SetState(true);
            
            TagAdded.Invoke(objectTag);
            TagsChanged.Invoke();
            
            AddTagsWhile(objectTag.ForcedTagsWhileActive, state.WhileActive);
            BlockTagsWhile(objectTag.BlockedTagsWhileActive, state.WhileActive);
            
            foreach (var changeRule in TagChangeRule.All)
            {
                if (changeRule.TimerResetWhenAdded.Contains(objectTag))
                {
                    m_ruleTimers[changeRule] = 0;
                }
            }
            
            Debug.Log($"{m_name}:AddTag '{objectTag.name}' added");
            
            return true;
        }
        
        /// <summary>
        /// Attempts to remove a tag from the TagOwner, applying <see cref="ObjectTag.ActionsOnRemoved"/>
        /// and invoking related events if successful.
        /// </summary>
        /// <param name="objectTag">the tag to remove</param>
        /// <param name="force">if true, ignore anything that would prevent the tag being removed</param>
        public bool RemoveTag(ObjectTag objectTag, bool force = false)
        {
            if (!force && m_tagChangesBlocked.IsRequested)
            {
                Debug.Log($"{m_name}:RemoveTag - cannot remove '{objectTag.name}', global tag changes blocked ({m_tagChangesBlocked.TokenIdentifiers}).");
                return false;
            }
            
            var state = m_tagStates[objectTag];

            if (state.IsOn == false)
            {
                return false;
            }

            if (!force && m_tagStates[objectTag].IsForcedOn)
            {
                Debug.Log($"{m_name}:RemoveTag - cannot remove '{objectTag.name}', ForcedOn = true.");
                return false;
            }
            
            if (state.IsOn)
            {
                foreach (var tagBehaviour in objectTag.Behaviours)
                {
                    RemoveBehaviour(tagBehaviour);
                }
                
                objectTag.ActionsOnRemoved.ApplyTo(this, force);

                TagRemoved.Invoke(objectTag);
                TagsChanged.Invoke();

                state.SetState(false);
                
                Debug.Log($"{m_name}:RemoveTag '{objectTag.name}' removed");

                foreach (var changeRule in TagChangeRule.All)
                {
                    if (changeRule.TimerResetWhenRemoved.Contains(objectTag))
                    {
                        m_ruleTimers[changeRule] = 0;
                    }
                }
                
                return true;
            }
            
            return false;
        }

        public void AddTags(IEnumerable<ObjectTag> tags, bool runFilters = true, bool force = false)
        {
            foreach (var objectTag in tags)
            {
                AddTag(objectTag, runFilters, force);
            }
        }
        
        public void RemoveTags(IEnumerable<ObjectTag> tags, bool force = false)
        {
            foreach (var objectTag in tags)
            {
                RemoveTag(objectTag, force);
            }
        }

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

        /// <summary>
        /// removes all tags
        /// </summary>
        public void ClearAll()
        {
            foreach (var pair in m_tagStates)
            {
                pair.Value.SetState(false);
            }
        }

        /// <summary>
        /// prevent all tag changes until the <see cref="CancelToken"/> is cancelled
        /// </summary>
        /// <param name="token"></param>
        public void BlockChangesWhile(CancelToken token)
        {
            m_tagChangesBlocked.RequestService(token);
        }
        
        /// <summary>
        /// add and prevent them from being removed until the <see cref="CancelToken"/> is cancelled
        /// </summary>
        /// <param name="tags"></param>
        /// <param name="cancelToken"></param>
        public void AddTagsWhile(IEnumerable<ObjectTag> tags, CancelToken cancelToken)
        {
            foreach (var tag in tags)
            {
                m_tagStates[tag].AddWhile(cancelToken);
            }
        }

        /// <summary>
        /// remove and prevent these tags from being added until the <see cref="CancelToken"/> is cancelled (ref counted, so all tokens must be cancelled)
        /// </summary>
        /// <param name="tags"></param>
        /// <param name="cancelToken"></param>
        public void BlockTagsWhile(IEnumerable<ObjectTag> tags, CancelToken cancelToken)
        {
            foreach (var tag in tags)
            {
                m_tagStates[tag].BlockWhile(cancelToken);
            }
        }
        
        public float GetTagTime(ObjectTag objectTag)
        {
            return m_tagStates[objectTag].ElapsedTime;
        }

        /// <summary>
        /// Updates rule timers and applies <see cref="TagChangeRule.Actions"/> when conditions are met.
        /// </summary>
        public void Update(float deltaTime)
        {
            foreach (var value in m_tagStates.Values)
            {
                if (value.IsOn)
                {
                    value.ElapsedTime += deltaTime;
                }
            }

            if (m_tagChangesBlocked.IsRequested)
            {
                return;
            }
            
            foreach (var rule in TagChangeRule.All)
            {
                if (rule.Filters.EvaluateFilters(this))
                {
                    var timeOverride = m_configuration.RuleTimeOverrides.Find(o => o.Rule == rule);
                    var requiredTime = timeOverride?.OverrideRequiredTime ?? rule.RequiredTime;
                   
                    m_ruleTimers[rule] += deltaTime;

                    if (m_ruleTimers[rule] > requiredTime)
                    {
                        rule.Actions.ApplyTo(this, false);
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
            UnityEditor.Handles.Label(transform.position + Vector3.up, new GUIContent(string.Join("\n", m_tagStates
                .Where(t => t.Value.IsOn)
                .Select(t => $"{t.Key.name.Split('.').Last()} - {t.Value.ElapsedTime:F2}"))), new GUIStyle("label") { wordWrap = false, richText = true, stretchWidth = true});
        }
#endif
        
        // -------------------------------------------------- private

        private string m_name;
        private TagOwnerConfiguration m_configuration;
        private readonly Dictionary<ObjectTag, TagState> m_tagStates = new(128);
        private readonly Dictionary<TagChangeRule, float> m_ruleTimers = new(128);
        private readonly List<ITagBehaviour> m_behaviours = new();
        private readonly CancelToken m_blockTagChangesWhile = new CancelToken(CancelToken.InitStates.Reset);
        private readonly TokenCounter m_tagChangesBlocked = new TokenCounter();

        private class TagState
        {
            public bool IsOn { get; private set; }
            public bool IsBlocked => m_blockedCounter.IsRequested;
            public bool IsForcedOn => m_addCounter.IsRequested;
            public float ElapsedTime { get; set; }
            public CancelToken WhileActive { get; }
            public string IsBlockedIdentifiers => m_blockedCounter.TokenIdentifiers;

            public TagState(TagOwner owner, ObjectTag tag)
            {
                m_tag = tag;
                m_owner = owner;
                
                WhileActive = new CancelToken($"{tag.EnumStringValue}");

                m_addCounter = new TokenCounter();
                m_addCounter.Released += OnAddCounterReleased;
                
                m_blockedCounter = new TokenCounter();
            }
            
            public void SetState(bool isOn)
            {
                IsOn = isOn;
                ElapsedTime = 0;

                if (isOn)
                {
                    WhileActive.Reset();
                }
                else
                {
                    WhileActive.Cancel();
                }
            }

            public void AddWhile(CancelToken cancelToken)
            {
                if (m_addCounter.RequestService(cancelToken))
                {
                    m_owner.AddTag(m_tag);
                }
            }

            public void BlockWhile(CancelToken cancelToken)
            {
                if (m_blockedCounter.RequestService(cancelToken))
                {
                    m_owner.RemoveTag(m_tag);
                }
            }

            private readonly ObjectTag m_tag;
            private readonly TagOwner m_owner;

            private TokenCounter m_addCounter;
            private TokenCounter m_blockedCounter;
            
            private void OnAddCounterReleased()
            {
                m_owner.RemoveTag(m_tag);
            }
        }
    }
}
