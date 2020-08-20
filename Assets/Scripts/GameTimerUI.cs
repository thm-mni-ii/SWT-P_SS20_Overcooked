using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;

namespace Underconnected
{
    /// <summary>
    /// Represents the game timer UI element displaying the time left before the level is over.
    /// </summary>
    public class GameTimerUI : MonoBehaviour
    {
        [Header("References")]
        [SerializeField] TMP_Text timerText;


        /// <summary>
        /// Holds the currently displayed seconds.
        /// Used to check whether the timer has advanced and the displayed time should be changed.
        /// It is needed to prevent assigning the same value to the timer text each frame and save canvas updates and string allocation.
        /// </summary>
        private int currentlyDisplayedSeconds;


        private void Awake()
        {
            // Force updating the timer text to set it to 00:00
            this.currentlyDisplayedSeconds = -1;
            this.UpdateTimerValue(0);
        }
        private void LateUpdate()
        {
            if (GameManager.CurrentLevel != null && GameManager.CurrentLevel.HasTimer)
                this.UpdateTimerValue(Mathf.CeilToInt(GameManager.CurrentLevel.Timer.SecondsLeft));
            else
                this.UpdateTimerValue(0);
        }


        /// <summary>
        /// Converts the given seconds to 00:00 format and displays them on the game timer UI.
        /// Checks whether the given seconds value is already displayed and updates the game timer UI's text if not.
        /// </summary>
        /// <param name="seconds">The seconds value to display on the game timer UI. Negative values will be treated as `0`.</param>
        private void UpdateTimerValue(int seconds)
        {
            seconds = Mathf.Max(0, seconds);

            if (this.currentlyDisplayedSeconds != seconds)
            {
                // TODO: Move conversion to 00:00 to a Utils class?
                if (seconds >= 60)
                    this.timerText.text = $"<b>{(seconds / 60):#0}:{(seconds % 60):00}</b>";
                else
                    this.timerText.text = $"<b>{seconds}</b>";

                this.currentlyDisplayedSeconds = seconds;
            }
        }
    }
}
