using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Underconnected
{
    /// <summary>
    /// Manages the game UI.
    /// Contains references to all UI elements.
    /// </summary>
    public class UIManager : MonoBehaviour
    {
        [Header("References")]
        [Tooltip("A reference to the UI canvas.")]
        [SerializeField] Canvas uiCanvas;
        [Tooltip("A reference to the level UI.")]
        [SerializeField] LevelUI levelUI;


        /// <summary>
        /// A reference to the UI canvas.
        /// </summary>
        public Canvas UICanvas => this.uiCanvas;
        /// <summary>
        /// A reference to the level UI.
        /// </summary>
        public LevelUI LevelUI => this.levelUI;
    }
}
