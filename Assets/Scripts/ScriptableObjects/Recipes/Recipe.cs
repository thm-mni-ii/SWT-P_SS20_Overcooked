﻿using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "Recipe", menuName = "UniOvercooked/Recipe", order = 0)]
public class Recipe : ScriptableObject
{
    [SerializeField] Matter[] inputs;
    [SerializeField] Matter output;


    public Matter Output => this.output;


    public RecipeMatchState MatchInputs(List<Matter> inputs)
    {
        List<Matter> unmatchedElements = new List<Matter>(this.inputs);

        foreach (Matter givenElement in inputs)
            if (!unmatchedElements.Remove(givenElement))
                return RecipeMatchState.NoMatch;

        return unmatchedElements.Count > 0 ? RecipeMatchState.PartialMatch : RecipeMatchState.FullMatch;
    }
}