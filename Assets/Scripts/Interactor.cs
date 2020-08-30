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
        [SerializeField] ContentsUI holdingContentsUI;


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
        /// Holds the interactable game object this interactor is currently looking at.
        /// </summary>
        public IInteractable LookedAtObject { get; private set; }


        /// <summary>
        /// Holds a result pool that is used inside of <see cref="GetObjectToInteract"/> to store raycast hit results.
        /// The idea behind this member variable is to avoid allocating a new array each time <see cref="GetObjectToInteract"/> is called and reuse this pre-allocated array instead.
        /// </summary>
        private RaycastHit[] hitResultsPool;

        /// <summary>
        /// The object id that was transmitted when this interactor was deserialized.
        /// </summary>
        private uint pendingHeldObjectID;


        private void Awake()
        {
            this.LookedAtObject = null;
            this.hitResultsPool = new RaycastHit[2];
        }
        private void Start()
        {
            if (this.pendingHeldObjectID > 0)
            {
                this.SetHeldObject(NetworkIdentity.spawned[this.pendingHeldObjectID].GetComponent<PickableObject>());
                this.pendingHeldObjectID = 0;
            }
        }
        private void Update()
        {
            IInteractable toInteract = this.GetObjectToInteract();

            if (this.LookedAtObject != toInteract && this.LookedAtObject != null)
                this.LookedAtObject.SetWatcher(null);

            this.LookedAtObject = toInteract;
            if (toInteract != null)
                toInteract.SetWatcher(this);
        }

        private void OnDisable()
        {
            if (this.LookedAtObject != null)
            {
                this.LookedAtObject.SetWatcher(null);
                this.LookedAtObject = null;
            }
        }

        public override bool OnSerialize(NetworkWriter writer, bool initialState)
        {
            bool dataWritten = base.OnSerialize(writer, initialState);

            if (initialState)
            {
                NetworkIdentity identity = this.IsHoldingObject ? this.HeldObject.GetComponent<NetworkIdentity>() : null;

                writer.WriteBoolean(identity != null);
                if (identity != null)
                    writer.WriteUInt32(identity.netId);

                dataWritten = true;
            }

            return dataWritten;
        }
        public override void OnDeserialize(NetworkReader reader, bool initialState)
        {
            base.OnDeserialize(reader, initialState);

            if (initialState)
            {
                if (reader.ReadBoolean())
                    this.pendingHeldObjectID = reader.ReadUInt32();
            }
        }


        /// <summary>
        /// Attempts to interact with interactable objects in reach.
        /// Performs a raycast from its <see cref="interactOrigin"/> to determine which objects are in range and tries to interact with the nearest one.
        /// Will do nothing if there are no interactables in range.
        /// </summary>
        public void Interact()
        {
            //IInteractable interactedObject = this.GetObjectToInteract();

            if (this.LookedAtObject != null)
                this.CmdRequestInteract(this.LookedAtObject.GetGameObject().GetComponent<NetworkIdentity>());
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
                {
                    if (this.holdingContentsUI != null && this.HeldObject.GetComponent<MatterObject>() != null)
                        this.holdingContentsUI.RemoveMatter(this.HeldObject.GetComponent<MatterObject>().Matter);

                    this.HeldObject.Drop(this);
                }

                this.HeldObject = heldObject;

                if (this.HeldObject != null)
                {
                    if (this.holdingContentsUI != null && this.HeldObject.GetComponent<MatterObject>() != null)
                        this.holdingContentsUI.AddMatter(this.HeldObject.GetComponent<MatterObject>().Matter);

                    this.HeldObject.Pickup(this);
                }
            }
        }

        /// <summary>
        /// Tries to find an <see cref="IInteractable"/> object in reach.
        /// Performs a raycast from its <see cref="interactOrigin"/> to determine which objects are in range and tries to interact with the nearest one.
        /// If it cannot find any, it will return the current <see cref="HeldObject"/> so it can be dropped.
        /// </summary>
        /// <returns>The found <see cref="IInteractable"/> to interact with or the value of <see cref="HeldObject"/> if none found.</returns>
        private IInteractable GetObjectToInteract()
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
                        return this.hitResultsPool[i].collider.GetComponent<IInteractable>();
                    }
                }

                return this.hitResultsPool[0].collider.GetComponent<IInteractable>();
            }
            else
                return this.HeldObject;
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
            if (interactable != null && this.LookedAtObject != null && this.LookedAtObject.GetGameObject().Equals(interactable.gameObject))
                this.RpcConfirmInteract(interactable);
            else
                Debug.LogWarning($"Client interaction check failed. Claimed: \"{interactable?.gameObject}\", Actual: \"{this.LookedAtObject?.GetGameObject()}\"");
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
