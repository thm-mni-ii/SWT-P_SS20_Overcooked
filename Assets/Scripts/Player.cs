using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Mirror;

public class Player : NetworkBehaviour
{
    [SerializeField] Interactor interactor;
    [SerializeField] PlayerControls controls;


    public PickableObject HeldObject => this.interactor.HeldObject;
    public bool IsHoldingObject => this.interactor.IsHoldingObject;


    #region Unity Callbacks

    private void Update()
    {
        if (this.isLocalPlayer)
        {
            if (Input.GetKeyDown(KeyCode.E))
                this.interactor.Interact();
        }
    }

    #endregion
}
