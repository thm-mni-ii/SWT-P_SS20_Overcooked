using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public abstract class Recipe : ScriptableObject
{
    private static Dictionary<string, Recipe> recipes = new Dictionary<string, Recipe>();

    public static Recipe GetByID(string recipeID)
    {
        Recipe target = null;

        if (recipes.TryGetValue(recipeID, out target))
            return target;

        Debug.LogError($"RecipeID '{recipeID}' not found.");
        return null;
    }

    private static void RegisterRecipe(Recipe recipe)
    {
        if (!recipes.ContainsKey(recipe.GetID()))
            recipes.Add(recipe.GetID(), recipe);
    }




    [SerializeField] Sprite icon;
    [SerializeField] new string name;
    [TextArea]
    [SerializeField] string description;
    [SerializeField] ElementObject prefab;


    private void OnEnable()
    {
        Recipe.RegisterRecipe(this);
    }


    public virtual string GetID() => this.GetElementSymbol();
    public virtual Sprite GetIcon() => icon;
    public virtual string GetFullName() => $"{this.GetName()} ({this.GetElementSymbol()})";
    public virtual string GetName() => this.name;
    public abstract string GetElementSymbol();
    public virtual string GetDescription() => this.description;
    public virtual GameObject GetPrefab() => this.prefab.gameObject;
    public abstract bool IsCompound();
}
