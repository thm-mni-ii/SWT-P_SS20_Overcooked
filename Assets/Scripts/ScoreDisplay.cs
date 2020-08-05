using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;

namespace Underconnected
{
    /// <summary>
    /// Sets current score as label text
    /// </summary>
    public class ScoreDisplay : MonoBehaviour
    {
        [SerializeField] TextMeshProUGUI label;

        /// <summary>
        /// Casts given int to String and sets it as label text
        /// </summary>
        /// <param name="score">current score as int</param>
        public void SetScore(int score)
        {
            this.label.text = score.ToString();
        }
    }
}
