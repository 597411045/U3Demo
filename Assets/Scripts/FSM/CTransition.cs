namespace FSM
{
    public abstract class CTransition : ITransition
    {
        protected string _name;
        protected string _toStateName;

        public bool ifReverse;
        public delegate bool DGT_RT_BOOL();

        public DGT_RT_BOOL Delegate_OnCheck;


        public string ToStateName
        {
            get { return _toStateName; }
        }

        private bool flag;
        public bool OnCheck(IState fromState)
        {
            flag = Delegate_OnCheck.Invoke();
            if (ifReverse) flag = !flag;
            if (flag)
            {
                fromState.OnExit();
                fromState.SMachine.GetState(_toStateName).OnEnter();
                return true;
            }

            return false;
        }

        public string Name
        {
            get { return _name; }
        }

        public CTransition(string name, string toStateName)
        {
            _name = name;
            _toStateName = toStateName;
        }
    }
}