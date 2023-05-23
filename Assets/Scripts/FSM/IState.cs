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
        Dictionary<string, ITransition> Transitions { get; }
        ISMachine SMachine { get; }

        void OnEnter();
        void OnExit();
        void OnUpdate();
        ITransition AddTransition(ITransition transition);
        void RemoveTransition(string name);
    }
}