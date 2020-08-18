using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Mirror;

namespace Underconnected
{
    /// <summary>
    /// Will check if local player presses the 'E' key and if so calls Interact.
    /// </summary>
    public class Player : NetworkBehaviour
    {
        [SerializeField] Interactor interactor;
        [SerializeField] PlayerControls controls;


        /// <summary>
        /// The object held by this player.
        /// </summary>
        public PickableObject HeldObject => this.interactor.HeldObject;
        /// <summary>
        /// Tells if this player is holding an object.
        /// </summary>
        public bool IsHoldingObject => this.interactor.IsHoldingObject;
        /// <summary>
        /// The player connection this player belongs to.
        /// </summary>
        public PlayerConnection Client { get; private set; }


        #region Unity Callbacks

        /// <summary>
        /// Registers itself on the current level.
        /// </summary>
        private void Start()
        {
            if (GameManager.CurrentLevel != null)
            {
                GameManager.CurrentLevel.RegisterPlayer(this);
            }
            else
            {
                Debug.LogWarning($"Can't register player {this} on the current level as it does not exist! Destroying the player...", this);
                GameObject.Destroy(this.gameObject);
            }
        }
        /// <summary>
        /// Unregisters itself from the current level.
        /// </summary>
        private void OnDestroy()
        {
            if (GameManager.CurrentLevel != null)
                GameManager.CurrentLevel.UnregisterPlayer(this);
        }

        /// <summary>
        /// Will check if local player presses the 'E' key and if so calls <see cref="Interactor.Interact"/>.
        /// </summary>
        private void Update()
        {
            if (this.hasAuthority)
            {
                if (Input.GetKeyDown(KeyCode.E))
                    this.interactor.Interact();
            }
        }

        #endregion


        public override bool OnSerialize(NetworkWriter writer, bool initialState)
        {
            bool dataWritten = base.OnSerialize(writer, initialState);

            if (initialState)
            {
                writer.WriteNetworkIdentity(this.Client != null ? this.Client.GetComponent<NetworkIdentity>() : null);
                dataWritten = true;
            }

            return dataWritten;
        }
        public override void OnDeserialize(NetworkReader reader, bool initialState)
        {
            base.OnDeserialize(reader, initialState);

            if (initialState)
            {
                NetworkIdentity clientIdentity = reader.ReadNetworkIdentity();
                this.Client = clientIdentity != null ? clientIdentity.GetComponent<PlayerConnection>() : null;
            }
        }


        /// <summary>
        /// Sets the player client this player belongs to.
        /// Synchronizes the value with the clients.
        /// </summary>
        /// <param name="client">The owner client.</param>
        public void SetClient(PlayerConnection client)
        {
            this.Client = client;

            if (this.isServer)
                this.RpcSetClient(client != null ? client.GetComponent<NetworkIdentity>() : null);
        }


        /// <summary>
        /// Tells the clients to set the given player client as this player's owner.
        /// Sent by the server to all clients.
        /// </summary>
        /// <param name="clientIdentity">The <see cref="NetworkIdentity"/> of the client that owns this player.</param>
        [ClientRpc]
        private void RpcSetClient(NetworkIdentity clientIdentity)
        {
            if (this.isClientOnly)
                this.Client = clientIdentity != null ? clientIdentity.GetComponent<PlayerConnection>() : null;
        }
    }
}
