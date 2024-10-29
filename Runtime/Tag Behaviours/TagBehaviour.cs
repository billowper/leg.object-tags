using UnityEngine;

namespace LowEndGames.ObjectTagSystem
{
    public abstract class TagBehaviour<TSettings> : MonoBehaviour, ITagBehaviour
        where TSettings : TagBehaviourSettings
    {
        public void Init(TagBehaviourSettings settings, TagOwner owner)
        {
            m_owner = owner;
            m_settings = settings as TSettings;

            OnInit();
        }
        
        public TagBehaviourSettings Settings => m_settings;
        
        protected TSettings m_settings;
        protected TagOwner m_owner;
        
        public bool MatchesSettingsType(TagBehaviourSettings tagBehaviourSettings)
        {
            return m_settings == tagBehaviourSettings;
        }

        protected virtual void OnInit(){}
        
        public void OnEnterPool()
        {
            m_settings = null;
            m_owner = null;
        }

        public void OnLeavePool()
        {
        }
    }
}