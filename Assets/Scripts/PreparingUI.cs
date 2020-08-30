using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;

namespace Underconnected
{
    /// <summary>
    /// Represents a UI that shows "READY?" during the preparing phase.
    /// </summary>
    public class PreparingUI : MonoBehaviour
    {
        [Header("References")]
        [SerializeField] TextMeshProUGUI label;
        [SerializeField] Animator animator;

        [Header("Settings")]
        [SerializeField] string readyText = "READY?";
        [SerializeField] string goText = "GO!";
        [Space(2.0F)]
        [SerializeField] string readyAnimation = "ready";
        [SerializeField] string numberPopAnimation = "pop";
        [SerializeField] string goAnimation = "go";
        [SerializeField] string hiddenAnimation = "hidden";


        /// <summary>
        /// Shows the "READY?" message.
        /// </summary>
        public void ShowReady()
        {
            this.animator.Play(this.readyAnimation);
            this.label.text = this.readyText;
        }
        /// <summary>
        /// Shows the given number.
        /// </summary>
        /// <param name="number">The number to show.</param>
        public void ShowCountdownNumber(int number)
        {
            this.animator.Play(this.numberPopAnimation);
            this.label.text = number.ToString();
        }
        /// <summary>
        /// Shows the "GO!" message.
        /// </summary>
        public void ShowGo()
        {
            this.animator.Play(this.goAnimation);
            this.label.text = this.goText;
        }

        /// <summary>
        /// Hides this UI element.
        /// </summary>
        public void Hide()
        {
            this.animator.Play(this.hiddenAnimation);
            this.label.text = string.Empty;
        }
    }
}
