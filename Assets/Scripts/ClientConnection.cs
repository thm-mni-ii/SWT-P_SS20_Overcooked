using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;
using Mirror;

namespace Underconnected
{
    /// <summary>
    /// Represents a connection between the server and a single client.
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
        /// <summary>
        /// Tells whether the requested level for this client has finished loading.
        /// </summary>
        public bool HasFinishedLoading { get; private set; }


        /// <summary>
        /// Called when the client has loaded a level.
        /// Parameter: the <see cref="ClientConnection"/> that has triggered this event.
        /// This event will only be triggered on the server.
        /// </summary>
        public event UnityAction<ClientConnection> OnLevelLoaded;


        /// <summary>
        /// Holds the level number to load for this client.
        /// </summary>
        private int levelToLoad;


        private void Awake()
        {
            this.Player = null;
            this.HasFinishedLoading = true;
        }
        private void Start() => GameManager.NetworkManager.RegisterClient(this);
        private void OnDestroy() => GameManager.NetworkManager.UnregisterClient(this);


        /// <summary>
        /// Sets the player object for this client.
        /// </summary>
        /// <param name="player">The player object to assign to this client.</param>
        public void SetPlayer(Player player) => this.Player = player;


        #region Server Side & Commands

        /// <summary>
        /// Tells this client to load the given level.
        /// Can only be called on the server.
        /// </summary>
        /// <param name="levelNum">The level number to load.</param>
        [Server]
        public void RequestLoadLevel(int levelNum)
        {
            // Setup state on the server
            this.HasFinishedLoading = false;
            this.levelToLoad = levelNum;

            // Send load level request to this client
            this.TargetLoadLevel(this.connectionToClient, levelNum);
        }

        /// <summary>
        /// Tells the server that the requested level has been loaded.
        /// Sent by the client to the server.
        /// </summary>
        /// <param name="levelNum">The level number that has been loaded.</param>
        [Command]
        private void CmdConfirmLevelLoaded(int levelNum)
        {
            // Make sure that the correct level has been loaded and fire the level loaded event
            if (levelNum == this.levelToLoad)
            {
                this.HasFinishedLoading = true;
                this.OnLevelLoaded?.Invoke(this);
            }
        }

        #endregion


        #region Client Side & ClientRPC

        /// <summary>
        /// Tells the server that the requested level has been loaded.
        /// Can only be called on the client.
        /// </summary>
        /// <param name="levelNum">The level number that has been loaded.</param>
        [Client]
        public void ConfirmLevelLoaded(int levelNum)
        {
            // Set correct state on the client side
            this.HasFinishedLoading = true;
            this.CmdConfirmLevelLoaded(levelNum);
        }

        /// <summary>
        /// Tells a client to load the level with the given number.
        /// Sent by the server to this client.
        /// </summary>
        /// <param name="target">This connection's <see cref="NetworkConnection"/>.</param>
        /// <param name="levelNum">The level number to load.</param>
        [TargetRpc]
        private void TargetLoadLevel(NetworkConnection target, int levelNum)
        {
            // Setup state on the client
            this.HasFinishedLoading = false;
            this.levelToLoad = levelNum;

            // Load the level
            GameManager.Instance.LoadLevel(levelNum);
        }

        #endregion
    }
}
