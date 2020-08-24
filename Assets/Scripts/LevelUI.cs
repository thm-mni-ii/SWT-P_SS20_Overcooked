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


        /// <summary>
        /// The level timer UI element.
        /// </summary>
        public LevelTimerUI LevelTimer => this.levelTimer;
        /// <summary>
        /// The demand queue UI element.
        /// </summary>
        public DemandQueueUI DemandQueue => this.demandQueue;
        /// <summary>
        /// A display for score
        /// </summary>
        public ScoreDisplay ScoreDisplay => this.scoreDisplay;
    }
}
