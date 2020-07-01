using System.Collections;
using System.Collections.Generic;
using System.Text;
using UnityEngine;

[CreateAssetMenu(fileName = "Recipe", menuName = "UniOvercooked/Compound Recipe", order = 0)]
public class RecipeCompound : Recipe
{
    [System.Serializable]
    public struct RecipeComponent
    {
        [SerializeField] private Recipe recipe;
        [SerializeField] private int amount;


        public Recipe Recipe => recipe;
        public int Amount => amount;
    }

    [SerializeField] RecipeComponent[] requiredComponents;


    public override string GetElementSymbol()
    {
        StringBuilder sb = new StringBuilder();

        foreach (RecipeComponent component in this.requiredComponents)
        {
            if (component.Amount > 1)
                sb.Append(component.Amount);
            sb.Append(component.Recipe.GetElementSymbol());
        }

        return sb.ToString();
    }
    public override bool IsCompound() => true;
    public RecipeComponent[] GetRequiredComponents() => this.requiredComponents;
}
