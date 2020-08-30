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
        [SerializeField] GameObject matterIconPrefab;

        [Header("Settings")]
        [SerializeField] Color timeLeftLow;
        [SerializeField] Color timeLeftHigh;



        /// <summary>
        /// The demand to display.
        /// </summary>
        public Demand Demand { get; private set; }


        private void Awake()
        {
            this.SetDemand(this.Demand);
        }
        private void Update()
        {
            if (this.Demand != null && this.Demand.HasTimeLimit)
            {
                this.timeLeftSlider.value = Mathf.Clamp01(this.Demand.TimeLeft / this.Demand.TimeLimit);
                this.timeLeftSliderFill.color = Color.Lerp(this.timeLeftLow, this.timeLeftHigh, this.timeLeftSlider.value);
            }
        }


        /// <summary>
        /// Sets the demand to display on this UI element.
        /// </summary>
        /// <param name="demand">The demand to display.</param>
        public void SetDemand(Demand demand)
        {
            this.Demand = demand;
            this.SetMatter(demand != null ? demand.Matter : null, true);

            if (demand != null && demand.HasTimeLimit)
            {
                this.timeLeftSliderFill.color = this.timeLeftHigh;
                this.timeLeftSlider.value = 1.0F;
                this.timeLeftSlider.enabled = true;
            }
            else
                this.timeLeftSlider.enabled = false;
        }
        /// <summary>
        /// Sets the matter to display on this UI element.
        /// </summary>
        /// <param name="matter">The matter to display. `null` will hide the icon.</param>
        /// <param name="showRequiredComponents">Whether to display the required components for the given <paramref name="matter"/>.</param>
        public void SetMatter(Matter matter, bool showRequiredComponents = true)
        {
            int quantity = matter is MatterMolecule ? ((MatterMolecule)matter).ElementalAmount : 1;

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
                    this.ClearRequiredComponents();
                    this.requiredMatterContainer.SetActive(true);

                    Dictionary<Matter, int> requiredMatters = new Dictionary<Matter, int>();
                    foreach (Matter component in ((MatterCompound)matter).Components)
                    {
                        if (!requiredMatters.ContainsKey(component))
                            requiredMatters.Add(component, 1);
                        else
                            requiredMatters[component]++;
                    }

                    var enumerator = requiredMatters.GetEnumerator();
                    while (enumerator.MoveNext())
                    {
                        GameObject uiElement = GameObject.Instantiate(this.matterIconPrefab, Vector3.zero, Quaternion.identity, this.requiredMatterContainer.transform);
                        MatterIcon matterIcon = uiElement.GetComponent<MatterIcon>();
                        matterIcon?.SetDisplay(enumerator.Current.Key, enumerator.Current.Value);
                    }
                }
                else
                    this.requiredMatterContainer.SetActive(false);
            }
            else
                this.iconUI.enabled = false;
        }


        /// <summary>
        /// Destroys this UI element.
        /// </summary>
        public void Remove() => GameObject.Destroy(this.gameObject);


        /// <summary>
        /// Removes all UI elements for required components.
        /// </summary>
        private void ClearRequiredComponents()
        {
            for (int i = this.requiredMatterContainer.transform.childCount - 1; i >= 0; i--)
                GameObject.Destroy(this.requiredMatterContainer.transform.GetChild(i).gameObject);
        }
    }
}
