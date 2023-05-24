using System;

namespace FSM
{
    public class TransToMove : CTransition
    {
        public static string CName = "TransToMove";

        public TransToMove() : base(CName, StateMove.CName)
        {
        }
    }
}