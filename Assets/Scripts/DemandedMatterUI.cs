using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.Events;
using TMPro;

namespace Underconnected
{
    /// <summary>
    /// Represents a UI element that displays a matter icon on the <see cref="DemandQueue"/> with a quantity text.
    /// </summary>
    public class DemandedMatterUI : MonoBehaviour
    {
        [Header("References")]
        [SerializeField] Image iconUI;
        [SerializeField] TextMeshProUGUI quantityText;
        [SerializeField] Slider timeLeftSlider;
        [SerializeField] Image timeLeftSliderFill;
        [SerializeField] GameObject requiredMatterContainer;
        [SerializeField] GameObject demandedMatterUIPrefab;

        [Header("Settings")]
        [SerializeField] Color timeLeftLow;
        [SerializeField] Color timeLeftHigh;


        /// <summary>
        /// Called when the time limit for this demand has been reached.
        /// Parameters: The UI element that has triggered this event.
        /// TODO: move to own Demand class?
        /// </summary>
        public event UnityAction<DemandedMatterUI> OnExpired;


        /// <summary>
        /// The matter to display.
        /// </summary>
        public Matter Matter { get; private set; }
        /// <summary>
        /// The time left for this demand.
        /// </summary>
        public float TimeLeft { get; private set; }
        /// <summary>
        /// The time limit for this demand. 0 if infinite.
        /// </summary>
        public float TimeLimit { get; private set; }


        private void Awake()
        {
            this.SetMatter(null);
            this.SetTimeLimit(0.0F);
        }
        private void Update()
        {
            if (this.TimeLeft > 0.0F)
            {
                this.TimeLeft = Mathf.Max(this.TimeLeft - Time.deltaTime, 0.0F);
                if (this.TimeLeft <= 0.0F)
                    this.OnExpired.Invoke(this);
            }

            if (this.TimeLimit > 0.0F)
            {
                this.timeLeftSlider.value = Mathf.Clamp01(this.TimeLeft / this.TimeLimit);
                this.timeLeftSliderFill.color = Color.Lerp(this.timeLeftLow, this.timeLeftHigh, this.timeLeftSlider.value);
            }
        }


        /// <summary>
        /// Sets the matter to display on this UI element.
        /// </summary>
        /// <param name="matter">The matter to display. `null` will hide the icon.</param>
        /// <param name="showRequiredComponents">Whether to display the required components for the given <paramref name="matter"/>.</param>
        public void SetMatter(Matter matter, bool showRequiredComponents = true)
        {
            int quantity = matter is MatterMolecule ? ((MatterMolecule)matter).ElementalAmount : 1;
            this.Matter = matter;

            if (matter != null)
            {
                this.iconUI.sprite = matter.GetIcon();
                this.iconUI.enabled = true;

                if (quantity > 1)
                {
                    this.quantityText.text = quantity.ToString();
                    this.quantityText.enabled = true;
                }
                else
                {
                    this.quantityText.text = string.Empty;
                    this.quantityText.enabled = false;
                }

                if (showRequiredComponents && matter is MatterCompound)
                {
                    this.requiredMatterContainer.SetActive(true);

                    foreach (Matter component in ((MatterCompound)matter).Components)
                    {
                        GameObject uiElement = GameObject.Instantiate(this.demandedMatterUIPrefab, Vector3.zero, Quaternion.identity, this.requiredMatterContainer.transform);
                        DemandedMatterUI demandedMatterUI = uiElement.GetComponent<DemandedMatterUI>();

                        demandedMatterUI?.SetMatter(component, false);
                    }
                }
                else
                    this.requiredMatterContainer.SetActive(false);
            }
            else
                this.iconUI.enabled = false;
        }
        /// <summary>
        /// Sets the time limit for this demand.
        /// </summary>
        /// <param name="timeLimit">The time limit. 0 for infinite.</param>
        public void SetTimeLimit(float timeLimit)
        {
            this.TimeLimit = timeLimit;
            this.TimeLeft = timeLimit;

            if (timeLimit > 0.0F)
            {
                this.timeLeftSliderFill.color = this.timeLeftHigh;
                this.timeLeftSlider.value = 1.0F;
                this.timeLeftSlider.enabled = true;
            }
            else
                this.timeLeftSlider.enabled = false;
        }

        /// <summary>
        /// Destroys this UI element.
        /// </summary>
        public void Remove()
        {
            GameObject.Destroy(this.gameObject);
        }
    }
}
