using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;
using UnityEngine.InputSystem;

namespace Underconnected {
    /// <summary>
    /// Represents the "Control Settings Screen" screen that is shown after press escape/start.
    /// </summary>
    public class PlayerControlsUI : MonoBehaviour {
        /// <summary>
        /// A reference to the selected Button when this screen appears.
        /// </summary>
        [SerializeField] Button activeButton;

        /// <summary>
        /// Selects the KeyboardMovementRebindButton to navigate this screen with the keyboard/gamepad.
        /// </summary>
        private void OnEnable() 
        {
            activeButton.Select();
        }

        /// <summary>
        /// Hides this screen.
        /// </summary>
        public void Close()
        {
            this.gameObject.SetActive(false);
        }
    }
}
