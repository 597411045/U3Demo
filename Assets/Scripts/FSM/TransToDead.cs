using System;

namespace FSM
{
    public class TransToDead : CTransition
    {
      

        public TransToDead(string CName, CState toState,int priority) : base(CName, toState, priority)
        {
        }
    }
}