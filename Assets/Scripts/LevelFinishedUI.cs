using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;

namespace Underconnected
{
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

        public void SetPoints(int numOfLevel, int deliveredPoints, int deliveredFailedPoints, int score, int highscore)
        {
            levelText.SetText("Level " + numOfLevel.ToString() + System.Environment.NewLine + "Chempleted");
            recipesDeliveredText.SetText(deliveredPoints.ToString());
            recipesFailedDeliveredText.SetText(deliveredFailedPoints.ToString());
            pointsText.SetText(score.ToString());
            highscoreText.SetText(highscore.ToString());
        }

        public void SetStars(int amountOfStars)
        {
            for (int i = 0; i < stars.Length; i++)
            {
                stars[i].sprite = i < amountOfStars ? filledStar : emptyStar;
            }
        }

        public void Close()
        {
            this.gameObject.SetActive(false);
        }
    }
}
