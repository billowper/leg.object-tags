using UnityEngine;

namespace LowEndGames.ObjectTagSystem
{
    public interface ITagBehaviour
    {
        Transform transform { get; }
        TagBehaviourSettings Settings { get; }
        void Init(TagBehaviourSettings settings, TagOwner owner);
        bool MatchesSettingsType(TagBehaviourSettings tagBehaviourSettings);
        void OnEnterPool();
        void OnLeavePool();
    }
}