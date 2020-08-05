﻿using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Mirror;

namespace Underconnected
{
    /// <summary>
    /// Will check if local player presses the 'E' key and if so calls Interact.
    /// </summary>
    public class Player : NetworkBehaviour
    {
        [SerializeField] Interactor interactor;
        [SerializeField] PlayerControls controls;

        /// <summary>
        /// the held object of local player
        /// </summary>
        public PickableObject HeldObject => this.interactor.HeldObject;
        /// <summary>
        /// checks if player is holding an object
        /// </summary>
        public bool IsHoldingObject => this.interactor.IsHoldingObject;


        #region Unity Callbacks

        /// <summary>
        /// Will check if local player presses the 'E' key and if so calls Interact.
        /// </summary>
        private void Update()
        {
            if (this.isLocalPlayer)
            {
                if (this.isLocalPlayer)
                {
                    if (Input.GetKeyDown(KeyCode.E))
                        this.interactor.Interact();
                }
            }
        }
        #endregion
    }
}
