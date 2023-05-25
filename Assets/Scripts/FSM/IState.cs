using System;
using System.Collections.Generic;

namespace FSM
{
    public interface IState
    {
        string Name { get; }
        string Tag { get; }
        float RunningTime { get; }
        bool IsActive { get; }
        List<ITransition> Transitions { get; }
        ISMachine SMachine { get; }

        void OnEnter();
        void OnExit();
        void OnUpdate();
        void AddTransition(ITransition transition);
        void RemoveTransition(ITransition transition);
    }
}