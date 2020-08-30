/* Created by: SWT-P_SS20_Overcooked (Team Drai Studios) */
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Mirror;

namespace Underconnected
{
    public partial class Level
    {
        /// <summary>
        /// The preparing phase which shows a timer running down until the main phase (<see cref="LevelPhase.Playing"/>).
        /// </summary>
        public class LevelPreparingPhase : LevelPhaseState
        {
            /// <summary>
            /// Holds the time in seconds left until the main phase.
            /// </summary>
            private float preparingTimer;

            /// <summary>
            /// Tells whether this phase has been initialized on the server.
            /// </summary>
            private bool isRunningOnServer;
            /// <summary>
            /// Holds the level this phase belongs to.
            /// </summary>
            private Level level;


            /// <summary>
            /// Creates a new LevelPreparingPhase.
            /// </summary>
            /// <param name="level">The level this phase belongs to.</param>
            public LevelPreparingPhase(Level level)
            {
                this.level = level;
                this.preparingTimer = 10.0F;
            }


            public override void Update(float deltaTime)
            {
                if (this.preparingTimer > 0.0F)
                {
                    this.preparingTimer = Mathf.Max(this.preparingTimer - deltaTime, 0.0F);

                    if (this.preparingTimer <= 3.0F)
                        GameManager.UI.LevelUI.PreparingUI.ShowCountdownNumber(Mathf.CeilToInt(this.preparingTimer));

                    if (this.isRunningOnServer && this.preparingTimer <= 0.0F)
                        this.level.ChangePhase(LevelPhase.Playing);
                }
            }


            public override void OnStateEnter(State<LevelPhase> previousState)
            {
                GameManager.UI.LevelUI.PreparingUI.ShowReady();

                this.level.OnPlayerRegistered += this.Level_OnPlayerRegistered;

                if (!this.isRunningOnServer && NetworkServer.active)
                {
                    // Setup events to create/destroy a player for each joining/leaving client automatically
                    GameManager.NetworkManager.OnClientJoin += this.level.AddPlayer;
                    GameManager.NetworkManager.OnClientLeave += this.level.RemovePlayer;

                    // Add a player for each client to this level
                    this.level.AddAllPlayers();

                    this.isRunningOnServer = true;
                }
            }
            public override void OnStateLeave(State<LevelPhase> nextState)
            {
                GameManager.UI.LevelUI.PreparingUI.ShowGo();

                this.level.OnPlayerRegistered -= this.Level_OnPlayerRegistered;

                if (this.isRunningOnServer)
                {
                    // Setup events to create/destroy a player for each joining/leaving client automatically
                    GameManager.NetworkManager.OnClientJoin -= this.level.AddPlayer;
                    GameManager.NetworkManager.OnClientLeave -= this.level.RemovePlayer;

                    this.isRunningOnServer = false;
                }
            }

            public override LevelPhase GetState() => LevelPhase.Preparing;


            /// <summary>
            /// Called when a player is spawned on this level.
            /// Checks whether it is our own player and disables their controls.
            /// </summary>
            /// <param name="player">The player that was registered.</param>
            private void Level_OnPlayerRegistered(Player player)
            {
                if (player.IsOwnPlayer)
                    player.Controls.DisableControls();

                player.ShowName();
            }
        }
    }
}
