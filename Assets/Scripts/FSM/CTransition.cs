namespace FSM
{
    public abstract class CTransition : ITransition
    {
        private string _name;
        private IState _fromState;
        private IState _toState;

        public delegate bool DGT_RT_BOOL();

        public DGT_RT_BOOL Delegate_OnCheck;
        public DGT_RT_BOOL Delegate_OnCompleteCallBack;
            
        public bool OnCheck()
        {
            return Delegate_OnCheck.Invoke();
        }

        public bool OnCompleteCallBack()
        {
            return Delegate_OnCompleteCallBack.Invoke();
        }
        
        
        public string Name
        {
            get { return _name; }
        }
        public IState FromState
        {
            get { return _fromState; }
        }
        public IState ToState
        {
            get { return _toState; }
        }

        public CTransition(string name)
        {
            _name = name;
        }
        
        

    }
}