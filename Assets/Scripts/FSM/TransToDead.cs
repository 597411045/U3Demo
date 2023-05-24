using System;

namespace FSM
{
    public class TransToDead : CTransition
    {
        public static string CName = "TransToDead";

        public TransToDead() : base(CName, StateDead.CName)
        {
        }
    }
}