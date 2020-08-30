﻿using System.Collections;
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
        [Tooltip("A reference to the level finished UI.")]
        [SerializeField] LevelFinishedUI levelFinishedUI;
        [Tooltip("A reference to the tutorial UI.")]
        [SerializeField] TutorialUI tutorialUI;


        /// <summary>
        /// A reference to the UI canvas.
        /// </summary>
        public Canvas UICanvas => this.uiCanvas;
        /// <summary>
        /// A reference to the level UI.
        /// </summary>
        public LevelUI LevelUI => this.levelUI;
        /// <summary>
        /// A reference to the level finished UI.
        /// </summary>
        public LevelFinishedUI LevelFinishedUI => this.levelFinishedUI;

        /// <summary>
        /// A reference to the tutorial UI.
        /// </summary>
        public TutorialUI TutorialUI => this.tutorialUI;

        /// <summary>
        /// Shows up the level finished screen, when the timer is finished.
        /// </summary>
        public void ShowLevelFinishedScreen() 
        {
            LevelFinishedUI.gameObject.SetActive(true);
            LevelUI.gameObject.SetActive(false);
        }

        public void showTutorial()
        {
            TutorialUI.gameObject.SetActive(true);
        }
        public void hideTutorial()
        {
            TutorialUI.gameObject.SetActive(false);
        }

    }
}
