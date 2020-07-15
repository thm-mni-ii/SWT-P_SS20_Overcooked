using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "Element", menuName = "UniOvercooked/Matter/Element", order = 0)]
public class MatterElement : Matter
{
    [SerializeField] string elementSymbol;


    public override string GetFormula() => this.elementSymbol;
    public override bool IsMolecule() => false;
    public override bool IsCompound() => false;
}
