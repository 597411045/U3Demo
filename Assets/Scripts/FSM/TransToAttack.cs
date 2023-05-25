using System;

namespace FSM
{
    public class TransToAttack : CTransition
    {
        public TransToAttack(string CName, CState toState, int priority) : base(CName, toState, priority)
        {
        }
    }
}