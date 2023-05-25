using System;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

namespace FSM
{
    public class CSMachine : ISMachine
    {
        private List<IState> _states;
        private bool _isActive;

        public CSMachine()
        {
            _states = new List<IState>();
        }

        public bool IsActive
        {
            get { return _isActive; }
        }

        public void AddState(IState state)
        {
            if (_isActive)
            {
                Debug.Log("state is running, cannot modify now");
                return;
            }
            
            if (_states == null) return;

            ((CState)state).SMachine = this;
            if (!_states.Exists((a) => { return a.Name == state.Name; }))
            {
                _states.Add(state);
            }
            else
            {
                throw new Exception("SM AddState Failed, State Already Exist");
            }
        }

        public void RemoveState(IState state)
        {
            if (_isActive)
            {
                Debug.Log("state is running, cannot modify now");
                return;
            }
            
            if (_states == null) return;
            IState tmp = _states.FirstOrDefault((a) => { return a.Name == state.Name; });
            if (tmp != null)
            {
                _states.Remove(state);
            }
            
        }

        public List<IState> States
        {
            get { return null; }
        }

        public void OnUpdate()
        {
            _isActive = true;
            
            //Debug.Log("SM Update:");
            foreach (var child in _states)
            {
                if (child.IsActive)
                {
                    child.OnUpdate();
                }
            }
            //Debug.Log("SM Finish:");
            _isActive = false;
        }
    }
}