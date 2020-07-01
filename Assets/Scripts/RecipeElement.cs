using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "Recipe", menuName = "UniOvercooked/Element Recipe", order = 0)]
public class RecipeElement : Recipe
{
    [SerializeField] string elementSymbol;


    public override string GetElementSymbol() => this.elementSymbol;
    public override bool IsCompound() => false;
}
