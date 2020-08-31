/* Created by: SWT-P_SS20_Overcooked (Team Drai Studios) */
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Mirror;

namespace Underconnected
{
    /// <summary>
    /// Sent by the server to a connecting client to inform it about the current game state on the server.
    /// </summary>
    public struct ServerStateMessage : IMessageBase
    {
        /// <summary>
        /// The current level number that is being played on the server.
        /// </summary>
        public int LevelNum { get; private set; }


        /// <summary>
        /// Creates a new 'CurrentLevelMessage' packet with the given level number.
        /// </summary>
        /// <param name="levelNum">The level number that is currently being played on the server.</param>
        public ServerStateMessage(int levelNum) => this.LevelNum = levelNum;


        public void Deserialize(NetworkReader reader)
        {
            this.LevelNum = reader.ReadInt32();
        }
        public void Serialize(NetworkWriter writer)
        {
            writer.WriteInt32(this.LevelNum);
        }
    }
}
