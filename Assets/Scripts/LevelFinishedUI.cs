using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;

namespace Underconnected
{
    /// <summary>
    /// Represents the "Level Finished" screen that is shown after a level is completed.
    /// </summary>
    public class LevelFinishedUI : MonoBehaviour
    {
        [SerializeField] TextMeshProUGUI levelText;
        [SerializeField] Image[] stars;
        [SerializeField] Sprite emptyStar;
        [SerializeField] Sprite filledStar;
        [SerializeField] TextMeshProUGUI recipesDeliveredText;
        [SerializeField] TextMeshProUGUI recipesFailedDeliveredText;
        [SerializeField] TextMeshProUGUI pointsText;
        [SerializeField] TextMeshProUGUI highscoreText;


        /// <summary>
        /// Sets the values to display on this screen.
        /// </summary>
        /// <param name="numOfLevel">The number of the finished level.</param>
        /// <param name="deliveredPoints">The amount of delivered recipes.</param>
        /// <param name="deliveredFailedPoints">The amount of failed recipes.</param>
        /// <param name="score">The player score.</param>
        /// <param name="highscore">The highscore for the finished level.</param>
        public void SetPoints(int numOfLevel, int deliveredPoints, int deliveredFailedPoints, int score, int highscore)
        {
            levelText.SetText("Level " + numOfLevel.ToString() + System.Environment.NewLine + "Chempleted");
            recipesDeliveredText.SetText(deliveredPoints.ToString());
            recipesFailedDeliveredText.SetText(deliveredFailedPoints.ToString());
            pointsText.SetText(score.ToString());
            highscoreText.SetText(highscore.ToString());
        }

        /// <summary>
        /// Sets the amount of reached stars to display.
        /// </summary>
        /// <param name="amountOfStars">The amount of stars.</param>
        public void SetStars(int amountOfStars)
        {
            for (int i = 0; i < stars.Length; i++)
            {
                stars[i].sprite = i < amountOfStars ? filledStar : emptyStar;
            }
        }

        /// <summary>
        /// Hides this screen.
        /// </summary>
        public void Close()
        {
            this.gameObject.SetActive(false);
        }
    }
}
