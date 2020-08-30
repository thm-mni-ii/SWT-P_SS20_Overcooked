using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Mirror;

namespace Underconnected
{
    public partial class Level
    {
        /// <summary>
        /// The level finished phase.
        /// Indicates that a level has finished and should show its finished screen.
        /// </summary>
        public class LevelFinishedPhase : State<LevelPhase>
        {
            /// <summary>
            /// Tells whether this phase has been initialized on the server.
            /// </summary>
            private bool isRunningOnServer;
            /// <summary>
            /// Holds the level this phase belongs to.
            /// </summary>
            private Level level;


            /// <summary>
            /// Creates a new LevelFinishedPhase.
            /// </summary>
            /// <param name="level">The level this phase belongs to.</param>
            public LevelFinishedPhase(Level level) => this.level = level;


            public override void OnStateEnter(State<LevelPhase> previousState)
            {
                this.isRunningOnServer = NetworkServer.active;

                // Disable our own player's controls and show the finished screen and the player party UI
                this.level.OwnPlayer?.Controls.DisableControls();
                GameManager.UI.ShowLevelFinishedScreen();
                GameManager.UI.ShowPlayerPartyUI();
            }
            public override void OnStateLeave(State<LevelPhase> nextState)
            {
                // Remove and despawn all the players we had on this level when we are running as a server
                // (as despawning will be synchronized with the clients automatically)
                if (this.isRunningOnServer)
                {
                    this.level.RemoveAllPlayers();
                    this.isRunningOnServer = false;
                }

                // Hide the finished screen if it is still open
                GameManager.UI.LevelFinishedUI.Exit();
            }


            public override LevelPhase GetState() => LevelPhase.Finished;
        }
    }
}
