using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Mirror;

public class Interactor : NetworkBehaviour
{
    [SerializeField] float interactReach = 0.5F;
    [SerializeField] LayerMask interactLayers;
    [SerializeField] Transform interactOrigin;


    void Update()
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
                        interactedObj.Interact(this);
                }
            }
        }
    }
}