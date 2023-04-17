using UnityEngine;

namespace LowEndGames.ObjectTagSystem
{
    public abstract class TagBehaviour<TSettings> : ITagBehaviour
        where TSettings : ScriptableObject
    {
        public void Init(ScriptableObject settings, TagOwner owner)
        {
            m_owner = owner;
            m_settings = settings as TSettings;

            OnInit();
        }

        protected TSettings m_settings;
        protected TagOwner m_owner;
        protected float m_elapsed;
        protected Transform transform => m_owner.transform;

        public void Update()
        {
            m_elapsed += Time.deltaTime;
            
            OnUpdate();
        }

        public void FixedUpdate()
        {
            OnFixedUpdate();
        }
        
        public bool MatchesSettingsType(BaseTagBehaviourSettings tagBehaviourSettings)
        {
            return m_settings == tagBehaviourSettings;
        }

        protected virtual void OnInit(){}
        
        protected virtual void OnUpdate(){}
        
        protected virtual void OnFixedUpdate(){}
        
        protected virtual void OnDestroyed(){}
        
        public virtual void OnDrawGizmos(){}
        
        public void OnEnterPool()
        {
            OnDestroyed();
            
            m_settings = null;
            m_owner = null;
        }

        public void OnLeavePool()
        {
            m_elapsed = 0;
        }

        public IPool Pool { get; set; }
    }
}