using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DemandQueue : MonoBehaviour
{
    [SerializeField] GameObject queueElementsContainer;
    [SerializeField] GameObject demandedRecipeUIPrefab;


    public void AddDemand(Recipe recipe)
    {
        GameObject uiElement = GameObject.Instantiate(this.demandedRecipeUIPrefab, Vector3.zero, Quaternion.identity, this.queueElementsContainer.transform);
        DemandedRecipeUI demandedRecipeUI = uiElement.GetComponent<DemandedRecipeUI>();

        demandedRecipeUI?.SetRecipe(recipe);
    }
}
