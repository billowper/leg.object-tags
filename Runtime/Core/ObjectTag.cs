using System;
using System.Collections.Generic;
using UnityEngine;

namespace LowEndGames.ObjectTagSystem
{
    [Serializable]
    public struct ObjectTag : IEquatable<ObjectTag>, IComparable<ObjectTag>
    {
        private sealed class ValueEqualityComparer : IEqualityComparer<ObjectTag>
        {
            public bool Equals(ObjectTag x, ObjectTag y)
            {
                return x.m_value == y.m_value;
            }

            public int GetHashCode(ObjectTag obj)
            {
                return (obj.m_value != null ? obj.m_value.GetHashCode() : 0);
            }
        }

        public static IEqualityComparer<ObjectTag> ValueComparer { get; } = new ValueEqualityComparer();

        [SerializeField] private string m_value;

        private ObjectTag(string value)
        {
            m_value = value;
        }
        
        public static implicit operator string(ObjectTag tag) => tag.m_value;
        
        public static ObjectTag Create(string tag) => new ObjectTag(tag);

        public override string ToString() => m_value;

        public bool Equals(ObjectTag other)
        {
            return m_value == other.m_value;
        }

        public override bool Equals(object obj)
        {
            return obj is ObjectTag other && Equals(other);
        }

        public override int GetHashCode()
        {
            return m_value.GetHashCode();
        }

        public bool IsValid()
        {
            return !string.IsNullOrEmpty(m_value);
        }

        public int CompareTo(ObjectTag other)
        {
            return string.Compare(m_value, other.m_value, StringComparison.Ordinal);
        }
    }
}