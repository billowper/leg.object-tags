using System;
using UnityEngine;
using UnityEngine.Events;

namespace LowEndGames.ObjectTagSystem
{
    [Serializable]
    public class ColliderEvent : UnityEvent<Collider> {}
}