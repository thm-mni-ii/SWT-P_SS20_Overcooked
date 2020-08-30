/* Created by: SWT-P_SS20_Overcooked (Team Drai Studios) */
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Underconnected
{
    /// <summary>
    /// creates a UI for level
    /// </summary>
    public class LevelUI : MonoBehaviour
    {
        [SerializeField] LevelTimerUI levelTimer = null;
        [SerializeField] DemandQueueUI demandQueue = null;
        [SerializeField] ScoreDisplay scoreDisplay = null;
        [SerializeField] PreparingUI preparingUI = null;


        /// <summary>
        /// The level timer UI element.
        /// </summary>
        public LevelTimerUI LevelTimer => this.levelTimer;
        /// <summary>
        /// The demand queue UI element.
        /// </summary>
        public DemandQueueUI DemandQueue => this.demandQueue;
        /// <summary>
        /// A display for player score.
        /// </summary>
        public ScoreDisplay ScoreDisplay => this.scoreDisplay;
        /// <summary>
        /// The preparing UI.
        /// </summary>
        public PreparingUI PreparingUI => this.preparingUI;
    }
}
