namespace FSM
{
    public class TransToIdle :CTransition
    {
      
        public TransToIdle(string CName, CState toState,int priority) : base(CName,toState,priority)
        {
        }
    }
}