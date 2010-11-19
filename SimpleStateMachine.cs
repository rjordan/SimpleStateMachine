using System;
using System.Collections.Generic;
using System.Linq;

namespace SimpleStateMachine {
    public class State {
        public State(string Key, string Description, string[] transitions) {
            this.Key = Key;
            this.Description = Description;
            this.Transitions = transitions;
        }

        public string Key { get; private set; }
        public string Description { get; private set; }
        public string[] Transitions { get; private set; }

        public delegate void EventHandler(string State);

        public EventHandler OnEnter; //Pass previous state
        public EventHandler OnExit; //Pass new state

    }

    [Serializable]
    public abstract class StateMachine {
        [NonSerialized]
        private Dictionary<string, State> _states = new Dictionary<string, State>();

        //This should allow serialization of the state
        protected string currentState;

        private State getState(string Key) {
            if (_states.Count == 0 || string.IsNullOrEmpty(Key)) 
                return null;
            return _states[Key];
        }

        public string State {
            get {
                return currentState;
            }
            set {
                if (currentState == value)
                    return;

                if (!string.IsNullOrEmpty(currentState) && !Transitions.Contains(value))
                    throw new ArgumentException("Invalid Transition!");

                State tmpState = getState(currentState);
                if (null != tmpState && null != tmpState.OnExit) {
                    foreach (State.EventHandler handler in getState(currentState).OnExit.GetInvocationList()) {
                        handler(value);
                    }
                }

                string prev_state = currentState;
                currentState = value;

                tmpState = getState(currentState);
                if (null != tmpState && null != tmpState.OnEnter) {
                    foreach (State.EventHandler handler in tmpState.OnEnter.GetInvocationList()) {
                        handler(prev_state);
                    }
                }
                    
            }
        }

        public string[] Transitions { get { return getState(currentState).Transitions; } }

        //Setup functions
        protected State[] States {
            set {
                foreach(State s in value) {
                    _states.Add(s.Key, s);
                }
            }
        }

        protected string InitialState {
            set {
                State = value;  //This makes sure onEnter gets called for the InitialState
            }
        }
    }
}
