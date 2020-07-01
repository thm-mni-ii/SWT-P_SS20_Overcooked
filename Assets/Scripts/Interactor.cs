using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Mirror;

public class Interactor : NetworkBehaviour
{
    [SerializeField] float interactReach = 0.5F;
    [SerializeField] LayerMask interactLayers;
    [SerializeField] Transform interactOrigin;


    public PickableObject HeldObject { get; private set; }
    public ModifiableObject HeldModifiedObject { get; private set; }
    public bool IsHoldingObject => this.HeldObject != null;
    public bool IsHoldingModifiedObject => this.HeldModifiedObject != null;


    public void Interact()
    {
        GameObject interactedObject = this.IsHoldingObject ? this.HeldObject.gameObject : this.GetObjectToInteract();

        if (interactedObject != null)
            this.CmdRequestInteract(interactedObject.GetComponent<NetworkIdentity>());
    }
    
    public void InteractWithDevice() {
        GameObject interactedObject = this.IsHoldingModifiedObject ? this.HeldObject.gameObject : this.GetObjectToInteract();

        if (interactedObject != null)
            this.CmdRequestInteract(interactedObject.GetComponent<NetworkIdentity>());
    }


    public void SetHeldObject(PickableObject heldObject)
    {
        this.HeldObject = heldObject;
    }

    public void SetHeldModifiedObject(ModifiableObject heldModifiedObject) {
        this.HeldModifiedObject = heldModifiedObject;
    }


    private GameObject GetObjectToInteract()
    {
        RaycastHit hitInfo;

        if (Physics.Raycast(this.interactOrigin.position, this.interactOrigin.forward, out hitInfo, this.interactReach, this.interactLayers))
            if (hitInfo.collider.GetComponent<IInteractable>() != null)
                return hitInfo.collider.gameObject;

        return null;
    }


    #region Network Code

    [Command]
    private void CmdRequestInteract(NetworkIdentity interactable)
    {
        GameObject actualInteractable = this.IsHoldingObject ? this.HeldObject.gameObject : this.GetObjectToInteract();

        if (interactable != null && actualInteractable != null && actualInteractable.Equals(interactable.gameObject))
            this.RpcConfirmInteract(interactable);
        else
            Debug.LogWarning($"Client pickup check failed. Claimed: \"{interactable?.gameObject}\", Actual: \"{actualInteractable}\"");
    }
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
