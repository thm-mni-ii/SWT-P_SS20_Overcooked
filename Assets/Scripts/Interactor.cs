using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Mirror;

public class Interactor : NetworkBehaviour
{
    [SerializeField] float interactReach = 0.5F;
    [SerializeField] LayerMask interactLayers;
    [SerializeField] Transform interactOrigin;


    private void Update()
    {
        if (Input.GetKeyDown(KeyCode.Space))
        {
            if (this.isLocalPlayer)
            {
                RaycastHit hitInfo;
                IInteractable interactedObj;
                if (Physics.Raycast(interactOrigin.position, interactOrigin.forward, out hitInfo, interactReach, interactLayers))
                {
                    interactedObj = hitInfo.collider.GetComponent<IInteractable>();
                    if (interactedObj != null)
                        this.CmdRequestInteract(hitInfo.collider.GetComponent<NetworkIdentity>());
                }
            }
        }
    }


    [Command]
    private void CmdRequestInteract(NetworkIdentity interactable)
    {
        this.RpcConfirmInteract(this.GetComponent<NetworkIdentity>(), interactable);
    }
    [ClientRpc]
    private void RpcConfirmInteract(NetworkIdentity interactor, NetworkIdentity interactable)
    {
        interactable.GetComponent<IInteractable>().Interact(interactor.GetComponent<Interactor>());
    }
}
