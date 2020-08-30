using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;
using Mirror;

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
        [SerializeField] TextMeshProUGUI recipesDeliveredCounterText;
        [SerializeField] TextMeshProUGUI recipesFailedDeliveredText;
        [SerializeField] TextMeshProUGUI recipesFailedDeliveredCounterText;
        [SerializeField] TextMeshProUGUI pointsText;
        [SerializeField] TextMeshProUGUI highscoreText;


        /// <summary>
        /// Sets the number of the level to display on this screen.
        /// Saves the number of the next level for the next level button.
        /// </summary>
        /// <param name="numOfLevel">The number of the finished level.</param>
        public void SetNumOfLevel(int numOfLevel)
        {
            levelText.SetText("LAB " + numOfLevel.ToString() + System.Environment.NewLine + "COMPLETED");
        }

        /// <summary>
        /// Sets the points of right delivered recipes and display them on this screen.
        /// </summary>
        /// <param name="deliveredPoints">The amount of points gained by delivering recipes.</param>
        public void SetDeliveredPoints(int deliveredPoints)
        {
            recipesDeliveredText.SetText(deliveredPoints.ToString());
        }

        /// <summary>
        /// Sets the amount of right delivered recipes and display them on this screen.
        /// </summary>
        /// <param name="deliveredCounter">The amount of delivered recipes.</param>

        public void SetDeliveredCounter(int deliveredCounter)
        {
            recipesDeliveredCounterText.SetText(deliveredCounter.ToString());
        }

        /// <summary>
        /// Sets the points of not delivered recipes and display them on this screen.
        /// </summary>
        /// <param name="deliveredFailedPoints">The amount of failed recipes.</param>
        public void SetDeliveredFailedPoints(int deliveredFailedPoints)
        {
            recipesFailedDeliveredText.SetText(deliveredFailedPoints.ToString());
        }

        /// <summary>
        /// Sets the amount of not delivered recipes and display them on this screen.
        /// </summary>
        /// <param name="deliveredFailedPoints">The amount of failed recipes.</param>
        public void SetDeliveredFailedCounter(int deliveredFailedCounter)
        {
            recipesFailedDeliveredCounterText.SetText(deliveredFailedCounter.ToString());
        }

        /// <summary>
        /// Sets total score to display on this screen.
        /// </summary>
        /// <param name="score">The player score.</param>
        public void SetScore(int score)
        {
            this.pointsText.text = score.ToString();
        }

        /// <summary>
        /// Sets current highscore to display on this screen.
        /// </summary>
        /// <param name="highscore">The highscore for the finished level.</param>
        public void SetHighscore(int highscore)
        {
            highscoreText.SetText(highscore.ToString());
        }


        /// <summary>
        /// Sets the amount of reached stars to display.
        /// </summary>
        /// <param name="amount">The amount of stars.</param>
        public void SetStars(int amount)
        {
            for (int i = 0; i < this.stars.Length; i++)
                this.stars[i].sprite = i < amount ? this.filledStar : this.emptyStar;
        }

        /// <summary>
        /// Hides this screen.
        /// </summary>
        public void Exit()
        {
            this.gameObject.SetActive(false);
        }
        /// <summary>
        /// Hides this screen and load the next level.
        /// </summary>
        public void NextLevel()
        {
            this.gameObject.SetActive(false);

            if (NetworkServer.active)
                GameManager.NetworkManager.RequestChangeToNextLevel();
        }
    }
}
