using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Mirror;

namespace Underconnected
{
    /// <summary>
    /// Represents an interactor that is able to interact with <see cref="IInteractable"/> objects.
    /// </summary>
    public class Interactor : NetworkBehaviour
    {
        [SerializeField] float interactReach = 0.5F;
        [SerializeField] LayerMask interactLayers;
        [SerializeField] Transform interactOrigin;

        /// <summary>
        /// The <see cref="PickableObject"/> that is currently picked up by this interactor.
        /// </summary>
        /// <value></value>
        public PickableObject HeldObject { get; private set; }
        /// <summary>
        /// Checks if this interactor is holding a <see cref="PickableObject"/>.
        /// </summary>
        public bool IsHoldingObject => this.HeldObject != null;


        /// <summary>
        /// Holds a result pool that is used inside of <see cref="GetObjectToInteract"/> to store raycast hit results.
        /// The idea behind this member variable is to avoid allocating a new array each time <see cref="GetObjectToInteract"/> is called and reuse this pre-allocated array instead.
        /// </summary>
        private RaycastHit[] hitResultsPool;


        private void Awake()
        {
            this.hitResultsPool = new RaycastHit[2];
        }


        /// <summary>
        /// Attempts to interact with interactable objects in reach.
        /// Performs a raycast from its <see cref="interactOrigin"/> to determine which objects are in range and tries to interact with the nearest one.
        /// Will do nothing if there are no interactables in range.
        /// </summary>
        public void Interact()
        {
            GameObject interactedObject = this.GetObjectToInteract();

            if (interactedObject != null)
                this.CmdRequestInteract(interactedObject.GetComponent<NetworkIdentity>());
        }
        /// <summary>
        /// Sets the held object for this interactor.
        /// Will drop the previous held object if there is one.
        /// </summary>
        /// <param name="heldObject">The new object to hold. `null` will simply drop the currently held object.</param>
        public void SetHeldObject(PickableObject heldObject)
        {
            if (this.HeldObject != heldObject)
            {
                if (this.HeldObject != null)
                    this.HeldObject.Drop(this);

                this.HeldObject = heldObject;

                if (this.HeldObject != null)
                    this.HeldObject.Pickup(this);
            }
        }

        /// <summary>
        /// Tries to find an <see cref="IInteractable"/> object in reach.
        /// Performs a raycast from its <see cref="interactOrigin"/> to determine which objects are in range and tries to interact with the nearest one.
        /// If it cannot find any, it will return the current <see cref="HeldObject"/> so it can be dropped.
        /// </summary>
        /// <returns>The found <see cref="IInteractable"/> to interact with or the value of <see cref="HeldObject"/> if none found.</returns>
        private GameObject GetObjectToInteract()
        {
            int resultsAmount = Physics.RaycastNonAlloc(this.interactOrigin.position, this.interactOrigin.forward, this.hitResultsPool, this.interactReach, this.interactLayers);

            if (resultsAmount > 0)
            {
                if (resultsAmount > 1 && this.IsHoldingObject)
                {
                    for (int i = 0; i < resultsAmount; i++)
                    {
                        if (this.hitResultsPool[i].collider.gameObject.Equals(this.HeldObject.gameObject))
                            continue;
                        if (this.hitResultsPool[i].collider.GetComponent<IInteractable>() != null)
                            return this.hitResultsPool[i].collider.gameObject;
                    }
                }

                if (this.hitResultsPool[0].collider.GetComponent<IInteractable>() != null)
                    return this.hitResultsPool[0].collider.gameObject;
            }
            else if (this.HeldObject != null)
                return this.HeldObject.gameObject;

            return null;
        }


        #region Network Code

        /// <summary>
        /// Tells the server that this client wants to interact with the given <paramref name="interactable"/>.
        /// The server checks whether the client is able to interact with the given <paramref name="interactable"/> and broadcasts <see cref="RpcConfirmInteract(NetworkIdentity)"/> to all clients if the check was successful.
        /// Sent by a client to the server.
        /// </summary>
        /// <param name="interactable">The <see cref="NetworkIdentity"/> component of the interactable object the client attempts to interact with.</param>
        [Command]
        private void CmdRequestInteract(NetworkIdentity interactable)
        {
            GameObject actualInteractable = this.GetObjectToInteract();

            if (interactable != null && actualInteractable != null && actualInteractable.Equals(interactable.gameObject))
                this.RpcConfirmInteract(interactable);
            else
                Debug.LogWarning($"Client interaction check failed. Claimed: \"{interactable?.gameObject}\", Actual: \"{actualInteractable}\"");
        }
        /// <summary>
        /// Tells a client that this interactor has interacted with the given <paramref name="interactable"/>.
        /// Simulates the interaction on the client side.
        /// Sent by the server to a client, usually as a response to <see cref="CmdRequestInteract(NetworkIdentity)"/>.
        /// </summary>
        /// <param name="interactable">The <see cref="NetworkIdentity"/> component of the object this interactor has interacted with.</param>
        [ClientRpc]
        private void RpcConfirmInteract(NetworkIdentity interactable)
        {
            IInteractable i = interactable.GetComponent<IInteractable>();

            if (i != null)
                i.Interact(this);
            else
                Debug.LogError("Interactor: Server pointed to a non interactable object.");
        }

        #endregion
    }
}
