using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Mirror;
using UnityEngine.InputSystem;

namespace Underconnected

{
    /// <summary>
    /// Will check if local player presses the interaction key and if so calls Interact.
    /// </summary>
    public class Player : NetworkBehaviour
    {
        [SerializeField] Interactor interactor;

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
        /// Will check if local player presses the interaction key and if so calls <see cref="Interactor.Interact"/>.
        /// </summary>
         private void OnInteraction(){
            this.interactor.Interact();
        }
     
        #endregion
    }
    
}
