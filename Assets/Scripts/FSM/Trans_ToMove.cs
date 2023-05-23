using System;

namespace FSM
{
    public class Trans_ToMove : CTransition
    {
        public static string CName = "Trans_ToMove";

        public Trans_ToMove() : base(CName, State_Move.CName)
        {
        }
    }
}