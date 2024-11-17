using System.Collections.Generic;
using UnityEngine.Assertions;

namespace LowEndGames.ObjectTagSystem
{
    public class TokenCounter
    {
        public int Count { get; private set; }
        public bool IsRequested => Count > 0;
        public string TokenIdentifiers => string.Join(", ", m_tokenIdentifiers);

        public event System.Action Released;
        
        public bool RequestService(CancelToken token)
        {
            Assert.IsNotNull(token, "expected valid token");
            Assert.IsFalse(token.IsCancelled, "token should be reset before requesting");
            Count++;
            token.Cancelled += OnTokenCancelled;
            m_tokenIdentifiers.Add(token.Identifier);
            return Count == 1;
        }
        
        private readonly List<string> m_tokenIdentifiers = new List<string>(32);

        private void OnTokenCancelled(CancelToken token)
        {
            m_tokenIdentifiers.Remove(token.Identifier);

            Count--;
            if (Count == 0 && Released != null)
            {
                Released();
            }
        }
    }
}