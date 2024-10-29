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
        
        public event Action Cancelled = () => { };
        
        public bool IsCancelled { get; private set; }
        
        public string Identifier { get; private set; }

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
		
        public void Cancel()
        {
            Assert.IsFalse(IsCancelled, "should not already be cancelled");
            IsCancelled = true;
            Cancelled();
        }
		
        public void Reset()
        {
            IsCancelled = false;
            Cancelled = () => { };
        }

        public override string ToString()
        {
            return Identifier;
        }
    }
}