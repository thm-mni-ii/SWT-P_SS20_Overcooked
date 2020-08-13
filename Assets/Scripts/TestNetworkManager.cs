using System.Collections;
using System.Collections.Generic;
using UnityEngine;
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


        public override void Awake()
        {
            base.Awake();
            this.AllClients = new List<ClientConnection>();
        }

        public override void OnStartServer() => GameManager.Instance.LoadLevel(1);

        public override void OnClientConnect(NetworkConnection conn)
        {
            base.OnClientConnect(conn);
            conn.Send(new AddPlayerMessage());
        }
        public override void OnServerAddPlayer(NetworkConnection conn) => NetworkServer.AddPlayerForConnection(conn, GameObject.Instantiate(this.playerPrefab, this.clientsContainer));


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
        public void UnregisterClient(ClientConnection connection) => this.AllClients.Remove(connection);
    }
}
