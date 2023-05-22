using System.Collections.Generic;

namespace FSM
{
    public interface ISMachine
    {
        IState DefaultState { get; }
        IState CurState { get; }
        void AddState(IState state);
        void RemoveState(IState state);
        void SetState(IState state);
        IState GetStateWithTag(string tag);
        IState GetStateWithName(string name);
        Dictionary<string, IState> States { get; }
    }
}