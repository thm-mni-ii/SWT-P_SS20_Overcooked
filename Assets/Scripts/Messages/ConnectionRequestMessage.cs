using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Mirror;

namespace Underconnected
{
    /// <summary>
    /// Sent by the client to connect to a server.
    /// Used to also transmit player information to the server.
    /// </summary>
    public struct ConnectionRequestMessage : IMessageBase
    {
        /// <summary>
        /// The information about the connecting player.
        /// </summary>
        public PlayerInfo PlayerInfo { get; private set; }


        /// <summary>
        /// Creates a new 'ConnectionRequestMessage' packet with the given player info.
        /// </summary>
        /// <param name="playerInfo">The information about the current player.</param>
        public ConnectionRequestMessage(PlayerInfo playerInfo) => this.PlayerInfo = playerInfo;


        public void Deserialize(NetworkReader reader)
        {
            this.PlayerInfo = new PlayerInfo(reader.ReadString(), reader.ReadColor());
        }
        public void Serialize(NetworkWriter writer)
        {
            writer.WriteString(this.PlayerInfo.Name);
            writer.WriteColor(this.PlayerInfo.Color);
        }
    }
}
