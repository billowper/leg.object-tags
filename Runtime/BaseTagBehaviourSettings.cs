using UnityEngine;

namespace LowEndGames.ObjectTagSystem
{
    public abstract class BaseTagBehaviourSettings : ScriptableObject
    {
        [RuntimeInitializeOnLoadMethod(RuntimeInitializeLoadType.BeforeSceneLoad)]
        private static void Init()
        {
            var allTagBehaviours = Resources.LoadAll<BaseTagBehaviourSettings>("");

            foreach (var tagBehaviour in allTagBehaviours)
            {
                tagBehaviour.CreatePools();
            }
        }
        
        public abstract ITagBehaviour Create(TagOwner owner);
        public abstract void CreatePools();

        public virtual void DrawGizmos(Transform transform)
        {
            
        }
    }
}