/* Created by: SWT-P_SS20_Overcooked (Team Drai Studios) */
using System.Collections;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using UnityEngine;
using UnityEngine.Events;
using Mirror;

namespace Underconnected
{
    /// <summary>
    /// Represents a game level.
    /// </summary>
    public partial class Level : NetworkBehaviour
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

        [SerializeField] public bool isTutorial = false;


        /// <summary>
        /// Holds the score the players currently have.
        /// </summary>
        public int PlayerScore => this.playerScore;
        /// <summary>
        /// Returns an immutable list of all <see cref="Player"/> objects inside of this level.
        /// </summary>
        public ReadOnlyCollection<Player> AllPlayers => this.allPlayers.AsReadOnly();
        /// <summary>
        /// Holds our own player.
        /// </summary>
        public Player OwnPlayer { get; private set; }

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
        /// The current amount of correctly delivered recipes.
        /// Synchronized with the clients through <see cref="SyncVarAttribute"/> and thus can only be changed on the server.
        /// </summary>
        [SyncVar(hook = nameof(DeliveredCounter_OnChange))] int deliveredCounter;
        /// <summary>
        /// The current score that the players lose by not delivering recipes.
        /// Synchronized with the clients through <see cref="SyncVarAttribute"/> and thus can only be changed on the server.
        /// </summary>
        [SyncVar(hook = nameof(DeliveredFailedScore_OnChange))] int deliveredFailedScore;
        /// <summary>
        /// The current amount of not delivered recipes.
        /// Synchronized with the clients through <see cref="SyncVarAttribute"/> and thus can only be changed on the server.
        /// </summary>
        [SyncVar(hook = nameof(DeliveredFailedCounter_OnChange))] int deliveredFailedCounter;

        /// <summary>
        /// Holds the phase this level is currently in.
        /// </summary>
        private StateMachine<LevelPhase> levelPhase;

        /// <summary>
        /// Holds a list of all players inside of this level.
        /// </summary>
        private List<Player> allPlayers;


        /// <summary>
        /// Called when a player is registered on this level.
        /// </summary>
        public event UnityAction<Player> OnPlayerRegistered;
        /// <summary>
        /// Called when a player is unregistered from this level.
        /// </summary>
        public event UnityAction<Player> OnPlayerUnregistered;


        private void Awake()
        {
            this.playerScore = 0;
            this.deliveredCounter = 0;
            this.deliveredScore = 0;
            this.deliveredFailedCounter = 0;
            this.deliveredFailedScore = 0;

            this.allPlayers = new List<Player>();

            this.levelPhase = new StateMachine<LevelPhase>();
            this.levelPhase.RegisterState(new LevelInitPhase());
            this.levelPhase.RegisterState(new LevelPreparingPhase(this));
            this.levelPhase.RegisterState(new LevelPlayingPhase(this));
            this.levelPhase.RegisterState(new LevelFinishedPhase(this));
            this.levelPhase.SetState(LevelPhase.Init);
        }

        private void Update() => this.levelPhase.Update(Time.deltaTime);
        private void FixedUpdate() => this.levelPhase.FixedUpdate(Time.fixedDeltaTime);
        private void LateUpdate() => this.levelPhase.LateUpdate(Time.deltaTime);

        /// <summary>
        /// Starts this level by putting it into its starting phase.
        /// </summary>
        public virtual void StartLevel() => this.ChangePhase(LevelPhase.Preparing);
        /// <summary>
        /// Stops and shuts down this level.
        /// </summary>
        public void Unload() => this.levelPhase.Shutdown();


        public override bool OnSerialize(NetworkWriter writer, bool initialState)
        {
            bool dataWritten = base.OnSerialize(writer, initialState);

            if (initialState)
            {
                writer.WriteByte((byte)this.levelPhase.CurrentState);
                dataWritten = true;
            }

            dataWritten |= ((LevelPhaseState)this.levelPhase.CurrentStateObject).OnSerialize(writer, initialState);

            return dataWritten;
        }
        public override void OnDeserialize(NetworkReader reader, bool initialState)
        {
            base.OnDeserialize(reader, initialState);

            if (initialState)
                this.levelPhase.SetState((LevelPhase)reader.ReadByte());

            ((LevelPhaseState)this.levelPhase.CurrentStateObject).OnDeserialize(reader, initialState);
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
                    this.IncrementDeliveredCounter();
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

                if (player.IsOwnPlayer)
                    this.OwnPlayer = player;

                this.allPlayers.Add(player);
                this.OnPlayerRegistered?.Invoke(player);
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

                if (this.OwnPlayer == player)
                    this.OwnPlayer = null;

                this.allPlayers.Remove(player);
                this.OnPlayerUnregistered?.Invoke(player);
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
                this.deliveredScore = Mathf.Max(0, newScore);
        }

