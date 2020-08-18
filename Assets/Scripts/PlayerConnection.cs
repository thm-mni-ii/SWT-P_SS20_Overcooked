﻿using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;
using Mirror;

namespace Underconnected
{
    /// <summary>
    /// Represents a connection between the server and a single player client.
    /// </summary>
    public class PlayerConnection : NetworkBehaviour
    {
        /// <summary>
        /// Tells whether this is our own connection to the server.
        /// </summary>
        public bool IsOwn => this.hasAuthority;
        /// <summary>
        /// Tells whether the requested level for this player client has finished loading.
        /// </summary>
        public bool HasFinishedLoading { get; private set; }


        /// <summary>
        /// Called when this player client has loaded a level.
        /// Parameter: the <see cref="PlayerConnection"/> that has triggered this event.
        /// This event will only be triggered on the server.
        /// </summary>
        public event UnityAction<PlayerConnection> OnLevelLoaded;


        /// <summary>
        /// Holds the level number to load for this player client.
        /// </summary>
        private int levelToLoad;


        private void Awake()
        {
            this.HasFinishedLoading = true;
        }
        private void Start() => GameManager.NetworkManager.RegisterClient(this);
        private void OnDestroy() => GameManager.NetworkManager.UnregisterClient(this);


        #region Server Side & Commands

        /// <summary>
        /// Tells this player client to load the given level.
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
        /// Tells a player client to load the level with the given number.
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