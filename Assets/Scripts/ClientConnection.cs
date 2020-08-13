using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Mirror;

namespace Underconnected
{
    /// <summary>
    /// Represents a connection between a server and a single client.
    /// </summary>
    public class ClientConnection : NetworkBehaviour
    {
        /// <summary>
        /// The player that belongs to this client connection.
        /// `null` if none exists.
        /// </summary>
        public Player Player { get; private set; }
        /// <summary>
        /// Tells whether this is our own client's connection to the server.
        /// </summary>
        public bool IsOwnConnection => this.hasAuthority;


        private void Start() => GameManager.NetworkManager.RegisterClient(this);
        private void OnDestroy() => GameManager.NetworkManager.UnregisterClient(this);
    }
}
