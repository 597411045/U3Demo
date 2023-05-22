using System.Collections.Generic;
using UnityEngine;

namespace FSM
{
    public class CState : IState
    {
        protected string _name;
        protected string _tag;
        protected float _runningTime;
        protected bool _ifWorking;
        protected int _index;
        protected ISMachine _SMachine;
        protected Dictionary<string, ITransition> _transitions;
        protected ITransition _workingTransition;

        public string Name
        {
            get { return _name; }
        }

        public string Tag
        {
            get { return _tag; }
            set { _tag = value; }
        }

        public float RunningTime
        {
            get { return _runningTime; }
        }

        public Dictionary<string, ITransition> Transitions
        {
            get { return _transitions; }
        }

        public ISMachine SMachine
        {
            get { return _SMachine; }
        }

        public CState(string name)
        {
            _name = name;
            _transitions = new Dictionary<string, ITransition>();
        }


        public virtual void OnEnter()
        {
            _runningTime = 0;
            _index = 0;
            _ifWorking = true;
        }

        public virtual void OnExit()
        {
            _runningTime = 0;
            _index = 0;
            _ifWorking = false;
        }

        public virtual void OnUpdate(float delta)
        {
            if (!_ifWorking) return;
            _runningTime += delta;
            if (_transitions.Count < 1) return;

            if (_workingTransition != null && _workingTransition.OnCheck())
            {
                if (_workingTransition.OnCompleteCallBack())
                {
                    DoTransition(_workingTransition);
                }
            }
        }

        public void AddTransition(ITransition transition)
        {
            if (!_transitions.ContainsKey(transition.Name))
            {
                _transitions.Add(transition.Name,transition);
            }
        }

        public void SetStateMachine(ISMachine machine)
        {
            _SMachine = machine;
        }

        protected virtual void DoTransition(ITransition transition)
        {
           // _SMachine.CurState.OnExit(transition.ToState);
            //_SMachine.SetState(transition.ToState);
           // _SMachine.CurState.OnEnter(transition.FromState);
        }
    }
}