using System.Collections;
using System.Collections.Generic;
using System.Text;
using UnityEngine;

/// <summary>
/// Represents a compound matter.
/// </summary>
[CreateAssetMenu(fileName = "Compound", menuName = "UniOvercooked/Matter/Compound", order = 0)]
public class MatterCompound : Matter
{
    [Tooltip("The matters this compound consists of.")]
    [SerializeField] Matter[] components;

    /// <summary>
    /// The matters this compound consists of.
    /// </summary>
    public Matter[] Components => this.components;


    public override string GetFormula()
    {
        StringBuilder sb = new StringBuilder();

        if (this.components != null)
            foreach (Matter component in this.components)
                sb.Append(component.GetFormula());

        return sb.ToString();
    }
    public override bool IsMolecule() => true;
    public override bool IsCompound() => true;
}