        /// <summary>
        /// Increments the recipes delievered amount by one.
        /// Only has effect when called on the server.
        public void IncrementDeliveredCounter() => this.SetDeliveredCounter(this.deliveredCounter + 1);
        /// <summary>
        /// Sets the recipes delievered score to the given amount.
        /// Only has effect when called on the server.
        /// </summary>
        /// <param name="newScore">The new player amount.</param>
        public void SetDeliveredCounter(int newAmount)
        {
            if (this.isServer)
                this.deliveredCounter = Mathf.Max(0, newAmount);
        }

        /// <summary>
        /// Reduces the recipes delievered score by the given amount.
        /// Only has effect when called on the server.
        /// </summary>
        /// <param name="scoreDelta">The amount by which to decrease the player score.</param>
        public void IncrementFailedDeliveredScore(int scoreDelta) => this.SetDeliveredFailedScore(this.deliveredFailedScore + scoreDelta);
        /// <summary>
        /// Sets the recipes delievered score to the given amount.
        /// Only has effect when called on the server.
        /// </summary>
        /// <param name="newScore">The new player score.</param>
        public void SetDeliveredFailedScore(int newScore)
        {
            if (this.isServer)
                this.deliveredFailedScore = newScore;
        }

        /// <summary>
        /// Increments the recipes failed amount by one.
        /// Only has effect when called on the server.
        public void IncrementDeliveredFailedCounter() => this.SetDeliveredFailedCounter(this.deliveredFailedCounter + 1);
        /// <summary>
        /// Sets the recipes delievered score to the given amount.
        /// Only has effect when called on the server.
        /// </summary>
        /// <param name="newScore">The new player amount.</param>
        public void SetDeliveredFailedCounter(int newAmount)
        {
            if (this.isServer)
                this.deliveredFailedCounter = newAmount;
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
        /// Transitions to the given level phase and synchronizes it with the clients.
        /// Can only be called on the server.
        /// </summary>
        /// <param name="levelPhase">The level phase to transition to.</param>
        [Server]
        private void ChangePhase(LevelPhase levelPhase)
        {
            this.levelPhase.SetState(levelPhase);
            this.RpcSetPhase((byte)levelPhase);
        }


        /// <summary>
        /// Tells the client to transition to the given level phase.
        /// </summary>
        /// <param name="phaseNum">The phase number as defined in <see cref="LevelPhase"/>.</param>
        [ClientRpc]
        private void RpcSetPhase(byte phaseNum)
        {
            if (this.isClientOnly)
                this.levelPhase.SetState((LevelPhase)phaseNum);
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
        /// Called when the value of <see cref="DeliveredCounter"/> has changed on the server side.
        /// Updates the amount on a client to synchronize it with the server. 
        /// Updates the amount for the level finished screen.
        /// </summary>
        /// <param name="oldValue">The previous recipes delivered score.</param>
        /// <param name="newValue">The new recipes delivered  score.</param>
        private void DeliveredCounter_OnChange(int oldValue, int newValue)
        {
            GameManager.UI.LevelFinishedUI.SetDeliveredCounter(newValue);
        }
        /// <summary>
        /// Called when the value of <see cref="DeliveredFailedScore"/> has changed on the server side.
        /// Updates the score on a client to synchronize it with the server. 
        /// Updates the score for the level finished screen.
        /// </summary>
        /// <param name="oldValue">The previous recipes delivered score.</param>
        /// <param name="newValue">The new recipes delivered  score.</param>
        private void DeliveredFailedScore_OnChange(int oldValue, int newValue)
        {
            GameManager.UI.LevelFinishedUI.SetDeliveredFailedPoints(newValue);
        }
        /// <summary>
        /// Called when the value of <see cref="DeliveredFailedCounter"/> has changed on the server side.
        /// Updates the amount on a client to synchronize it with the server. 
        /// Updates the amount for the level finished screen.
        /// </summary>
        /// <param name="oldValue">The previous recipes delivered score.</param>
        /// <param name="newValue">The new recipes delivered  score.</param>
        private void DeliveredFailedCounter_OnChange(int oldValue, int newValue)
        {
            GameManager.UI.LevelFinishedUI.SetDeliveredFailedCounter(newValue);
        }


        /// <summary>
        /// Represents a level phase state.
        /// Offers more functionality to level phases.
        /// </summary>
        public abstract class LevelPhaseState : State<LevelPhase>
        {
            /// <summary>
            /// Serializes this level phase.
            /// </summary>
            /// <param name="writer">The writer to serialize this phase into.</param>
            /// <param name="initialState">Tells whether this is the inital state.</param>
            /// <returns>Whether data was written to <paramref name="writer"/> by calling this method.</returns>
            public virtual bool OnSerialize(NetworkWriter writer, bool initialState) => false;
            /// <summary>
            /// Deserializes this level phase.
            /// </summary>
            /// <param name="reader">The reader to deserialize this phase from.</param>
            /// <param name="initialState">Tells whether this is the initial state.</param>
            public virtual void OnDeserialize(NetworkReader reader, bool initialState) { }
        }
    }
}
