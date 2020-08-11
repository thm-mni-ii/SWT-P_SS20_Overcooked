using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Underconnected
{
    /// <summary>
    /// Represents a matter object which players can carry around on a level or place inside of lab equipment.
    /// Requires the game object to have a <see cref="PickableObject"/> component.
    /// </summary>
    [RequireComponent(typeof(PickableObject))]
    public class MatterObject : MonoBehaviour
    {
        [Header("References")]
        [SerializeField] Rigidbody[] rigidbodies;
        [SerializeField] Collider[] colliders;
        [SerializeField] PickableObject pickableObject;

        [Header("Settings")]
        [SerializeField] Matter matter;


        /// <summary>
        /// The matter this matter objects is representing.
        /// </summary>
        public Matter Matter => this.matter;


        private void Awake()
        {
            // Setup events for reacting to picking up and dropping this matter object
            this.pickableObject.OnPickedUp += this.OnPickedUp;
            this.pickableObject.OnDropped += this.OnDropped;
        }


        /// <summary>
        /// Enables gravity and collisions for this object.
        /// </summary>
        public void EnablePhysics()
        {
            foreach (Rigidbody rb in this.rigidbodies)
                rb.isKinematic = false;
            foreach (Collider c in this.colliders)
                c.enabled = true;
        }
        /// <summary>
        /// Disables gravity and collisions for this object.
        /// </summary>
        public void DisablePhysics()
        {
            foreach (Rigidbody rb in this.rigidbodies)
                rb.isKinematic = true;
            foreach (Collider c in this.colliders)
                c.enabled = false;
        }


        /// <summary>
        /// Called when this object has been picked up by an <see cref="Interactor"/> (usually a player).
        /// Calls <see cref="DisablePhysics"/> to disable physics for this object.
        /// </summary>
        /// <param name="pickedUp">The object that has been picked up.</param>
        /// <param name="picker">The interactor that has picked up <paramref name="pickedUp"/>.</param>
        private void OnPickedUp(PickableObject pickedUp, Interactor picker) => this.DisablePhysics();
        /// <summary>
        /// Called when this object has been dropped by an <see cref="Interactor"/> (usually a player).
        /// Calls <see cref="EnablePhysics"/> to enable physics for this object.
        /// </summary>
        /// <param name="dropped">The object that has been dropped.</param>
        /// <param name="dropper">The interactor that has dropped <paramref name="dropped"/>.</param>
        private void OnDropped(PickableObject dropped, Interactor dropper) => this.EnablePhysics();
    }
}
