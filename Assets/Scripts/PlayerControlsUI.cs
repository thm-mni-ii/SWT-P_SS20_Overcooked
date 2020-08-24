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

        [SerializeField] TextMeshProUGUI forwardButtonText;

        [SerializeField] InputMaster inputMaster;

        /// <summary>
        /// Hides this screen.
        /// </summary>

        
        public void Close()
        {
            this.gameObject.SetActive(false);
        }

        public void ChangeForward(){
           
            forwardButtonText.text = "T";
            inputMaster.Player.Movement.PerformInteractiveRebinding()
            .WithControlsExcluding("Mouse")
            .WithCancelingThrough("<Keyboard>/escape")
            .WithTargetBinding(0)
            .WithBindingGroup("Keyboard")
            .OnMatchWaitForAnother(0.1f)
            .Start();
            
        }
    }
}
