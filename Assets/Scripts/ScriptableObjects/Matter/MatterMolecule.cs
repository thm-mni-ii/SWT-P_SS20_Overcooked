/* Created by: SWT-P_SS20_Overcooked (Team Drai Studios) */
using System.Collections;
using System.Collections.Generic;
using System.Text;
using UnityEngine;

namespace Underconnected
{
    /// <summary>
    /// Represents a molecule.
    /// </summary>
    [CreateAssetMenu(fileName = "Molecule", menuName = "Underconnected/Matter/Molecule", order = 0)]
    public class MatterMolecule : Matter
    {
        [Tooltip("The element this molecule consists of.")]
        [SerializeField] MatterElement element;
        [Tooltip("The amount of elements this molecule consists of.")]
        [SerializeField] int elementalAmount;
        [Tooltip("This matter's formula.")]
        [SerializeField] string formula;


        /// <summary>
        /// The element this molecule consists of.
        /// </summary>
        public MatterElement Element => this.element;
        /// <summary>
        /// The amount of <see cref="Element"/>s this molecule consists of.
        /// </summary>
        public int ElementalAmount => this.elementalAmount;


        public override string GetFormula() => this.formula;
        public override bool IsMolecule() => true;
        public override bool IsCompound() => false;
    }
}
