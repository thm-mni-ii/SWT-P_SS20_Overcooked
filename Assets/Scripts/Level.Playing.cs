using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Mirror;

namespace Underconnected
{
    public partial class Level
    {
        /// <summary>
        /// The playing phase.
        /// Indicates that a level is currently being played.
        /// </summary>
        public class LevelPlayingPhase : State<LevelPhase>
        {
            /// <summary>
            /// The coroutine that adds demands to the demands list.
            /// </summary>
            private Coroutine demandCoroutine;
            /// <summary>
            /// The wait object for <see cref="demandCoroutine"/>.
            /// </summary>
            private WaitForSeconds demandWait;
            /// <summary>
            /// The amount of seconds the demand coroutine waits between each iteration.
            /// </summary>
            private float demandWaitAmount;

            /// <summary>
            /// Tells whether this phase has been initialized on the server.
            /// </summary>
            private bool isRunningOnServer;
            /// <summary>
            /// Holds the level this phase belongs to.
            /// </summary>
            private Level level;


            /// <summary>
            /// Creates a new LevelPlayingPhase.
            /// </summary>
            /// <param name="level">The level this phase belongs to.</param>
            public LevelPlayingPhase(Level level)
            {
                this.level = level;
                this.demandWaitAmount = 0.1F;
                this.demandWait = new WaitForSeconds(this.demandWaitAmount);
            }


            public override void OnStateEnter(State<LevelPhase> previousState)
            {
                // Check whether the level has a timer and start it
                if (this.level.HasTimer)
                    this.level.Timer.StartTimer();

                // Check whether the level has a timer and subscribe UI events
                if (this.level.HasDemandQueue)
                {
                    // Add all demands that are already inside the queue to the demands list
                    foreach (Demand demand in this.level.demandQueue.CurrentDemands)
                        GameManager.UI.LevelUI.DemandQueue.AddDemand(demand);

                    this.SubscribeDemandQueueUI();
                }

                // If our player has been registered before: enable their controls
                this.level.OwnPlayer?.Controls.EnableControls();
                this.level.OnPlayerRegistered += this.Level_OnPlayerRegistered;

                if (!this.isRunningOnServer && NetworkServer.active)
                {
                    // Register server timer events
                    if (this.level.HasTimer)
                        this.level.Timer.OnTimerFinished += this.Timer_OnTimerFinished_Server;

                    // Register server demand queue events and start the demand queue coroutine
                    if (this.level.HasDemandQueue)
                    {
                        this.level.DemandQueue.OnDemandExpired += this.DemandQueue_OnDemandExpired_Server;
                        this.demandCoroutine = this.level.StartCoroutine(this.Do_DemandCoroutine());
                    }

                    // Setup events to create/destroy a player for each joining/leaving client automatically
                    GameManager.NetworkManager.OnClientJoin += this.level.AddPlayer;
                    GameManager.NetworkManager.OnClientLeave += this.level.RemovePlayer;

                    // Add a player for each client to this level
                    this.level.AddAllPlayers();

                    this.isRunningOnServer = true;
                }

                // Hide all player names
                foreach (Player p in this.level.AllPlayers)
                    p.HideName();
            }
            public override void OnStateLeave(State<LevelPhase> nextState)
            {
                // Stop the level timer if this level has one
                if (this.level.HasTimer)
                    this.level.Timer.StopTimer();

                // Clear demand queue and unsubscribe its events
                if (this.level.HasDemandQueue)
                {
                    this.UnsubscribeDemandQueueUI();
                    GameManager.UI.LevelUI.DemandQueue.Clear();
                }

                this.level.OnPlayerRegistered -= this.Level_OnPlayerRegistered;

                if (this.isRunningOnServer)
                {
                    // Unsubscribe from timer events
                    if (this.level.HasTimer)
                        this.level.Timer.OnTimerFinished -= this.Timer_OnTimerFinished_Server;

                    // Stop level timer and coroutines
                    if (this.level.HasDemandQueue)
                    {
                        this.level.StopCoroutine(this.demandCoroutine);
                        this.level.DemandQueue.OnDemandExpired -= this.DemandQueue_OnDemandExpired_Server;
                    }

                    // Unsubscribe from events
                    GameManager.NetworkManager.OnClientJoin -= this.level.AddPlayer;
                    GameManager.NetworkManager.OnClientLeave -= this.level.RemovePlayer;

                    this.isRunningOnServer = false;
                }
            }


