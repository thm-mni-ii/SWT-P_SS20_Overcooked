using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Mirror;

public class DemandQueue : NetworkBehaviour
{
    [SerializeField] GameObject queueElementsContainer;
    [SerializeField] GameObject demandedRecipeUIPrefab;


    public List<Recipe> CurrentDemands => this.currentDemands;


    private List<Recipe> currentDemands;


    private void Awake()
    {
        this.currentDemands = new List<Recipe>();
    }


    public void AddDemand(Recipe recipe)
    {
        if (this.isServer)
            this.RpcAcceptDemand(recipe.GetID());
        else
            this.AcceptDemand(recipe);
    }


    private void AcceptDemand(Recipe recipe)
    {
        GameObject uiElement = GameObject.Instantiate(this.demandedRecipeUIPrefab, Vector3.zero, Quaternion.identity, this.queueElementsContainer.transform);
        DemandedRecipeUI demandedRecipeUI = uiElement.GetComponent<DemandedRecipeUI>();

        demandedRecipeUI?.SetRecipe(recipe);
        this.currentDemands.Add(recipe);
    }


    [ClientRpc]
    private void RpcAcceptDemand(string recipeID)
    {
        Recipe targetRecipe = Recipe.GetByID(recipeID);
        if (targetRecipe != null)
            this.AcceptDemand(targetRecipe);
        else
            Debug.LogError($"Cannot accept nonexisting recipe with id '{recipeID}'.");
    }
}
