using System;
using UnityEngine.Assertions;

namespace LowEndGames.ObjectTagSystem
{
    public class CancelToken
    {
        public enum InitStates
        {
            Cancelled,
            Reset,
        }
        
        public event Action<CancelToken> Cancelled = (t) => { };
        
        public bool IsCancelled { get; private set; }
        
        public string Identifier { get; set; }

        public CancelToken(InitStates initState = InitStates.Reset)
        {
            Identifier = "Anonymous";
            IsCancelled = initState == InitStates.Cancelled;
        }
		
        public CancelToken(string identifier, InitStates initState = InitStates.Reset)
        {
            Identifier = identifier;
            IsCancelled = initState == InitStates.Cancelled;
        }

        public CancelToken()
        {
            IsCancelled = false;
        }

        public void Cancel()
        {
            Assert.IsFalse(IsCancelled, "should not already be cancelled");
            IsCancelled = true;
            Cancelled(this);
        }
		
        public void Reset()
        {
            IsCancelled = false;
            Cancelled = (t) => { };
        }

        public override string ToString()
        {
            return Identifier;
        }
    }
}