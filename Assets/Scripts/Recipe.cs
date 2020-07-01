using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public abstract class Recipe : ScriptableObject
{
    [SerializeField] Sprite icon;
    [SerializeField] new string name;
    [TextArea]
    [SerializeField] string description;


    public virtual Sprite GetIcon() => icon;
    public virtual string GetFullName() => $"{this.GetName()} ({this.GetElementSymbol()})";
    public virtual string GetName() => this.name;
    public abstract string GetElementSymbol();
    public virtual string GetDescription() => this.description;
    public abstract bool IsCompound();
}
