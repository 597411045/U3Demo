using System;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

namespace FSM
{
    public class CSMachine : ISMachine
    {
        private Dictionary<string, IState> _states;

        public CSMachine()
        {
            _states = new Dictionary<string, IState>();
        }

        public IState AddState(IState state)
        {
            ((CState)state).SMachine = this;
            if (!_states.ContainsKey(state.Name))
            {
                _states.Add(state.Name, state);
            }
            else
            {
                throw new Exception("SM AddState Failed, State Already Exist");
            }

            return state;
        }

        public void RemoveState(IState state)
        {
            if (_states == null) return;
            if (_states.ContainsKey(state.Name)
                && _states[state.Name].IsActive == false)
            {
                _states.Remove(state.Name);
            }
        }

        public IState GetState(string name)
        {
            if (_states.ContainsKey(name))
            {
                return _states[name];
            }

            throw new Exception("No " + name + " in SM");
            return null;
        }

        public Dictionary<string, IState> States
        {
            get { return null; }
        }

        public void OnUpdate()
        {
            foreach (var child in _states)
            {
                if (child.Value.IsActive)
                {
                    child.Value.OnUpdate();
                }
            }
        }
    }
}