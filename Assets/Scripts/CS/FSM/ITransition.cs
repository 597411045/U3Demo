using System;

namespace FSM
{
    public interface ITransition 
    {
        string Name { get; }
        IState ToState { get; }
        bool OnCheck(IState fromState);
        public int Priority { get; }

    }
}