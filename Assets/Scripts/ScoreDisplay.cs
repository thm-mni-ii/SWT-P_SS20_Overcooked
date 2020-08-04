using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;

namespace Underconnected
{
    public class ScoreDisplay : MonoBehaviour
    {
        [SerializeField] TextMeshProUGUI label;


        public void SetScore(int score)
        {
            this.label.text = score.ToString();
        }
    }
}
