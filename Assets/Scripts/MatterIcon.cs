using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;

namespace Underconnected
{
    /// <summary>
    /// Represents a matter icon UI element.
    /// </summary>
    public class MatterIcon : MonoBehaviour
    {
        [Header("Settings")]
        [SerializeField] Matter matterToDisplay = null;
        [SerializeField] int quantityToDisplay = 1;

        [Header("References")]
        [SerializeField] Image matterIcon;
        [SerializeField] GameObject quantityGameObject;
        [SerializeField] TextMeshProUGUI quantityText;


        /// <summary>
        /// The currently displayed matter.
        /// </summary>
        public Matter DisplayedMatter => this.matterToDisplay;
        /// <summary>
        /// The currently displayed quantity.
        /// </summary>
        public int DisplayedQuantity => this.quantityToDisplay;


        private void Awake() => this.SetDisplay(this.matterToDisplay, this.quantityToDisplay);


        /// <summary>
        /// Sets the matter and quantity to display.
        /// </summary>
        /// <param name="matter">The matter to display.</param>
        /// <param name="quantity">The quantity to display.</param>
        public void SetDisplay(Matter matter, int quantity)
        {
            this.matterToDisplay = matter;
            this.quantityToDisplay = quantity;

            this.matterIcon.enabled = matter != null;
            this.matterIcon.sprite = matter != null ? matter.GetIcon() : null;
            this.quantityGameObject.SetActive(quantity > 1);
            this.quantityText.text = quantity.ToString();
        }
    }
}
