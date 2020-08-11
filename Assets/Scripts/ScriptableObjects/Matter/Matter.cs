using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Underconnected
{
    /// <summary>
    /// Represents a matter that can be combined to other matters.
    /// A base class for single elements, molecules and compounds.
    /// </summary>
    public abstract class Matter : ScriptableObject
    {
        /// <summary>
        /// Contains all known matters.
        /// Used to look up a matter by its ID.
        /// </summary>
        /// <typeparam name="string">The ID that is associated with a matter.</typeparam>
        /// <typeparam name="Matter">The matter that is associated with an ID.</typeparam>
        private static Dictionary<string, Matter> matters = new Dictionary<string, Matter>();

        /// <summary>
        /// Looks up a matter by the given ID.
        /// </summary>
        /// <param name="matterID">The ID to look up.</param>
        /// <returns>The matter associated with <paramref name="matterID"/> or `null` if none found.</returns>
        public static Matter GetByID(string matterID)
        {
            Matter target = null;

            if (matters.TryGetValue(matterID, out target))
                return target;

            Debug.LogError($"MatterID '{matterID}' not found.");
            return null;
        }

        /// <summary>
        /// Registers a matter and adds it to the <see cref="matters"/> dictionary.
        /// It only adds it if no matter with the same ID had been added before.
        /// </summary>
        /// <param name="matter">The matter to register.</param>
        private static void RegisterMatter(Matter matter)
        {
            if (!matters.ContainsKey(matter.GetID()))
                matters.Add(matter.GetID(), matter);
        }




        [Tooltip("The icon to show on the demand queue, object info canvases, etc.")]
        [SerializeField] Sprite icon;
        [Tooltip("The name for this matter.")]
        [SerializeField] new string name;
        [Tooltip("The description for this matter.")]
        [TextArea]
        [SerializeField] string description;
        [Tooltip("The prefab that belongs to this matter.")]
        [SerializeField] MatterObject prefab;


        protected virtual void OnEnable()
        {
            Matter.RegisterMatter(this);
        }


        /// <summary>
        /// Returns the ID for this matter.
        /// </summary>
        /// <returns>The ID.</returns>
        public virtual string GetID() => this.GetFormula();
        /// <summary>
        /// Returns the icon to display for this matter.
        /// </summary>
        /// <returns>The icon.</returns>
        public virtual Sprite GetIcon() => icon;
        /// <summary>
        /// Returns the full name for this matter, usually consisting of the name and its formula.
        /// </summary>
        /// <returns>The full name.</returns>
        public virtual string GetFullName() => $"{this.GetName()} ({this.GetFormula()})";
        /// <summary>
        /// Returns the name for this matter.
        /// </summary>
        /// <returns>The name.</returns>
        public virtual string GetName() => this.name;
        /// <summary>
        /// Returns the formula for this matter.
        /// </summary>
        /// <returns>The secret formula.</returns>
        public abstract string GetFormula();
        /// <summary>
        /// Returns the description for this matter.
        /// </summary>
        /// <returns>The description.</returns>
        public virtual string GetDescription() => this.description;
        /// <summary>
        /// Returns the <see cref="MatterObject"/> prefab that should be spawned when this matter is materialized inside the game world.
        /// </summary>
        /// <returns>The <see cref="MatterObject"/> prefab.</returns>
        public virtual GameObject GetPrefab() => this.prefab.gameObject;
        /// <summary>
        /// Tells whether this matter is a molecule.
        /// </summary>
        /// <returns>Whether this is a molecule</returns>
        public abstract bool IsMolecule();
        /// <summary>
        /// Tells whether this matter is a compound.
        /// </summary>
        /// <returns>Whether this is a compound.</returns>
        public abstract bool IsCompound();
    }
}
