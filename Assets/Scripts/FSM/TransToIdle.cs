namespace FSM
{
    public class TransToIdle :CTransition
    {
        private static string CName = "TransToIdle";
        public TransToIdle() : base(CName,StateIdle.CName)
        {
        }
    }
}