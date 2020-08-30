/* Created by: SWT-P_SS20_Overcooked (Team Drai Studios) */
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;
using Mirror;
using TMPro;

namespace Underconnected
{
    /// <summary>
    /// A temporary network manager that spawns players as soon as they connect to the server.
    /// </summary>
    public class TestNetworkManager : NetworkManager
    {
        [Header("References")]
        [Tooltip("The hierarchy element to store client connection game objects in.")]
        [SerializeField] Transform clientsContainer;


        /// <summary>
        /// Contains all player clients connected to the same server.
        /// </summary>
        public List<PlayerConnection> AllClients { get; private set; }
        /// <summary>
        /// Our own client's connection to the server.
        /// </summary>
        public PlayerConnection ConnectionToServer { get; private set; }


        /// <summary>
        /// Called when a new client is registered via <see cref="RegisterClient(PlayerConnection)"/> and joins the game.
        /// The client already has the correct level loaded and is ready to spawn its level player.
        /// Parameters: The client that has joined the game.
        /// </summary>
        public event UnityAction<PlayerConnection> OnClientJoin;
        /// <summary>
        /// Called when a player client is unregistered via <see cref="UnregisterClient(PlayerConnection)"/> and leaves the game.
        /// Parameters: The client that has left the game.
        /// </summary>
        public event UnityAction<PlayerConnection> OnClientLeave;
        /// <summary>
        /// Called when all clients have loaded the requested level.
        /// Will only be fired on the server.
        /// </summary>
        public event UnityAction OnClientsLevelReady;


        /// <summary>
        /// Tells whether we are waiting for clients to load a requested level.
        /// </summary>
        private bool isWaitingForClientsToLoad;
        /// <summary>
        /// The level that is currently loaded on clients.
        /// </summary>
        private int levelOnClients;

        /// <summary>
        /// Holds the clients that have attempted to connect and are still loading.
        /// Used to store their <see cref="PlayerInfo"/> to create their <see cref="PlayerConnection"/> later.
        /// Key is the connection id (<see cref="NetworkConnection.connectionId"/>), Value is the associated player information.
        /// </summary>
        private Dictionary<int, PlayerInfo> pendingConnections;


        public override void Awake()
        {
            base.Awake();

            this.AllClients = new List<PlayerConnection>();
            this.isWaitingForClientsToLoad = false;
            this.levelOnClients = 0;
            this.pendingConnections = new Dictionary<int, PlayerInfo>();

            this.OnClientsLevelReady += this.TestNetworkManager_OnClientsLevelReady;
        }

        public override void OnStartServer()
        {
            // Register the spawnable prefabs on server
            Matter.RegisterSpawnablePrefabs();

            // Register message handlers
            NetworkServer.RegisterHandler<ConnectionRequestMessage>(this.OnConnectionRequestMessage, false);

            // Register server events
            GameManager.OnLevelLoaded += GameManager_OnLevelLoaded_Server;

            // Load the first level if no level is currently loaded (e.g. when launching from the Unity Editor while having a level scene open)
            if (GameManager.CurrentLevel == null)
                GameManager.Instance.LoadLevel(1);
        }
        public override void OnStopServer()
        {
            // Unregister server events
            GameManager.OnLevelLoaded -= GameManager_OnLevelLoaded_Server;

            if (GameManager.CurrentLevel != null)
                GameManager.Instance.UnloadCurrentLevel();

            this.pendingConnections.Clear();
        }

