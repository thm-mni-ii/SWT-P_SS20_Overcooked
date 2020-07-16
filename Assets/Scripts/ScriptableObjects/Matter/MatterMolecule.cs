using System.Collections;
using System.Collections.Generic;
using System.Text;
using UnityEngine;

[CreateAssetMenu(fileName = "Molecule", menuName = "Underconnected/Matter/Molecule", order = 0)]
public class MatterMolecule : Matter
{
    [SerializeField] MatterElement element;
    [SerializeField] int elementalAmount;
    [SerializeField] string formula;


    public MatterElement Element => this.element;
    public int ElementalAmount => this.elementalAmount;


    public override string GetFormula() => this.formula;
    /*{
        if (this.element != null)
            return $"{this.element.GetFormula()}_{this.elementalAmount}";

        return string.Empty;
    }*/
    public override bool IsMolecule() => true;
    public override bool IsCompound() => false;
}
