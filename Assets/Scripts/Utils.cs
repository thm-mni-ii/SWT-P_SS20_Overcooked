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
            /* Source: https://en.wikipedia.org/wiki/Hiragana */

            /* Monographs */
            "a", "e", "i", "o", "u",
            "ka", "ke", "ki", "ko", "ku",
            "ga", "ge", "gi", "go", "gu",
            "sa", "se", "shi", "so", "su",
            "za", "ze", "ji", "zo", "zu",
            "ta", "te", "chi", "to", "tsu",
            "da", "de", "do",
            "na", "ne", "ni", "no", "nu",
            "ha", "he", "hi", "ho", "fu",
            "ba", "be", "bi", "bo", "bu",
            "pa", "pe", "pi", "po", "pu",
            "ma", "me", "mi", "mo", "mu",
            "ya", "yo", "yu",
            "ra", "re", "ri", "ro", "ru",
            "wa", "n",

            /* Digraphs */
            "kya", "kyu", "kyo",
            "gya", "gyu", "gyo",
            "sha", "shu", "sho",
            "ja", "ju", "jo",
            "cha", "chu", "cho",
            "nya", "nyu", "nyo",
            "hya", "hyu", "hyo",
            "bya", "byu", "byo",
            "pya", "pyu", "pyo",
            "mya", "myu", "myo",
            "rya", "ryu", "ryo"
        };


        /// <summary>
        /// Generates a random name with the given minimum length.
        /// </summary>
        /// <param name="minLength">The minimum length the result name should have.</param>
        /// <returns>The generated name.</returns>
        public static string GetRandomPlayerName(int minLength)
        {
            StringBuilder playerName = new StringBuilder();
            int lastSyllable = -1;
            int currentSyllable = 0;

            while (playerName.Length < minLength)
            {
                currentSyllable = Random.Range(0, RANDOM_SYLLABLES.Length);

                if (currentSyllable != lastSyllable)
                {
                    playerName.Append(RANDOM_SYLLABLES[currentSyllable]);
                    lastSyllable = currentSyllable;
                }
            }

            if (playerName.Length > 0)
                playerName[0] = char.ToUpper(playerName[0]);

            return playerName.ToString();
        }
    }
}
