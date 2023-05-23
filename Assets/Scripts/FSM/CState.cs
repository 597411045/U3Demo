using System;
using System.Collections.Generic;
using UnityEngine;

namespace FSM
{
    public class CState : IState
    {
        protected string _name;
        protected string _tag;
        protected float _runningTime;
        protected bool _isActive;
        protected ISMachine _SMachine;
        protected Dictionary<string, ITransition> _transitions;
        public Action _stateAction;

        public string Name
        {
            get { return _name; }
        }

        public string Tag
        {
            get { return _tag; }
        }

        public float RunningTime
        {
            get { return _runningTime; }
        }

        public bool IsActive
        {
            get { return _isActive; }
        }

        public Dictionary<string, ITransition> Transitions
        {
            get { return null; }
        }

        public ISMachine SMachine
        {
            get { return _SMachine; }
            set { _SMachine = value; }
        }


        public CState(string name)
        {
            _name = name;
            _transitions = new Dictionary<string, ITransition>();
        }


        public virtual void OnEnter()
        {
            _runningTime = 0;
            _isActive = true;
        }

        public virtual void OnExit()
        {
            _runningTime = 0;
            _isActive = false;
        }

        private bool _willPass = false;

        public virtual void OnUpdate()
        {
            Debug.Log(Name + " OnUpdate");
            _runningTime += Time.deltaTime;
            Debug.Log(Name + " Doing Something");
            _stateAction?.Invoke();
            Debug.Log(Name + " Checking Transitions");

            _willPass = false;
            foreach (var child in _transitions)
            {
                if (child.Value.OnCheck(this))
                {
                    Debug.Log(child.Key + "Pass Checked True, Changing To " + child.Value.ToStateName);
                    _willPass = true;
                    break;
                }
            }

            if (!_willPass) Debug.Log("Pass Checked Failed, Will Remain " + Name);

            Debug.Log(Name + " OnUpdate Done");
        }

        public ITransition AddTransition(ITransition transition)
        {
            if (!_transitions.ContainsKey(transition.Name))
            {
                _transitions.Add(transition.Name, transition);
            }

            Debug.Log(Name + " AddTransition: " + transition.Name);
            return transition;
        }

        public void RemoveTransition(string name)
        {
            if (_transitions.ContainsKey(name))
            {
                _transitions.Remove(name);
            }

            Debug.Log(Name + " RemoveTransition: " + name);
        }
    }
}