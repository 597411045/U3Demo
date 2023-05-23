using System.Collections.Generic;

namespace FSM
{
    public interface ISMachine
    {
        IState AddState(IState state);
        void RemoveState(IState state);

        IState GetState(string name);
        Dictionary<string, IState> States { get; }
    }
}