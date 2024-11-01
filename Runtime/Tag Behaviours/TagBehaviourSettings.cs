using UnityEngine;
using UnityEngine.Pool;

namespace LowEndGames.ObjectTagSystem
{
    public abstract class TagBehaviourSettings : ScriptableObject
    {
        [SerializeField] private GameObject m_prefab;

        public ITagBehaviour Create(TagOwner owner)
        {
            var instance = m_pool.Get();
            instance.transform.gameObject.SetActive(true);
            instance.transform.SetParent(owner.GameObject.transform);
            instance.transform.localPosition = Vector3.zero;
            instance.transform.localRotation = Quaternion.identity;
            instance.Init(this, owner);
            return instance;
        }
        
        public void ReturnToPool(ITagBehaviour behaviour)
        {
            behaviour.transform.gameObject.SetActive(false);
            
            m_pool.Release(behaviour);
        }
        
        // -------------------------------------------------- private
        
        [RuntimeInitializeOnLoadMethod(RuntimeInitializeLoadType.BeforeSceneLoad)]
        private static void Init()
        {
            var allTagBehaviours = Resources.LoadAll<TagBehaviourSettings>("");

            foreach (var tagBehaviour in allTagBehaviours)
            {
                tagBehaviour.CreatePools();
            }
        }

        private ObjectPool<ITagBehaviour> m_pool;

        private void CreatePools()
        {
            m_pool = new ObjectPool<ITagBehaviour>(CreateFunc, OnGet, OnRelease);
        }
        
        private void OnRelease(ITagBehaviour behaviour)
        {
            behaviour.OnEnterPool();
        }

        private void OnGet(ITagBehaviour behaviour)
        {
            behaviour.OnLeavePool();
        }

        private ITagBehaviour CreateFunc()
        {
            return Instantiate(m_prefab).GetComponent<ITagBehaviour>();
        }
    }
}