using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// Represents a single element.
/// </summary>
[CreateAssetMenu(fileName = "Element", menuName = "UniOvercooked/Matter/Element", order = 0)]
public class MatterElement : Matter
{
    [Tooltip("The element symbol for this element.")]
    [SerializeField] string elementSymbol;


    public override string GetFormula() => this.elementSymbol;
    public override bool IsMolecule() => false;
    public override bool IsCompound() => false;
}
