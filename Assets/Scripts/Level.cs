﻿using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Mirror;

namespace Underconnected
{
    /// <summary>
    /// Represents a game level.
    /// </summary>
    public class Level : NetworkBehaviour
    {
        [Header("Settings")]
        [SerializeField] int scorePerDelivery = 50;
        [SerializeField] int levelDurationSeconds = 180;
        [SerializeField] Matter[] demandsPool;
        [SerializeField] Transform[] spawnPoints;


        /// <summary>
        /// Holds the score the players currently have.
        /// </summary>
        public int PlayerScore => this.PlayerScore;
        /// <summary>
        /// Holds all the possible spawn locations for this level.
        /// </summary>
        public Transform[] SpawnPoints => this.spawnPoints;

        /// <summary>
        /// The score the players currentlyy have.
        /// </summary>
        [SyncVar(hook = nameof(PlayerScore_OnChange))] int playerScore;
        /// <summary>
        /// The coroutine that adds demands to the demands list.
        /// </summary>
        private Coroutine demandCoroutine;
        /// <summary>
        /// A timeout object used by <see cref="demandCoroutine"/> to wait for a certain amount of time.
        /// </summary>
        private WaitForSeconds demandCoroutineWait;


        private void Awake()
        {
            this.playerScore = 0;
            this.demandCoroutine = null;
            this.demandCoroutineWait = new WaitForSeconds(20.0F);
        }

        public override void OnStartServer()
        {
            GameManager.UI.LevelUI.GameTimer.SetTime(levelDurationSeconds);
            GameManager.UI.LevelUI.GameTimer.StartTimer();

            this.demandCoroutine = this.StartCoroutine(this.Do_DemandCoroutine());
        }
        public override void OnStopServer()
        {
            this.StopCoroutine(this.demandCoroutine);
            GameManager.UI.LevelUI.GameTimer.StopTimer();
        }
    
        //comment
        public override void OnStartClient() {
            GameManager.UI.LevelUI.GameTimer.OnTimerFinished += this.GameTimer_OnTimerFinished;
        }

        /// <summary>
        /// Returns the spawn location for the given player number.
        /// </summary>
        /// <param name="playerNum">The player number.</param>
        /// <returns>The spawn location for the given <paramref name="playerNum"/>.</returns>
        public Transform GetSpawnForPlayer(int playerNum) => this.spawnPoints.Length > 0 ? this.spawnPoints[playerNum % this.spawnPoints.Length] : null;

        /// <summary>
        /// Attempts to deliver the given matter object.
        /// Checks the demand queue whether the given matter is demanded, destroys it and increments the player score.
        /// Does nothing if the given matter is not inside the demand queue.
        /// Only has effect when called on the server.
        /// </summary>
        /// <param name="matterObject">The matter object to deliver.</param>
        public void DeliverObject(MatterObject matterObject)
        {
            Matter matter = matterObject != null ? matterObject.Matter : null;

            if (this.isServer && matter != null)
            {
                if (GameManager.UI.LevelUI.DemandQueue.HasDemand(matter))
                {
                    this.IncrementPlayerScore(this.scorePerDelivery);
                    GameManager.UI.LevelUI.DemandQueue.DeliverDemand(matter);
                    NetworkServer.Destroy(matterObject.gameObject);
                }
            }
        }

        /// <summary>
        /// Increments the player score by the given amount.
        /// Only has effect when called on the server.
        /// </summary>
        /// <param name="scoreDelta">The amount by which to increase the player score.</param>
        public void IncrementPlayerScore(int scoreDelta) => this.SetPlayerScore(this.playerScore + scoreDelta);
        /// <summary>
        /// Sets the player score to the given amount.
        /// Only has effect when called on the server.
        /// </summary>
        /// <param name="newScore">The new player score.</param>
        public void SetPlayerScore(int newScore)
        {
            if (this.isServer)
                this.playerScore = newScore;
        }
        

        /// <summary>
        /// The coroutine used to add demands to the demand queue.
        /// </summary>
        private IEnumerator Do_DemandCoroutine()
        {
            while (true)
            {
                // add random demand from demand pool
                yield return this.demandCoroutineWait;
                GameManager.UI.LevelUI.DemandQueue.AddDemand(this.demandsPool[Random.Range(0, this.demandsPool.Length)]);
            }
        }

        /// <summary>
        /// Called when the value of <see cref="PlayerScore"/> has changed on the server side.
        /// Updates the score on a client to synchronize it with the server. 
        /// Updates the score for the level finished screen.
        /// </summary>
        /// <param name="oldValue">The previous player score.</param>
        /// <param name="newValue">The new player score.</param>
        private void PlayerScore_OnChange(int oldValue, int newValue)
        {
            GameManager.UI.LevelUI.ScoreDisplay.SetScore(newValue);
            GameManager.UI.LevelFinishedUI.SetScore(newValue);
        }

        /// <summary>
        /// Starts the ShowLevelFinishedScreen method of the UIManager script.
        /// </summary>
        private void GameTimer_OnTimerFinished() {
            GameManager.UI.ShowLevelFinishedScreen();
        }
    }
}
