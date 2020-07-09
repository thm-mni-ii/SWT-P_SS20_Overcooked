using System.Collections;
using System.Collections.Generic;
using System.Text;
using UnityEngine;

[CreateAssetMenu(fileName = "Compound", menuName = "UniOvercooked/Matter/Compound", order = 0)]
public class MatterCompound : Matter
{
    [System.Serializable]
    public struct MatterComponent
    {
        [SerializeField] private Matter matter;
        [SerializeField] private int amount;


        public Matter Matter => matter;
        public int Amount => amount;
    }

    [SerializeField] MatterComponent[] requiredComponents;


    public override string GetFormula()
    {
        StringBuilder sb = new StringBuilder();

        if (this.requiredComponents != null)
        {
            foreach (MatterComponent component in this.requiredComponents)
            {
                if (component.Amount > 1)
                    sb.Append(component.Amount);
                sb.Append(component.Matter.GetFormula());
            }
        }

        return sb.ToString();
    }
    public override bool IsCompound() => true;
    public MatterComponent[] GetRequiredComponents() => this.requiredComponents;
}
