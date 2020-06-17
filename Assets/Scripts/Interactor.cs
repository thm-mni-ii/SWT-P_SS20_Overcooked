using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Mirror;

public class Interactor : NetworkBehaviour
{
    [SerializeField] float interactReach = 0.5F;
    [SerializeField] LayerMask interactLayers;
    [SerializeField] Transform interactOrigin;


    #region Unity Callbacks

    private void Update()
    {
        if (Input.GetKeyDown(KeyCode.Space))
        {
            if (this.isLocalPlayer)
            {
                GameObject interactedObj = this.GetObjectToInteract();
                if (interactedObj != null)
                    this.CmdRequestInteract(interactedObj.GetComponent<NetworkIdentity>());
            }
        }
    }

    #endregion


    #region Logic

    private GameObject GetObjectToInteract()
    {
        RaycastHit hitInfo;

        if (Physics.Raycast(this.interactOrigin.position, this.interactOrigin.forward, out hitInfo, this.interactReach, this.interactLayers))
            if (hitInfo.collider.GetComponent<IInteractable>() != null)
                return hitInfo.collider.gameObject;

        return null;
    }

    #endregion


    #region Network Code

    [Command]
    private void CmdRequestInteract(NetworkIdentity interactable)
    {
        GameObject actualInteractable = this.GetObjectToInteract();
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
