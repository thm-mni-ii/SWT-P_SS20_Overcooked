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
        [SerializeField] DemandQueue gameDemandQueue = null;
        [SerializeField] ScoreDisplay scoreDisplay = null;


        /// <summary>
        /// The level timer UI element.
        /// </summary>
        public LevelTimerUI LevelTimer => this.levelTimer;
        /// <summary>
        /// Queue of incoming matter demands
        /// </summary>
        public DemandQueue DemandQueue => this.gameDemandQueue;
        /// <summary>
        /// A display for score
        /// </summary>
        public ScoreDisplay ScoreDisplay => this.scoreDisplay;
    }
}
