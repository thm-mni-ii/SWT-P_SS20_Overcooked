using System.Collections;
using System.Collections.Generic;
using System.Text;
using UnityEngine;

namespace Underconnected
{
    /// <summary>
    /// Contains useful methods.
    /// </summary>
    public static class Utils
    {
        /// <summary>
        /// Contains random syllables to generate random strings from.
        /// </summary>
        private static readonly string[] RANDOM_SYLLABLES = {
            "a", "e", "i", "o", "u",
            "ha", "ba", "pa",
            "ho", "bo", "po",
            "ka", "ga",
            "ko", "go",
            "ku", "gu",
            "ki", "gi",
            "ke", "ge",
            "ma", "me", "mu", "mo",
            "fu", "bu", "pu",
            "na", "no", "nu",
            "sa", "za",
            "su", "zu",
            "so", "zo",
            "ya", "yo",
            "tsu", "n",
            "shi", "ji",
            "to", "do",
            "ta", "da",
            "te", "de",
            "ra", "ru", "ri", "ro"
        };


        /// <summary>
        /// Generates a random name with the given minimum length.
        /// </summary>
        /// <param name="minLength">The minimum length the result name should have.</param>
        /// <returns>The generated name.</returns>
        public static string GetRandomPlayerName(int minLength)
        {
            StringBuilder playerName = new StringBuilder();

            while (playerName.Length < minLength)
                playerName.Append(RANDOM_SYLLABLES[Random.Range(0, RANDOM_SYLLABLES.Length)]);

            if (playerName.Length > 0)
                playerName[0] = char.ToUpper(playerName[0]);

            return playerName.ToString();
        }
    }
}
