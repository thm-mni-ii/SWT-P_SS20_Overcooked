using System.Collections;
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
        /// The object held by this player.
        /// </summary>
        public PickableObject HeldObject => this.interactor.HeldObject;
        /// <summary>
        /// Tells if this player is holding an object.
        /// </summary>
        public bool IsHoldingObject => this.interactor.IsHoldingObject;


        #region Unity Callbacks

        /// <summary>
        /// Will check if local player presses the 'E' key and if so calls <see cref="Interactor.Interact"/>.
        /// </summary>
        private void Update()
        {
            if (this.hasAuthority)
            {
                if (Input.GetKeyDown(KeyCode.E))
                    this.interactor.Interact();
            }
        }
        #endregion
    }
}
