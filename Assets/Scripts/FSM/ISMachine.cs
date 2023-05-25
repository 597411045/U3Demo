using System.Collections.Generic;

namespace FSM
{
    public interface ISMachine
    {
        bool IsActive { get; }
        
        void AddState(IState state);
        void RemoveState(IState state);

        List<IState> States { get; }
    }
}