            public override LevelPhase GetState() => LevelPhase.Playing;


            /// <summary>
            /// The coroutine used to add demands to the demand queue.
            /// </summary>
            private IEnumerator Do_DemandCoroutine()
            {
                float nextDemandTimeout = Random.Range(this.level.demandSpawnTimeMinMax.x, this.level.demandSpawnTimeMinMax.y);
                this.AddRandomDemand();

                while (true)
                {
                    yield return this.demandWait;
                    nextDemandTimeout -= this.demandWaitAmount;

                    if (nextDemandTimeout <= 0.0F || this.level.demandQueue.CurrentDemands.Count <= 0)
                    {
                        this.AddRandomDemand();
                        nextDemandTimeout = Random.Range(this.level.demandSpawnTimeMinMax.x, this.level.demandSpawnTimeMinMax.y);
                    }
                }
            }


            /// <summary>
            /// Adds a random demand to the demand queue.
            /// </summary>
            private void AddRandomDemand()
            {
                if (this.level.demandsPool.Length > 0)
                    this.level.DemandQueue.AddDemand(this.level.demandsPool[Random.Range(0, this.level.demandsPool.Length)], this.level.timePerDemand);
            }


            /// <summary>
            /// Subscribes to the demand queue events to start updating the demand queue UI.
            /// </summary>
            private void SubscribeDemandQueueUI()
            {
                this.level.demandQueue.OnDemandAdded += this.DemandQueue_OnDemandAdded;
                this.level.demandQueue.OnDemandRemoved += this.DemandQueue_OnDemandRemoved;
            }
            /// <summary>
            /// Unsubscribes from the demand queue events to stop updating the demand queue UI.
            /// </summary>
            private void UnsubscribeDemandQueueUI()
            {
                this.level.demandQueue.OnDemandAdded -= this.DemandQueue_OnDemandAdded;
                this.level.demandQueue.OnDemandRemoved -= this.DemandQueue_OnDemandRemoved;
            }


            /// <summary>
            /// Called when a player is spawned on this level.
            /// Checks whether it is our own player and enables their controls.
            /// </summary>
            /// <param name="player">The player that was registered.</param>
            private void Level_OnPlayerRegistered(Player player)
            {
                if (player.IsOwnPlayer)
                    player.Controls.EnableControls();
            }

            /// <summary>
            /// Called when the level timer is finished.
            /// Changes the level phase to <see cref="LevelPhase.Finished"/>.
            /// Is only called on the server.
            /// </summary>
            private void Timer_OnTimerFinished_Server() => this.level.ChangePhase(LevelPhase.Finished);

            /// <summary>
            /// Called when a new demand is added to the demand queue.
            /// Adds the demand to the demand queue UI.
            /// </summary>
            /// <param name="demand">The new demand that has been added.</param>
            private void DemandQueue_OnDemandAdded(Demand demand) => GameManager.UI.LevelUI.DemandQueue.AddDemand(demand);
            /// <summary>
            /// Called when a demand is removed from the demand queue.
            /// Removes the demand from the demand queue UI.
            /// </summary>
            /// <param name="demand">The demand that was removed.</param>
            private void DemandQueue_OnDemandRemoved(Demand demand) => GameManager.UI.LevelUI.DemandQueue.RemoveDemand(demand);
            /// <summary>
            /// Called when a demand's time limit is reached.
            /// Applies a score penalty.
            /// </summary>
            /// <param name="demand">The expired demand.</param>
            private void DemandQueue_OnDemandExpired_Server(Demand demand)
            {
                this.level.IncrementDeliveredFailedCounter();
                if (this.level.playerScore > demand.Matter.GetScoreFailPenalty())
                {
                    this.level.IncrementPlayerScore(-demand.Matter.GetScoreFailPenalty());
                    this.level.IncrementFailedDeliveredScore(-demand.Matter.GetScoreFailPenalty());

                }
                else if(this.level.playerScore > 0 && this.level.playerScore< demand.Matter.GetScoreFailPenalty())
                {
                    this.level.IncrementFailedDeliveredScore(-this.level.playerScore);
                    this.level.IncrementPlayerScore(-this.level.playerScore);
                }
                
            }
                
        }
    }
}
