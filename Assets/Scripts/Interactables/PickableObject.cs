﻿/* Created by: SWT-P_SS20_Overcooked (Team Drai Studios) */
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;
using Mirror;

namespace Underconnected
{
    /// <summary>
    /// Represents an object that can be picked up by an <see cref="Interactor"/>.
    /// </summary>
    public class PickableObject : NetworkBehaviour, IInteractable
    {
        [Tooltip("The default parent to apply when this pickable is dropped.")]
        [SerializeField] Transform defaultParent = null;
        [SerializeField] bool canBeDropped = true;
        [SerializeField] Rigidbody[] nonKinematicRBs;
        [SerializeField] ObjectHighlighting highlighting;


        /// <summary>
        /// Tells whether this object has been picked and is currently held by an <see cref="Interactor"/>.
        /// </summary>
        public bool IsPickedUp => this.currentHolder != null;
        /// <summary>
        /// Tells whether this object can be dropped after it is picked up.
        /// </summary>
        public bool CanBeDropped => this.canBeDropped;
        /// <summary>
        /// Returns the <see cref="Interactor"/> that is currently holding this object.
        /// </summary>
        public Interactor CurrentHolder => this.currentHolder;


        /// <summary>
        /// Called when this object has been picked up.
        /// Accepts 2 parameters: a <see cref="PickableObject"/> that has been interacted with and an <see cref="Interactor"/> that has interacted with that object.
        /// </summary>
        public event UnityAction<PickableObject, Interactor> OnPickedUp;
        /// <summary>
        /// Called when this object has been dropped.
        /// Accepts 2 parameters: a <see cref="PickableObject"/> that has been dropped and an <see cref="Interactor"/> that has dropped that object.
        /// </summary>
        public event UnityAction<PickableObject, Interactor> OnDropped;


        /// <summary>
        /// Stores the <see cref="Interactor"/> that is currently holding this object.
        /// </summary>
        private Interactor currentHolder;

        /// <summary>
        /// Holds the interactor that is currently looking at this interactable.
        /// </summary>
        private Interactor watchingInteractor;


        private void OnDisable()
        {
            if (this.IsPickedUp)
                this.Drop(this.CurrentHolder);

            this.watchingInteractor = null;
        }


        public void Interact(Interactor interactor)
        {
            if (!this.IsPickedUp)
                this.Pickup(interactor);
            else if (this.canBeDropped)
                this.Drop(interactor);
        }
        public void SetWatcher(Interactor watcher)
        {
            this.watchingInteractor = watcher;
            if (watcher != null)
                this.highlighting.ShowHighlighting();
            else
                this.highlighting.HideHighlighting();
        }
        public GameObject GetGameObject() => this.gameObject;

        /// <summary>
        /// Sets whether this object can be dropped after it is picked up.
        /// </summary>
        /// <param name="canBeDropped">The new value.</param>
        public void SetDroppable(bool canBeDropped)
        {
            this.canBeDropped = canBeDropped;
        }

        /// <summary>
        /// Picks up this object and attaches it to the given <see cref="Interactor"/> object.
        /// Resets the rotation of the object to make the orb text visible.
        /// </summary>
        /// <param name="interactor">The <see cref="Interactor"/> object that should pick up this object.</param>
        public virtual void Pickup(Interactor interactor)
        {
            if (interactor != this.currentHolder)
            {
                this.currentHolder = interactor;
                interactor.SetHeldObject(this);
                this.transform.SetParent(interactor.transform, true);

                foreach (Rigidbody rb in this.nonKinematicRBs)
                    rb.isKinematic = true;

                this.OnPickedUp?.Invoke(this, interactor);
                this.transform.rotation = Quaternion.Euler(0, 90, 60);
            }

        }
        /// <summary>
        /// Drops this object.
        /// Resets the rotation of the object to make the orb text visible.
        /// Will only drop it if the given interactor matches <see cref="CurrentHolder"/>.
        /// </summary>
        /// <param name="interactor">The <see cref="Interactor"/> object that should drop this object.</param>
        public virtual void Drop(Interactor interactor)
        {
            if (interactor == this.currentHolder)
            {
                this.currentHolder = null;
                interactor.SetHeldObject(null);
                this.transform.SetParent(this.defaultParent);

                foreach (Rigidbody rb in this.nonKinematicRBs)
                    rb.isKinematic = false;

                this.OnDropped?.Invoke(this, interactor);
                this.transform.rotation = Quaternion.Euler(0, 90, 60);
            }
        }
    }
}
