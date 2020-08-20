using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;

namespace Underconnected {
    /// <summary>
    /// Represents the "Control Settings Screen" screen that is shown after ...
    /// </summary>
    public class PlayerControlsUI : MonoBehaviour {
        /// <summary>
        /// Hides this screen.
        /// </summary>
        public void Close()
        {
            this.gameObject.SetActive(false);
        }
    }
}
