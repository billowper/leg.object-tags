using UnityEngine.Assertions;

namespace LowEndGames.ObjectTagSystem
{
    public class TokenCounter
    {
        public int Count { get; private set; }
        public bool IsRequested => Count > 0;

        public event System.Action Released;

        public bool RequestService(CancelToken token)
        {
            Assert.IsNotNull(token, "expected valid token");
            Assert.IsFalse(token.IsCancelled, "token should be reset before requesting");
            Count++;
            token.Cancelled += OnTokenCancelled;
            return Count == 1;
        }

        private void OnTokenCancelled()
        {
            Count--;
            if (Count == 0 && Released != null)
            {
                Released();
            }
        }
    }
}