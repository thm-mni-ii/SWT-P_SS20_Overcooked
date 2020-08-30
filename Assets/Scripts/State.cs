using System.Collections;
using System.Collections.Generic;
using UnityEngine;


namespace Underconnected
{
    /// <summary>
    /// Represents the state inside of a state machine.
    /// </summary>
    /// <typeparam name="T">The enum containing all possible states.</typeparam>
    public abstract class State<T>
        where T : System.Enum
    {
        /// <summary>
        /// Holds the state machine this state was registered to.
        /// </summary>
        public StateMachine<T> StateMachine { get; protected set; }


        /// <summary>
        /// Updates this state.
        /// </summary>
        /// <param name="deltaTime">The time that has passed since the last update.</param>
        public virtual void Update(float deltaTime) { }
        /// <summary>
        /// Updates this state.
        /// </summary>
        /// <param name="deltaTime">The time that has passed since the last fixed update.</param>
        public virtual void FixedUpdate(float deltaTime) { }
        /// <summary>
        /// Updates this state.
        /// </summary>
        /// <param name="deltaTime">The time that has passed since the last update.</param>
        public virtual void LateUpdate(float deltaTime) { }


        /// <summary>
        /// Called when this state has been registered.
        /// </summary>
        /// <param name="stateMachine">The state machine this state has been registered to.</param>
        public virtual void OnRegistered(StateMachine<T> stateMachine) => this.StateMachine = stateMachine;

        /// <summary>
        /// Called when this state becomes the active one.
        /// </summary>
        /// <param name="previousState">The previous state that was replaced by this state.</param>
        public virtual void OnStateEnter(State<T> previousState) { }
        /// <summary>
        /// Called when this state is no longer the active one.
        /// </summary>
        /// <param name="nextState">The state that has replaced this state.</param>
        public virtual void OnStateLeave(State<T> nextState) { }

        /// <summary>
        /// Returns the state associated with this state object.
        /// </summary>
        /// <returns>The state associated with this state object.</returns>
        public abstract T GetState();
    }
}
