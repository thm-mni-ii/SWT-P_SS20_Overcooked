using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class DemandedMatterUI : MonoBehaviour
{
    [Header("References")]
    [SerializeField] Image iconUI;
    [SerializeField] TextMeshProUGUI quantityText;
    [SerializeField] GameObject requiredMatterContainer;
    [SerializeField] GameObject demandedMatterUIPrefab;


    private Matter matter;


    private void Awake()
    {
        this.SetMatter(null);
    }


    public void SetMatter(Matter matter, int quantity = 1, bool showRequiredComponents = true)
    {
        this.matter = matter;

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

                foreach (MatterCompound.MatterComponent component in ((MatterCompound)matter).GetRequiredComponents())
                {
                    GameObject uiElement = GameObject.Instantiate(this.demandedMatterUIPrefab, Vector3.zero, Quaternion.identity, this.requiredMatterContainer.transform);
                    DemandedMatterUI demandedMatterUI = uiElement.GetComponent<DemandedMatterUI>();

                    demandedMatterUI?.SetMatter(component.Matter, component.Amount, false);
                }
            }
            else
                this.requiredMatterContainer.SetActive(false);
        }
        else
            this.iconUI.enabled = false;
    }
}
