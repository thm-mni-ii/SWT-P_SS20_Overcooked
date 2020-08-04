using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Underconnected
{
    public class LevelUI : MonoBehaviour
    {
        [SerializeField] GameTimer gameTimer = null;
        [SerializeField] DemandQueue gameDemandQueue = null;
        [SerializeField] ScoreDisplay scoreDisplay = null;

        public GameTimer GameTimer => this.gameTimer;
        public DemandQueue DemandQueue => this.gameDemandQueue;
        public ScoreDisplay ScoreDisplay => this.scoreDisplay;
    }
}
