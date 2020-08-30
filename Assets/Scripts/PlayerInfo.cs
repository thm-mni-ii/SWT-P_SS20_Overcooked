using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Underconnected
{
    /// <summary>
    /// A struct holding information for a connected player.
    /// </summary>
    [System.Serializable]
    public struct PlayerInfo
    {
        /// <summary>
        /// Creates random player information.
        /// </summary>
        /// <returns></returns>
        public static PlayerInfo Random() => new PlayerInfo(Utils.GetRandomPlayerName(UnityEngine.Random.Range(4, 9)), UnityEngine.Random.ColorHSV());



        /// <summary>
        /// Holds the name of the player.
        /// </summary>
        public string Name { get; private set; }
        /// <summary>
        /// Holds the color of the player.
        /// </summary>
        public Color Color { get; private set; }


        /// <summary>
        /// Creates new player information.
        /// </summary>
        /// <param name="name">The name of the player.</param>
        /// <param name="color">The color of the player.</param>
        public PlayerInfo(string name, Color color)
        {
            this.Name = name;
            this.Color = color;
        }
    }
}
