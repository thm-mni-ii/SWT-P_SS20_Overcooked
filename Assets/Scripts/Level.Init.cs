/* Created by: SWT-P_SS20_Overcooked (Team Drai Studios) */
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Underconnected
{
    public partial class Level
    {
        /// <summary>
        /// Level init phase.
        /// Indicates that the level is loading and has not been started yet (by calling <see cref="StartLevel"/>).
        /// </summary>
        public class LevelInitPhase : LevelPhaseState
        {
            public override void OnStateEnter(State<LevelPhase> previousState)
            {
                // Make sure that the demand queue is empty and that the score display is at 0 when initializing a new level
                GameManager.UI.LevelUI.DemandQueue.Clear();
                GameManager.UI.LevelUI.ScoreDisplay.SetScore(0);
            }

            public override LevelPhase GetState() => LevelPhase.Init;
        }
    }
}
