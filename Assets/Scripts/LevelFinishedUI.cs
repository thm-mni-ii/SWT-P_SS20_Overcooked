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

        private int numOfNextLevel;

        /// <summary>
        /// Sets the number of the level to display on this screen.
        /// Saves the number of the next level for the next level button.
        /// </summary>
        /// <param name="numOfLevel">The number of the finished level.</param>
        public void SetNumOfLevel(int numOfLevel) {
            levelText.SetText("Level " + numOfLevel.ToString() + System.Environment.NewLine + "Chempleted");
            numOfNextLevel = numOfLevel + 1;
        }

        /// <summary>
        /// Sets the points of right delivered recipes and display them on this screen.
        /// </summary>
        /// <param name="deliveredPoints">The amount of delivered recipes.</param>
        public void SetDeliveredPoints(int deliveredPoints) {
            recipesDeliveredText.SetText(deliveredPoints.ToString());
        }

        /// <summary>
        /// Sets the points of wrong delivered recipes and display them on this screen.
        /// </summary>
        /// <param name="deliveredFailedPoints">The amount of failed recipes.</param>
        public void SetDeliveredFailedPoints(int deliveredFailedPoints) {
            recipesFailedDeliveredText.SetText(deliveredFailedPoints.ToString());
        }

        /// <summary>
        /// Sets total score to display on this screen.
        /// </summary>
        /// <param name="score">The player score.</param>
        public void SetScore(int score) {
            this.pointsText.text = score.ToString();
        }

        /// <summary>
        /// Sets current highscore to display on this screen.
        /// </summary>
        /// <param name="highscore">The highscore for the finished level.</param>
        public void SetHighscore(int highscore) {
            highscoreText.SetText(highscore.ToString());
        }


        /// <summary>
        /// Sets the amount of reached stars to display.
        /// </summary>
        /// <param name="score">The amount of stars.</param>
        public void SetStars(int score)
        {
            int numOfStars = 0;
            if (score >= 100) { //placeholder points
                stars[0].sprite = filledStar;
                if (score >= 200) {
                    stars[1].sprite = filledStar;
                    if (score >= 300) {
                        stars[2].sprite = filledStar;
                    }
                }
            }
            /*for (int i = 0; i < stars.Length; i++)
            {
                stars[i].sprite = i < numOfStars ? filledStar : emptyStar;
            }*/
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
        public void NextLevel() {
            this.gameObject.SetActive(false);
            GameManager.Instance.LoadLevel(numOfNextLevel);
        }
    }
}
