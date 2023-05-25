using System;
using System.Collections.Generic;
using UnityEngine;

namespace FSM
{
    public abstract class CTransition : ITransition
    {
        protected string _name;
        protected CState _toState;
        protected int _priority;

        public delegate bool DGT_RT_BOOL();


        public DGT_RT_BOOL Delegate_OnCheck;


        public IState ToState
        {
            get { return _toState; }
        }

        private bool flag;

        public bool OnCheck(IState fromState)
        {
            //Debug.Log($"{Name} Update");
            if (Delegate_OnCheck.Invoke())
            {
                //Debug.Log($"{Name} Checked true");
                if (_toState != null)
                {
                    //Debug.Log($"{Name} has toState, state will to {_toState.Name}");

                    fromState.OnExit();
                    _toState.OnEnter();
                }
                else
                {
                    //Debug.Log($"{Name} has no toState, state will remain {fromState.Name}");
                }
                return true;
            }
            else
            {
                //Debug.Log($"{Name} Checked false");

                return false;
            }
        }

        public int Priority
        {
            get { return _priority; }
        }

        public string Name
        {
            get { return _name; }
        }

        public CTransition(string name, CState toState, int priority)
        {
            _name = name;
            _toState = toState;
            _priority = priority;
        }
    }
}