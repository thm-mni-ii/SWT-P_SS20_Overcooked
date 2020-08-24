using System.Collections;
using System.Collections.Generic;
using System.Collections.ObjectModel;
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
        [SerializeField] float timePerDemand = 30.0F;
        [SerializeField] int bonusScore = 10;
        [Range(0.0F, 1.0F)]
        [SerializeField] float bonusScoreThreshold = 0.15F;
        [SerializeField] Vector2 demandSpawnTimeMinMax = new Vector2(10.0F, 30.0F);
        [SerializeField] Matter[] demandsPool;
        [SerializeField] Transform[] spawnPoints;

        [Header("References")]
        [SerializeField] LevelTimer timer;
        [SerializeField] DemandQueue demandQueue;


        /// <summary>
        /// Holds the score the players currently have.
        /// </summary>
        public int PlayerScore => this.playerScore;
        /// <summary>
        /// Returns an immutable list of all <see cref="Player"/> objects inside of this level.
        /// </summary>
        public ReadOnlyCollection<Player> AllPlayers => this.allPlayers.AsReadOnly();

        /// <summary>
        /// Holds the timer for this level.
        /// Can be `null` if this level does not have a timer.
        /// </summary>
        public LevelTimer Timer => this.timer;
        /// <summary>
        /// Tells whether this level has a <see cref="LevelTimer"/> by checking if <see cref="Timer"/> is not `null`.
        /// </summary>
        public bool HasTimer => this.Timer != null;

        /// <summary>
        /// Holds the demand queue for this level.
        /// Contains all demands that the players have to deliver in order to get score.
        /// </summary>
        public DemandQueue DemandQueue => this.demandQueue;
        /// <summary>
        /// Tells whether this level has a <see cref="DemandQueue"/> by checking if <see cref="DemandQueue"/> is not `null`.
        /// </summary>
        public bool HasDemandQueue => this.DemandQueue != null;


        /// <summary>
        /// The score the players currently have.
        /// Synchronized with the clients through <see cref="SyncVarAttribute"/> and thus can only be changed on the server.
        /// </summary>
        [SyncVar(hook = nameof(PlayerScore_OnChange))] int playerScore;
        /// <summary>
        /// The current score that the players make with correctly delivered recipes.
        /// Synchronized with the clients through <see cref="SyncVarAttribute"/> and thus can only be changed on the server.
        /// </summary>
        [SyncVar(hook = nameof(DeliveredScore_OnChange))] int deliveredScore;

        /// <summary>
        /// The coroutine that adds demands to the demands list.
        /// </summary>
        private Coroutine demandCoroutine;

        /// <summary>
        /// Holds a list of all players inside of this level.
        /// </summary>
        private List<Player> allPlayers;

        /// <summary>
        /// Tells whether this level has been set up to run on the server.
        /// </summary>
        private bool isRunningOnServer;
        /// <summary>
        /// Tells whether this level has been set up to run on the client.
        /// </summary>
        private bool isRunningOnClient;


        private void Awake()
        {
            this.playerScore = 0;
            this.deliveredScore = 0;

            this.demandCoroutine = null;

            this.allPlayers = new List<Player>();

            this.isRunningOnServer = false;
            this.isRunningOnClient = false;
        }

        public override void OnStartServer()
        {
            if (!this.isRunningOnServer)
            {
                // Setup UI and coroutines
                if (this.HasTimer)
                    this.Timer.StartTimer();

                if (this.HasDemandQueue)
                {
                    this.SubscribeDemandQueueUI();
                    this.DemandQueue.OnDemandExpired += this.DemandQueue_OnDemandExpired_Server;
                    this.demandCoroutine = this.StartCoroutine(this.Do_DemandCoroutine());
                }

                // Setup events to create/destroy a player for each joining/leaving client automatically
                GameManager.NetworkManager.OnClientJoin += this.AddPlayer;
                GameManager.NetworkManager.OnClientLeave += this.RemovePlayer;

                // Add a player for each client to this level
                this.AddAllPlayers();

                this.isRunningOnServer = true;
            }
        }
        public override void OnStartClient()
        {
            if (!this.isRunningOnClient)
            {
                if (this.HasTimer)
                    this.Timer.OnTimerFinished += this.Timer_OnTimerFinished;

                if (this.HasDemandQueue && !this.isServer /* only subscribe if we are not running a server as the server also subscribes to these events (see OnStartServer)*/)
                    this.SubscribeDemandQueueUI();

                this.isRunningOnClient = true;
            }
        }

        public void Unload()
        {
            if (this.isRunningOnServer)
            {
                // Stop level timer and coroutines
                if (this.HasTimer)
                    this.Timer.StopTimer();

                if (this.HasDemandQueue)
                {
                    this.StopCoroutine(this.demandCoroutine);
                    this.DemandQueue.OnDemandExpired -= this.DemandQueue_OnDemandExpired_Server;
                }

                // Unsubscribe from events
                GameManager.NetworkManager.OnClientJoin -= this.AddPlayer;
                GameManager.NetworkManager.OnClientLeave -= this.RemovePlayer;

                // Remove all players from this level
                this.RemoveAllPlayers();

                this.isRunningOnServer = false;
            }

            if (this.isRunningOnClient)
            {
                if (this.HasTimer)
                    this.Timer.OnTimerFinished -= this.Timer_OnTimerFinished;

                this.isRunningOnClient = false;
            }

            if (this.HasDemandQueue)
            {
                this.UnsubscribeDemandQueueUI();
                GameManager.UI.LevelUI.DemandQueue.Clear();
            }
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
                Demand deliveredDemand = this.DemandQueue.Deliver(matter);
                if (deliveredDemand != null)
                {
                    int bonusPoints = 0;
                    float timeLeftPercent = 0.0F;

                    // Calculate bonus points if the demand has a time limit and it was delivered before the threshold
                    if (deliveredDemand.HasTimeLimit)
                    {
                        timeLeftPercent = Mathf.Clamp01(deliveredDemand.TimeLeft / deliveredDemand.TimeLimit);

                        if (timeLeftPercent >= this.bonusScoreThreshold)
                            bonusPoints = Mathf.CeilToInt(((timeLeftPercent - this.bonusScoreThreshold) / (1.0F - this.bonusScoreThreshold)) * this.bonusScore);
                    }

                    this.IncrementPlayerScore(matter.GetScoreReward() + bonusPoints);
                    this.IncrementDeliveredScore(matter.GetScoreReward() + bonusPoints);
                    NetworkServer.Destroy(matterObject.gameObject);
                }
            }
        }

        /// <summary>
        /// Registers the given player on this level.
        /// Usually called when a new <see cref="Player"/> object is spawned on the server or the client.
        /// It will add the given object to <see cref="AllPlayers"/>.
        /// </summary>
        /// <param name="player">The player object to register.</param>
        public void RegisterPlayer(Player player)
        {
            if (!this.allPlayers.Contains(player))
            {
                Debug.Log($"Registered player {player}", this);
                this.allPlayers.Add(player);
            }
        }
        /// <summary>
        /// Unregisters the given player from this level.
        /// Usually called when a <see cref="Player"/> object is despawned on the server or the client.
        /// It won't destroy this object, just remove it from <see cref="AllPlayers"/>.
        /// </summary>
        /// <param name="player">The player object to unregister.</param>
        public void UnregisterPlayer(Player player)
        {
            if (this.allPlayers.Contains(player))
            {
                Debug.Log($"Unregistered player {player}", this);
                this.allPlayers.Remove(player);
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
                this.playerScore = Mathf.Max(0, newScore);
        }

        /// <summary>
        /// Increments the recipes delievered score by the given amount.
        /// Only has effect when called on the server.
        /// </summary>
        /// <param name="scoreDelta">The amount by which to increase the player score.</param>
        public void IncrementDeliveredScore(int scoreDelta) => this.SetDeliveredScore(this.deliveredScore + scoreDelta);
        /// <summary>
        /// Sets the recipes delievered score to the given amount.
        /// Only has effect when called on the server.
        /// </summary>
        /// <param name="newScore">The new player score.</param>
        public void SetDeliveredScore(int newScore)
        {
            if (this.isServer)
                this.deliveredScore = newScore;
        }


        /// <summary>
        /// The coroutine used to add demands to the demand queue.
        /// </summary>
        private IEnumerator Do_DemandCoroutine()
        {
            while (true)
            {
                // add random demand from demand pool
                yield return new WaitForSeconds(Random.Range(this.demandSpawnTimeMinMax.x, this.demandSpawnTimeMinMax.y));

                if (this.demandsPool.Length > 0)
                    this.DemandQueue.AddDemand(this.demandsPool[Random.Range(0, this.demandsPool.Length)], this.timePerDemand);
            }
        }


        /// <summary>
        /// Subscribes to the demand queue events to start updating the demand queue UI.
        /// </summary>
        private void SubscribeDemandQueueUI()
        {
            this.demandQueue.OnDemandAdded += this.DemandQueue_OnDemandAdded;
            this.demandQueue.OnDemandRemoved += this.DemandQueue_OnDemandRemoved;
        }
        /// <summary>
        /// Unsubscribes from the demand queue events to stop updating the demand queue UI.
        /// </summary>
        private void UnsubscribeDemandQueueUI()
        {
            this.demandQueue.OnDemandAdded -= this.DemandQueue_OnDemandAdded;
            this.demandQueue.OnDemandRemoved -= this.DemandQueue_OnDemandRemoved;
        }


        /// <summary>
        /// Adds a player for the given player connection.
        /// Can only be called on the server.
        /// </summary>
        /// <param name="client">The player connection to spawn a new player for.</param>
        [Server]
        private void AddPlayer(PlayerConnection client)
        {
            foreach (Player p in this.allPlayers)
                if (p.Client == client)
                    return;

            Transform spawnPos = this.GetSpawnForPlayer(this.allPlayers.Count);
            GameObject playerGO = GameObject.Instantiate(GameManager.PlayerPrefab, spawnPos.position, spawnPos.rotation, this.transform);
            Player player = playerGO.GetComponent<Player>();

            NetworkServer.Spawn(playerGO, client.connectionToClient);
            this.RegisterPlayer(player);
            player.SetClient(client);
        }
        /// <summary>
        /// Adds a player for each connected client found in <see cref="GameManager.NetworkManager"/>.
        /// Can only be called on the server.
        /// </summary>
        [Server]
        private void AddAllPlayers()
        {
            // Spawn players for all clients that are already connected
            foreach (PlayerConnection client in GameManager.NetworkManager.AllClients)
                this.AddPlayer(client);
        }
        /// <summary>
        /// Removes and destroys the player assigned to the given player connection.
        /// Can only be called on the server.
        /// </summary>
        /// <param name="client">The player client whose player to remove.</param>
        [Server]
        private void RemovePlayer(PlayerConnection client)
        {
            Player targetPlayer = null;

            foreach (Player p in this.allPlayers)
            {
                if (p.Client == client)
                {
                    targetPlayer = p;
                    break;
                }
            }

            if (targetPlayer != null)
            {
                this.UnregisterPlayer(targetPlayer);
                NetworkServer.Destroy(targetPlayer.gameObject);
            }
        }
        /// <summary>
        /// Removes and destroys all players on this level.
        /// Can only be called on the server.
        /// </summary>
        [Server]
        private void RemoveAllPlayers()
        {
            Player player;
            while (this.allPlayers.Count > 0)
            {
                player = this.allPlayers[0];
                this.UnregisterPlayer(player);
                NetworkServer.Destroy(player.gameObject);
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
            GameManager.UI.LevelFinishedUI.SetStars(newValue); //doesnt work with FailedDeliveredPoints
        }

        /// <summary>
        /// Called when the value of <see cref="DeliveredScore"/> has changed on the server side.
        /// Updates the score on a client to synchronize it with the server. 
        /// Updates the score for the level finished screen.
        /// </summary>
        /// <param name="oldValue">The previous recipes delivered score.</param>
        /// <param name="newValue">The new recipes delivered  score.</param>
        private void DeliveredScore_OnChange(int oldValue, int newValue)
        {
            GameManager.UI.LevelFinishedUI.SetDeliveredPoints(newValue);
        }

        /// <summary>
        /// Starts the ShowLevelFinishedScreen method of the UIManager script.
        /// </summary>
        private void Timer_OnTimerFinished()
        {
            GameManager.UI.ShowLevelFinishedScreen();
        }

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
        /// <param name="demand"></param>
        private void DemandQueue_OnDemandRemoved(Demand demand) => GameManager.UI.LevelUI.DemandQueue.RemoveDemand(demand);
        /// <summary>
        /// Called when a demand's time limit is reached.
        /// Applies a score penalty.
        /// </summary>
        /// <param name="demand">The expired demand.</param>
        private void DemandQueue_OnDemandExpired_Server(Demand demand)
        {
            this.IncrementPlayerScore(-demand.Matter.GetScoreFailPenalty());
            this.IncrementDeliveredScore(-demand.Matter.GetScoreFailPenalty());
        }
    }
}
