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
        [Header("Keyboard")]
        [SerializeField] TextMeshProUGUI forwardKeyboardText;
        [SerializeField] TextMeshProUGUI leftKeyboardText;
        [SerializeField] TextMeshProUGUI backwardKeyboardText;
        [SerializeField] TextMeshProUGUI rightKeyboardText;
        [SerializeField] TextMeshProUGUI interactionKeyboardText;

        [Header("Gamepad")]
        [SerializeField] TextMeshProUGUI forwardGamepadText;
        [SerializeField] TextMeshProUGUI leftGamepadText;
        [SerializeField] TextMeshProUGUI backwardGamepadText;
        [SerializeField] TextMeshProUGUI rightGamepadText;
        [SerializeField] TextMeshProUGUI interactionGamepadText;

        [SerializeField] InputMaster inputMaster;

        /// <summary>
        /// Hides this screen.
        /// </summary>
        public void Close()
        {
            this.gameObject.SetActive(false);
        }

        public void ChangeKeyboardForward(){
            forwardKeyboardText.text = " ";
            inputMaster.Player.Movement.PerformInteractiveRebinding()
            .WithControlsExcluding("Mouse")
            .WithCancelingThrough("<Keyboard>/escape")
            .WithTargetBinding(0)
            .WithBindingGroup("Keyboard")
            .OnMatchWaitForAnother(0.1f)
            .Start();
        }

        public void ChangeKeyboardLeft() {
            leftKeyboardText.text = " ";
            inputMaster.Player.Movement.PerformInteractiveRebinding()
            .WithControlsExcluding("Mouse")
            .WithCancelingThrough("<Keyboard>/escape")
            .WithTargetBinding(0)
            .WithBindingGroup("Keyboard")
            .OnMatchWaitForAnother(0.1f)
            .Start();
        }

        public void ChangeKeyboardBackward() {

            backwardKeyboardText.text = " ";
            inputMaster.Player.Movement.PerformInteractiveRebinding()
            .WithControlsExcluding("Mouse")
            .WithCancelingThrough("<Keyboard>/escape")
            .WithTargetBinding(0)
            .WithBindingGroup("Keyboard")
            .OnMatchWaitForAnother(0.1f)
            .Start();
        }

        public void ChangeKeyboardRight() {
            rightKeyboardText.text = " ";
            inputMaster.Player.Movement.PerformInteractiveRebinding()
            .WithControlsExcluding("Mouse")
            .WithCancelingThrough("<Keyboard>/escape")
            .WithTargetBinding(0)
            .WithBindingGroup("Keyboard")
            .OnMatchWaitForAnother(0.1f)
            .Start();
        }

        public void ChangeKeyboardInteraction() {
            interactionKeyboardText.text = " ";
            inputMaster.Player.Movement.PerformInteractiveRebinding()
            .WithControlsExcluding("Mouse")
            .WithCancelingThrough("<Keyboard>/escape")
            .WithTargetBinding(0)
            .WithBindingGroup("Keyboard")
            .OnMatchWaitForAnother(0.1f)
            .Start();
        }

        public void ChangeGamepadForward() {
            forwardKeyboardText.text = " ";
            inputMaster.Player.Movement.PerformInteractiveRebinding()
            .WithControlsExcluding("Mouse") //necessary at Gamepad?
            .WithCancelingThrough("<Keyboard>/escape") //no Keyboard
            .WithTargetBinding(0)
            .WithBindingGroup("Gamepad")
            .OnMatchWaitForAnother(0.1f)
            .Start();
        }

        public void ChangeGamepadLeft() {
            leftKeyboardText.text = " ";
            inputMaster.Player.Movement.PerformInteractiveRebinding()
            .WithControlsExcluding("Mouse") //necessary at Gamepad?
            .WithCancelingThrough("<Keyboard>/escape") //no Keyboard
            .WithTargetBinding(0)
            .WithBindingGroup("Gamepad")
            .OnMatchWaitForAnother(0.1f)
            .Start();
        }

        public void ChangeGamepadBackward() {

            backwardKeyboardText.text = " ";
            inputMaster.Player.Movement.PerformInteractiveRebinding()
            .WithControlsExcluding("Mouse") //necessary at Gamepad?
            .WithCancelingThrough("<Keyboard>/escape") //no Keyboard
            .WithTargetBinding(0)
            .WithBindingGroup("Gamepad")
            .OnMatchWaitForAnother(0.1f)
            .Start();
        }

        public void ChangeGamepadRight() {
            rightKeyboardText.text = " ";
            inputMaster.Player.Movement.PerformInteractiveRebinding()
            .WithControlsExcluding("Mouse") //necessary at Gamepad?
            .WithCancelingThrough("<Keyboard>/escape") //no Keyboard
            .WithTargetBinding(0)
            .WithBindingGroup("Gamepad")
            .OnMatchWaitForAnother(0.1f)
            .Start();
        }

        public void ChangeGamepadInteraction() {
            interactionKeyboardText.text = " ";
            inputMaster.Player.Movement.PerformInteractiveRebinding()
            .WithControlsExcluding("Mouse") //necessary at Gamepad?
            .WithCancelingThrough("<Keyboard>/escape") //no Keyboard
            .WithTargetBinding(0)
            .WithBindingGroup("Gamepad")
            .OnMatchWaitForAnother(0.1f)
            .Start();
        }
    }
}
