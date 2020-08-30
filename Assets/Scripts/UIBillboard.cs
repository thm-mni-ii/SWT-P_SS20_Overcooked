using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Underconnected
{
    /// <summary>
    /// Implements UI billboarding.
    /// Rotates the game object it is sitting on towards the camera.
    /// </summary>
    public class UIBillboard : MonoBehaviour
    {
        private void LateUpdate()
        {
            if (GameManager.Camera != null)
                this.transform.rotation = GameManager.Camera.transform.rotation;
        }
    }
}
