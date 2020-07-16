using System.Collections;
using System.Collections.Generic;
using System.Text;
using UnityEngine;

[CreateAssetMenu(fileName = "Compound", menuName = "Underconnected/Matter/Compound", order = 0)]
public class MatterCompound : Matter
{
    [SerializeField] Matter[] components;
    [SerializeField] string formula;


    public Matter[] Components => this.components;


    public override string GetFormula() => this.formula;
    /*{
        StringBuilder sb = new StringBuilder();

        if (this.components != null)
            foreach (Matter component in this.components)
                sb.Append(component.GetFormula());

        return sb.ToString();
    }*/
    public override bool IsMolecule() => true;
    public override bool IsCompound() => true;
}
