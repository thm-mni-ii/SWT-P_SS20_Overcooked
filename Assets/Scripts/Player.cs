using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Mirror;
using UnityEngine.InputSystem;

namespace Underconnected

{
    /// <summary>
    /// Will check if local player presses the 'E' key and if so calls Interact.
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
        /// Will check if local player presses the 'E' key and if so calls <see cref="Interactor.Interact"/>.
        /// </summary>
        /*private void Update()
        {
            if (this.isLocalPlayer)
            {
                if (this.isLocalPlayer)
                {
                    if (Input.GetKeyDown(KeyCode.E))
                        this.interactor.Interact();
                }
            }
        }*/
        /// <summary>
        /// 
        /// </summary>
         private void OnInteraction(){
            Debug.Log("OnInteraction wird aufgerufen");
            this.interactor.Interact();
        }

        /*private void OnInteract(InputValue value){
            Debug.Log("Wird ausgeführt");
            //this.interactor.Interact();
        }*/
     
        #endregion
    }
    
}