        public override void OnStartClient()
        {
            // Register the spawnable prefabs on client (if not registered already because we are running a server)
            if (!NetworkServer.active)
                Matter.RegisterSpawnablePrefabs();

            // Register message handlers
            NetworkClient.RegisterHandler<ServerStateMessage>(this.OnServerStateMessage, false);

            // Register client events
            GameManager.OnLevelLoaded += GameManager_OnLevelLoaded_Client;

            // Unload the currently loaded level if there is one (e.g. when launching from the Unity Editor while having a level scene open)
            // and we are not running a server.
            // The client should always load the level that is currently running on the server
            if (!NetworkServer.active)
                GameManager.Instance.UnloadCurrentLevel();
        }
        public override void OnStopClient()
        {
            // Unregister client events
            GameManager.OnLevelLoaded -= GameManager_OnLevelLoaded_Client;

            if (GameManager.CurrentLevel != null)
            {
                // Unload the level if we are not running a server.
                // Otherwise let the server handle unloading the level (see OnStopServer).
                if (!NetworkServer.active)
                    GameManager.Instance.UnloadCurrentLevel();
            }
        }
        public override void OnClientConnect(NetworkConnection conn) => conn.Send(new ConnectionRequestMessage(GameManager.PlayerInfo));

        public override void OnServerAddPlayer(NetworkConnection conn)
        {
            if (this.pendingConnections.ContainsKey(conn.connectionId))
            {
                GameObject connectionGO = GameObject.Instantiate(this.playerPrefab, this.clientsContainer);
                PlayerConnection clientConnection = connectionGO.GetComponent<PlayerConnection>();
                clientConnection.SetPlayerInfo(this.pendingConnections[conn.connectionId]);

                // Register server-side events for the connected client
                if (clientConnection != null)
                    clientConnection.OnLevelLoaded += this.PlayerConnection_OnLevelLoaded;

                this.pendingConnections.Remove(conn.connectionId);
                NetworkServer.AddPlayerForConnection(conn, connectionGO);
            }
            else
                conn.Disconnect(); // No player info was sent before requesting to add a player -> disconnect and let the client retry
        }


        /// <summary>
        /// Registers a new playerr connection for this network manager.
        /// Called when a new <see cref="PlayerConnection"/> object is spawned.
        /// Usually happens when a new player client connects to the server.
        /// Fires <see cref="OnClientJoin"/>.
        /// </summary>
        /// <param name="connection">The new connection to register.</param>
        public void RegisterClient(PlayerConnection connection)
        {
            connection.transform.SetParent(this.clientsContainer);

            if (connection.IsOwn)
                this.ConnectionToServer = connection;

            if (!this.AllClients.Contains(connection))
            {
                this.AllClients.Add(connection);
                this.OnClientJoin?.Invoke(connection);
            }
        }
        /// <summary>
        /// Unregisters a player connection.
        /// Called when a <see cref="PlayerConnection"/> object is destroyed.
        /// Usually happens when a player client disconnects from the server.
        /// Fires <see cref="OnClientLeave"/>.
        /// </summary>
        /// <param name="connection">The connection to unregister.</param>
        public void UnregisterClient(PlayerConnection connection)
        {
            if (this.AllClients.Contains(connection))
            {
                this.AllClients.Remove(connection);
                this.OnClientLeave?.Invoke(connection);
            }

            if (this.isWaitingForClientsToLoad)
                this.CheckIfClientsReady();
        }

        /// <summary>
        /// Requests all the clients (apart from the local one) to load the next level.
        /// Only has effect when called on the server.
        /// </summary>
        public void RequestChangeToNextLevel() => this.RequestLevelChange(GameManager.CurrentLevelNum + 1);
        /// <summary>
        /// Requests all the clients (apart from the local one) to load the level with the given number.
        /// Only has effect when called on the server.
        /// </summary>
        /// <param name="levelNum">The level number to load on the clients.</param>
        public void RequestLevelChange(int levelNum)
        {
            if (NetworkServer.active)
            {
                this.levelOnClients = levelNum;
                this.isWaitingForClientsToLoad = true;

                // Instructs all connected clients to load the new level
                foreach (PlayerConnection client in this.AllClients)
                {
                    // Skip our own local client to prevent double-loading the level
                    // The level will be loaded on the server and the local client after all remote clients have finished loading their level
                    if (client.IsOwn)
                        continue;

                    client.RequestLoadLevel(levelNum);
                }

                // See if all the clients are already ready
                // This is the case when we only have the local player connected, since the local player's level loading is handled by the server itself
                this.CheckIfClientsReady();
            }
        }


