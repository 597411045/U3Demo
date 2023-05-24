using System;

namespace FSM
{
    public class TransToAttack : CTransition
    {
        public static string CName = "TransToAttack";

        public TransToAttack() : base(CName, StateAttack.CName)
        {
        }
    }
}