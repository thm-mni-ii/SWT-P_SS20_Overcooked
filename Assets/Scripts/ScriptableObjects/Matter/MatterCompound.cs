using System.Collections;
using System.Collections.Generic;
using System.Text;
using UnityEngine;

namespace Underconnected
{
    /// <summary>
    /// Represents a compound matter.
    /// </summary>
    [CreateAssetMenu(fileName = "Compound", menuName = "Underconnected/Matter/Compound", order = 0)]
    public class MatterCompound : Matter
    {
        [Tooltip("The matters this compound consists of.")]
        [SerializeField] Matter[] components;
        [Tooltip("This compound's formula.")]
        [SerializeField] string formula;

        /// <summary>
        /// The matters this compound consists of.
        /// </summary>
        public Matter[] Components => this.components;


        public override string GetFormula() => this.formula;
        public override bool IsMolecule() => true;
        public override bool IsCompound() => true;
    }
}
