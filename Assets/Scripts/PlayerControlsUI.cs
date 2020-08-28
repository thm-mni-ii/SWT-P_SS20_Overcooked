using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;
using UnityEngine.InputSystem;

namespace Underconnected {
    /// <summary>
    /// Represents the "Control Settings Screen" screen that is shown after ...
    /// </summary>
    public class PlayerControlsUI : MonoBehaviour {
        [SerializeField] Button activeButton;
        
        private void OnEnable() {
            activeButton.Select();
        }

        public void Close(){
            this.gameObject.SetActive(false);
        }

        public void OnAbort(InputValue value) {
            Debug.Log("OnAbort wird aufgerufen");
            if(this.gameObject.activeSelf){
                this.gameObject.SetActive(false);
            }else{
                this.gameObject.SetActive(true);
            }
        }
       
    }
}
