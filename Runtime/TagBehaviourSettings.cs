using System;

namespace LowEndGames.ObjectTagSystem
{
    public abstract class TagBehaviourSettings<T> : BaseTagBehaviourSettings
        where T : class, ITagBehaviour
    {
        public override ITagBehaviour Create(TagOwner owner)
        {
            var instance = m_pool.TakeFromPool() as ITagBehaviour;
            instance.Init(this, owner);
            return instance;
        }
        
        public override void CreatePools()
        {
            m_pool = new ObjectPool<T>(32, CreateFunc);
        }

        private static T CreateFunc()
        {
            return Activator.CreateInstance<T>();
        }

        private ObjectPool<T> m_pool;
    }
}