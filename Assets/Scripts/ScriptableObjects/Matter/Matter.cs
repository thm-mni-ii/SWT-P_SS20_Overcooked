using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public abstract class Matter : ScriptableObject
{
    private static Dictionary<string, Matter> matters = new Dictionary<string, Matter>();

    public static Matter GetByID(string matterID)
    {
        Matter target = null;

        if (matters.TryGetValue(matterID, out target))
            return target;

        Debug.LogError($"MatterID '{matterID}' not found.");
        return null;
    }

    private static void RegisterMatter(Matter matter)
    {
        if (!matters.ContainsKey(matter.GetID()))
            matters.Add(matter.GetID(), matter);
    }




    [SerializeField] Sprite icon;
    [SerializeField] new string name;
    [TextArea]
    [SerializeField] string description;
    [SerializeField] ElementObject prefab;


    private void OnEnable()
    {
        Matter.RegisterMatter(this);
    }


    public virtual string GetID() => this.GetFormula();
    public virtual Sprite GetIcon() => icon;
    public virtual string GetFullName() => $"{this.GetName()} ({this.GetFormula()})";
    public virtual string GetName() => this.name;
    public abstract string GetFormula();
    public virtual string GetDescription() => this.description;
    public virtual GameObject GetPrefab() => this.prefab.gameObject;
    public abstract bool IsCompound();
}
