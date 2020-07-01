using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class DemandedRecipeUI : MonoBehaviour
{
    [Header("References")]
    [SerializeField] Image iconUI;
    [SerializeField] TextMeshProUGUI quantityText;
    [SerializeField] GameObject requiredRecipesContainer;
    [SerializeField] GameObject demandedRecipeUIPrefab;


    private Recipe recipe;


    private void Awake()
    {
        this.SetRecipe(null);
    }


    public void SetRecipe(Recipe recipe, int quantity = 1, bool showRequiredComponents = true)
    {
        this.recipe = recipe;

        if (recipe != null)
        {
            this.iconUI.sprite = recipe.GetIcon();
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

            if (showRequiredComponents && recipe is RecipeCompound)
            {
                this.requiredRecipesContainer.SetActive(true);

                foreach (RecipeCompound.RecipeComponent component in ((RecipeCompound)recipe).GetRequiredComponents())
                {
                    GameObject uiElement = GameObject.Instantiate(this.demandedRecipeUIPrefab, Vector3.zero, Quaternion.identity, this.requiredRecipesContainer.transform);
                    DemandedRecipeUI demandedRecipeUI = uiElement.GetComponent<DemandedRecipeUI>();

                    demandedRecipeUI?.SetRecipe(component.Recipe, component.Amount, false);
                }
            }
            else
                this.requiredRecipesContainer.SetActive(false);
        }
        else
            this.iconUI.enabled = false;
    }
}
