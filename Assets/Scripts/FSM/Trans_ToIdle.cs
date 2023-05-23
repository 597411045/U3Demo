namespace FSM
{
    public class Trans_ToIdle :CTransition
    {
        private static string CName = "Trans_ToIdle";
        public Trans_ToIdle() : base(CName,State_Idle.CName)
        {
        }
    }
}