using System;

namespace FSM
{
    public class TransToMove : CTransition
    {
        

        public TransToMove(string CName, CState toState,int priority) : base(CName, toState, priority)
        {
        }
    }
}