using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Underconnected
{
    /// <summary>
    /// Represents a recipe that contains input elements and assigns them an output(/result) element.
    /// Can be created from the Create Asset menu.
    /// </summary>
    [CreateAssetMenu(fileName = "Recipe", menuName = "UniOvercooked/Recipe", order = 0)]
    public class Recipe : ScriptableObject
    {
        [Tooltip("The input elements for this recipe.")]
        [SerializeField] Matter[] inputs;
        [Tooltip("The output element that is produced by this recipe.")]
        [SerializeField] Matter output;


        /// <summary>
        /// The input elements for this recipe.
        /// </summary>
        public Matter[] Inputs => this.inputs;
        /// <summary>
        /// The output element that is produced by this recipe.
        /// </summary>
        public Matter Output => this.output;


        /// <summary>
        /// Matches the given input elements.
        /// Tells whether the output element can be created using the given inputs.
        /// </summary>
        /// <param name="inputs">The input elements.</param>
        /// <param name="allowForeignElements">Whether elements that are not part of this recipe are allowed inside the inputs.</param>
        /// <returns>How much of this recipe matches the given input elements.</returns>
        public RecipeMatchState MatchInputs(List<Matter> inputs, bool allowForeignElements)
        {
            List<Matter> unmatchedElements = new List<Matter>(this.inputs);

            foreach (Matter givenElement in inputs)
                if (!unmatchedElements.Remove(givenElement) && !allowForeignElements)
                    return RecipeMatchState.NoMatch;

            return unmatchedElements.Count > 0 ? RecipeMatchState.PartialMatch : RecipeMatchState.FullMatch;
        }
    }
}
