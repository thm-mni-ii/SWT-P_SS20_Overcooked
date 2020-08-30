using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;

namespace Underconnected
{
    /// <summary>
    /// Represents a state machine that can have a single active state and move to other states.
    /// </summary>
    /// <typeparam name="TEnum">The enumeration containing all valid states for this state machine.</typeparam>
    public class StateMachine<TEnum>
        where TEnum : System.Enum
    {
        /// <summary>
        /// Holds the state this state machine is currently in.
        /// </summary>
        public TEnum CurrentState { get; private set; }
        /// <summary>
        /// Holds the actual state object associated with <see cref="CurrentState"/>.
        /// Can be `null` if this state machine has never been assigned a state yet.
        /// </summary>
        public State<TEnum> CurrentStateObject { get; private set; }


        /// <summary>
        /// Contains all states that are known to this state machine.
        /// Key is a value from the state enumeration, Value is the state object associated with that enum value.
        /// </summary>
        private Dictionary<TEnum, State<TEnum>> allStates;


        /// <summary>
        /// Fired when this state machine changes its state.
        /// Parameters: The new state object that is now the active one.
        /// </summary>
        public event UnityAction<State<TEnum>> OnStateChanged;


        /// <summary>
        /// Creates a new empty state machine.
        /// </summary>
        public StateMachine()
        {
            this.allStates = new Dictionary<TEnum, State<TEnum>>();
        }

        /// <summary>
        /// Updates this state machine and its current state.
        /// </summary>
        /// <param name="deltaTime">The time that has passed since the last update.</param>
        public void Update(float deltaTime) => this.CurrentStateObject?.Update(deltaTime);
        /// <summary>
        /// Updates this state machine and its current state.
        /// </summary>
        /// <param name="deltaTime">The time that has passed since the last fixed update.</param>
        public void FixedUpdate(float deltaTime) => this.CurrentStateObject?.FixedUpdate(deltaTime);
        /// <summary>
        /// Updates this state machine and its current state.
        /// </summary>
        /// <param name="deltaTime">The time that has passed since the last update.</param>
        public void LateUpdate(float deltaTime) => this.CurrentStateObject?.LateUpdate(deltaTime);


        /// <summary>
        /// Registers a new state for this state machine.
        /// </summary>
        /// <param name="stateObject">The state object to register. Cannot be `null`.</param>
        public void RegisterState(State<TEnum> stateObject)
        {
            if (stateObject != null && !this.allStates.ContainsKey(stateObject.GetState()))
            {
                this.allStates.Add(stateObject.GetState(), stateObject);
                stateObject.OnRegistered(this);
            }
        }

        /// <summary>
        /// Sets a new state for this state machine.
        /// </summary>
        /// <param name="state">The new state to set.</param>
        public void SetState(TEnum state)
        {
            State<TEnum> previousState = this.CurrentStateObject;
            State<TEnum> nextState = null;

            if (state != null)
                this.allStates.TryGetValue(state, out nextState);

            if (previousState != nextState)
            {
                this.CurrentState = state;
                this.CurrentStateObject = nextState;

                if (previousState != null)
                    previousState.OnStateLeave(nextState);
                if (nextState != null)
                    nextState.OnStateEnter(previousState);

                this.OnStateChanged?.Invoke(nextState);
            }
        }

        /// <summary>
        /// Shuts down this state machine by exiting the current state.
        /// </summary>
        public void Shutdown()
        {
            if (this.CurrentStateObject != null)
            {
                this.CurrentStateObject.OnStateLeave(null);
                this.CurrentStateObject = null;
                this.CurrentState = default(TEnum);
            }
        }
    }
}
