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
        [Tooltip("A reference to the level finished UI.")]
        [SerializeField] LevelFinishedUI levelFinishedUI;
        [Tooltip("A reference to the party player UI.")]
        [SerializeField] PlayerParty playerParty;
        [Tooltip("A reference to the Tutorial UI.")]
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
        /// A reference to the player party UI.
        /// </summary>
        public PlayerParty PlayerParty => this.playerParty;



        /// <summary>
        /// Hides all UI elements, screens and menus.
        /// </summary>
        public void HideAllUI()
        {
            this.HideLevelUI();
            this.LevelFinishedUI.gameObject.SetActive(false);
            //this.PlayerParty.gameObject.SetActive(false);
        }


        /// <summary>
        /// Shows the level UI.
        /// </summary>
        public void ShowLevelUI() => this.levelUI.gameObject.SetActive(true);
        /// <summary>
        /// Hides the level UI.
        /// </summary>
        public void HideLevelUI() => this.levelUI.gameObject.SetActive(false);

        /// <summary>
        /// Shows the level finished screen.
        /// </summary>
        public void ShowLevelFinishedScreen()
        {
            this.LevelFinishedUI.SetNumOfLevel(GameManager.CurrentLevelNum);
            this.LevelFinishedUI.gameObject.SetActive(true);
        }

        /// <summary>
        /// Shows the player party UI.
        /// </summary>
        public void ShowPlayerPartyUI() {
            this.PlayerParty.gameObject.SetActive(true);
        }
        /// <summary>
        /// Hides the player party UI.
        /// </summary>
        public void HidePlayerPartyUI() {
            this.PlayerParty.gameObject.SetActive(false);
        }
        /// <summary>
        /// Shows the tutorial UI.
        /// </summary>

        public void showTutorialUI()
        {
            this.tutorialUI.LoadTutorialUI();
        }
    }

}