        /// <summary>
        /// Checks if all the player clients have finished loading and are ready to start the level.
        /// Fires <see cref="OnClientsLevelReady"/> when all clients are ready.
        /// </summary>
        private void CheckIfClientsReady()
        {
            if (NetworkServer.active && this.isWaitingForClientsToLoad)
            {
                foreach (PlayerConnection client in this.AllClients)
                    if (!client.IsOwn && !client.HasFinishedLoading)
                        return;

                this.isWaitingForClientsToLoad = false;
                this.OnClientsLevelReady?.Invoke();
            }
        }


        /// <summary>
        /// Called when all clients have finished loading the requested level.
        /// Loads the level on the server side and prepares to start it.
        /// Will only be called on the server since <see cref="OnClientsLevelReady"/> will only be fired on the server side.
        /// </summary>
        private void TestNetworkManager_OnClientsLevelReady() => GameManager.Instance.LoadLevel(this.levelOnClients);

        /// <summary>
        /// Called when a level has been loaded on the server.
        /// </summary>
        /// <param name="levelNum">The level number that has been loaded.</param>
        /// <param name="level">The level object that has been loaded.</param>
        private void GameManager_OnLevelLoaded_Server(int levelNum, Level level) => level.StartLevel();
        /// <summary>
        /// Called when a level has been loaded on the client.
        /// If the client is already connected to the server, it just notifies the server that the level has been loaded.
        /// If not, it requests to add its player object by calling <see cref="ClientScene.AddPlayer(NetworkConnection)"/> to finish connecting.
        /// </summary>
        /// <param name="levelNum">The level number that has been loaded.</param>
        /// <param name="level">The level object that has been loaded.</param>
        private void GameManager_OnLevelLoaded_Client(int levelNum, Level level)
        {
            if (this.ConnectionToServer != null)
                this.ConnectionToServer.ConfirmLevelLoaded(levelNum);
            else
                ClientScene.AddPlayer(NetworkClient.connection);
        }

        /// <summary>
        /// Called when a player client has finished loading a level.
        /// Calls <see cref="CheckIfClientsReady"/> to check if all clients are ready and the server can start the level.
        /// </summary>
        /// <param name="connection">The client that has finished loading a requested level.</param>
        private void PlayerConnection_OnLevelLoaded(PlayerConnection connection) => this.CheckIfClientsReady();


        private void OnConnectionRequestMessage(NetworkConnection connection, ConnectionRequestMessage message)
        {
            if (this.pendingConnections.ContainsKey(connection.connectionId))
                this.pendingConnections.Remove(connection.connectionId);

            this.pendingConnections.Add(connection.connectionId, message.PlayerInfo);
            connection.Send(new ServerStateMessage(GameManager.CurrentLevelNum));
        }

        /// <summary>
        /// Called when the client receives a <see cref="ServerStateMessage"/>.
        /// Usually called when the client attempts to connect to the server and it responds with
        /// its current state so the client can load the correct level before fully connecting.
        /// </summary>
        /// <param name="connection">The connection that has sent this message.</param>
        /// <param name="message">The message that was sent by <paramref name="connection"/>.</param>
        private void OnServerStateMessage(NetworkConnection connection, ServerStateMessage message)
        {
            // Load the level that is currently running on the server but only if we are not the local client
            // to prevent double-loading the same level.
            // If we are the local client, just finish connecting by requesting to add a player object for
            // our connection.
            if (!NetworkServer.active)
                GameManager.Instance.LoadLevel(message.LevelNum);
            else
                ClientScene.AddPlayer(connection);
        }
    }
}
