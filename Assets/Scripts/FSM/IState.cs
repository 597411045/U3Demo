using System.Collections.Generic;

namespace FSM
{
    public interface IState
    {
        string Name { get; }
        string Tag { get; }
        float RunningTime { get; }
        Dictionary<string, ITransition> Transitions { get; }
        ISMachine SMachine { get; }

        void OnEnter();
        void OnExit();
        void OnUpdate(float delta);
        void AddTransition(ITransition transition);
        void SetStateMachine(ISMachine machine);
    }
}