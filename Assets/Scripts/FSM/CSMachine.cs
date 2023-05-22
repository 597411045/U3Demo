using System.Collections.Generic;
using System.Linq;

namespace FSM
{
    public class CSMachine : ISMachine
    {
        private IState _curState;
        private IState _defaultState;
        private Dictionary<string, IState> _states;

        public CSMachine(IState defaultState)
        {
            _states = new Dictionary<string, IState>();
            AddState(defaultState);
            _defaultState = defaultState;
            _curState = defaultState;
            _defaultState.OnEnter();
        }

        public IState DefaultState
        {
            get { return _defaultState; }
        }

        public IState CurState
        {
            get { return _curState; }
        }

        public void AddState(IState state)
        {
            if (!_states.ContainsKey(state.Name))
            {
                _states.Add(state.Name, state);
                state.SetStateMachine(this);
            }
        }

        public void RemoveState(IState state)
        {
            if (_states == null) return;
            if (_states.ContainsKey(state.Name)
                && _curState.Name != state.Name)
            {
                _states.Remove(state.Name);
            }
        }

        public void SetState(IState state)
        {
            if (state == null) return;
            _curState = state;
        }

        public IState GetStateWithTag(string tag)
        {
            if (_states == null) return null;
            foreach (var c in _states)
            {
                if (c.Value.Tag.Equals(tag))
                {
                    return c.Value;
                }
            }

            return null;
        }

        public IState GetStateWithName(string name)
        {
            if (_states == null) return null;
            if (_states.ContainsKey(name))
            {
                return _states[name];
            }

            return null;
        }

        public Dictionary<string, IState> States
        {
            get { return _states; }
        }

        public void OnUpdate(float delta)
        {
            _curState.OnUpdate(delta);
        }
    }
}