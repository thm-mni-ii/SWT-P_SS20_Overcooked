using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Mirror;

namespace Underconnected
{
    /// <summary>
    /// 
    /// </summary>
    public class Interactor : NetworkBehaviour
    {
        [SerializeField] float interactReach = 0.5F;
        [SerializeField] LayerMask interactLayers;
        [SerializeField] Transform interactOrigin;

        /// <summary>
        /// 
        /// </summary>
        /// <value></value>
        public PickableObject HeldObject { get; private set; }
        /// <summary>
        /// Checks if holding an object
        /// </summary>
        public bool IsHoldingObject => this.HeldObject != null;

        /// <summary>
        /// 
        /// </summary>
        private RaycastHit[] hitResultsPool;


        private void Awake()
        {
            this.hitResultsPool = new RaycastHit[2];
        }


        public void Interact()
        {
            GameObject interactedObject = this.GetObjectToInteract();

            if (interactedObject != null)
                this.CmdRequestInteract(interactedObject.GetComponent<NetworkIdentity>());
        }
        /// <summary>
        /// 
        /// </summary>
        /// <param name="heldObject"></param>
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
        /// 
        /// </summary>
        /// <returns></returns>
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

        [Command]
        /// <summary>
        /// 
        /// </summary>
        /// <param name="interactable"></param>
        private void CmdRequestInteract(NetworkIdentity interactable)
        {
            GameObject actualInteractable = this.GetObjectToInteract();

            if (interactable != null && actualInteractable != null && actualInteractable.Equals(interactable.gameObject))
                this.RpcConfirmInteract(interactable);
            else
                Debug.LogWarning($"Client pickup check failed. Claimed: \"{interactable?.gameObject}\", Actual: \"{actualInteractable}\"");
        }
        [ClientRpc]
        /// <summary>
        /// 
        /// </summary>
        /// <param name="interactable"></param>
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
