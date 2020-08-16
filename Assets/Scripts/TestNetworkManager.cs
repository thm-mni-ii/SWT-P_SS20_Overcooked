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
        /// Contains all clients connected to the same server.
        /// </summary>
        public List<ClientConnection> AllClients { get; private set; }
        /// <summary>
        /// Own client's connection to the server.
        /// </summary>
        public ClientConnection ConnectionToServer { get; private set; }


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


        public override void Awake()
        {
            base.Awake();

            this.AllClients = new List<ClientConnection>();
            this.isWaitingForClientsToLoad = false;
            this.levelOnClients = 0;
            this.OnClientsLevelReady += this.TestNetworkManager_OnClientsLevelReady;
        }

        public override void OnStartServer()
        {
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
            GameManager.Instance.UnloadCurrentLevel();
        }

        public override void OnStartClient()
        {
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
            GameManager.Instance.UnloadCurrentLevel();
        }

        public override void OnServerConnect(NetworkConnection conn) => conn.Send(new ServerStateMessage(GameManager.CurrentLevelNum));
        public override void OnServerAddPlayer(NetworkConnection conn)
        {
            GameObject connectionGO = GameObject.Instantiate(this.playerPrefab, this.clientsContainer);
            ClientConnection clientConnection = connectionGO.GetComponent<ClientConnection>();

            // Register server-side events for the connected client
            if (clientConnection != null)
            {
                clientConnection.OnLevelLoaded += this.ClientConnection_OnLevelLoaded;
            }

            NetworkServer.AddPlayerForConnection(conn, connectionGO);
        }


        /// <summary>
        /// Registers a new client connection for this network manager.
        /// Called when a new <see cref="ClientConnection"/> object is spawned.
        /// Usually happens when a new client connects to the server.
        /// </summary>
        /// <param name="connection">The new connection to register.</param>
        public void RegisterClient(ClientConnection connection)
        {
            connection.transform.SetParent(this.clientsContainer);

            if (connection.IsOwnConnection)
                this.ConnectionToServer = connection;

            if (!this.AllClients.Contains(connection))
                this.AllClients.Add(connection);
        }
        /// <summary>
        /// Unregisters a client connection.
        /// Called when a <see cref="ClientConnection"/> object is destroyed.
        /// Usually happens when a client disconnects from the server.
        /// </summary>
        /// <param name="connection">The connection to unregister.</param>
        public void UnregisterClient(ClientConnection connection)
        {
            this.AllClients.Remove(connection);

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
                foreach (ClientConnection client in this.AllClients)
                {
                    // Skip our own local client to prevent double-loading the level
                    // The level will be loaded on the server and the local client after all remote clients have finished loading their level
                    if (client.IsOwnConnection)
                        continue;

                    client.RequestLoadLevel(levelNum);
                }
            }
        }


        /// <summary>
        /// Checks if all the clients have finished loading and are ready to start the level.
        /// Fires <see cref="OnClientsLevelReady"/> when all clients are ready.
        /// </summary>
        private void CheckIfClientsReady()
        {
            if (NetworkServer.active && this.isWaitingForClientsToLoad)
            {
                foreach (ClientConnection client in this.AllClients)
                    if (!client.HasFinishedLoading)
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
        private void GameManager_OnLevelLoaded_Server(int levelNum, Level level) { }
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
        /// Called when a client has finished loading a level.
        /// Calls <see cref="CheckIfClientsReady"/> to check if all clients are ready and the server can start the level.
        /// </summary>
        /// <param name="connection">The client that has finished loading a requested level.</param>
        private void ClientConnection_OnLevelLoaded(ClientConnection connection) => this.CheckIfClientsReady();

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
