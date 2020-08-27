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
        public StateMachine<T> StateMachine { get; protected set; }


        public virtual void Update(float deltaTime) { }
        public virtual void FixedUpdate(float deltaTime) { }
        public virtual void LateUpdate(float deltaTime) { }


        public virtual void OnRegistered(StateMachine<T> stateMachine) => this.StateMachine = stateMachine;

        public virtual void OnStateEnter(State<T> previousState) { }
        public virtual void OnStateLeave(State<T> nextState) { }

        public abstract T GetState();
    }
}
