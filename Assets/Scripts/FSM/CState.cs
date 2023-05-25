using System;
using System.Collections.Generic;
using System.Linq;
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
        protected List<ITransition> _transitions;
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

        public List<ITransition> Transitions
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
            _transitions = new List<ITransition>();
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

        public virtual void OnUpdate()
        {
            _runningTime += Time.deltaTime;
            //Debug.Log($"{Name} Update:");
            _stateAction?.Invoke();


            foreach (var child in _transitions)
            {
                if (child.OnCheck(this))
                {
                    if (child.ToState == null)
                    {
                        //Debug.Log($"Pass True But not ToState, stop check queue");
                        break;
                    }
                }
            }

            //Debug.Log($"{Name} Finish:");
        }

        private void SortPriority()
        {
            _transitions.Sort((x, y) => { return x.Priority > y.Priority ? 1 : -1; });
        }

        public void AddTransition(ITransition transition)
        {
            if (_isActive)
            {
                //Debug.Log("state is running, cannot modify now");
                return;
            }

            if (!_transitions.Exists((a) => { return a.Name == transition.Name; }))
            {
                _transitions.Add(transition);
                SortPriority();
                //Debug.Log(Name + " AddTransition: " + transition.Name);
            }
        }

        public void RemoveTransition(ITransition transition)
        {
            if (_isActive)
            {
                //Debug.Log("state is running, cannot modify now");
                return;
            }

            //Debug.Log(Name + " Removing Transition: " + transition.Name);

            ITransition tmp = _transitions.FirstOrDefault((a) => { return a.Name == transition.Name; });
            if (tmp != null)
            {
                _transitions.Remove(transition);
            }
        }
    }
}