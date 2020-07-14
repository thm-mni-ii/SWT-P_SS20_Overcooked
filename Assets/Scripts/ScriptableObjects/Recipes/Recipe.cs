using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "Recipe", menuName = "Underconnected/Recipe", order = 0)]
public class Recipe : ScriptableObject
{
    [SerializeField] Matter[] inputs;
    [SerializeField] Matter output;


    public Matter[] Inputs => this.inputs;
    public Matter Output => this.output;


    public RecipeMatchState MatchInputs(List<Matter> inputs, bool allowForeignElements)
    {
        List<Matter> unmatchedElements = new List<Matter>(this.inputs);

        foreach (Matter givenElement in inputs)
            if (!unmatchedElements.Remove(givenElement) && !allowForeignElements)
                return RecipeMatchState.NoMatch;

        return unmatchedElements.Count > 0 ? RecipeMatchState.PartialMatch : RecipeMatchState.FullMatch;
    }
}
