using UnityEngine;

namespace LowEndGames.ObjectTagSystem
{
    public interface ITagBehaviour : IPooledObject, IPoolReference
    {
        void Init(ScriptableObject settings, TagOwner owner);
        void Update();
        void FixedUpdate();
        bool MatchesSettingsType(BaseTagBehaviourSettings tagBehaviourSettings);
        void OnDrawGizmos();
    }
}