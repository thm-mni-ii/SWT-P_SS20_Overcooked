using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

namespace Underconnected
{
    /// <summary>
    /// Represents a UI element displaying the contents of a game object that contains matter elements.
    /// </summary>
    public class ContentsUI : MonoBehaviour
    {
        [Header("References")]
        [SerializeField] MatterIcon matterIconPrefab;
        [SerializeField] GridLayoutGroup gridLayoutGroup;


        /// <summary>
        /// Holds the matter that is currently displayed.
        /// </summary>
        private Dictionary<Matter, MatterIcon> displayedMatter;


        private void Awake()
        {
            if (this.displayedMatter == null)
                this.displayedMatter = new Dictionary<Matter, MatterIcon>();

            if (this.gridLayoutGroup != null)
                this.gridLayoutGroup.cellSize = Vector2.one * ((RectTransform)this.transform).rect.height;
        }


        /// <summary>
        /// Adds the given matter to this UI.
        /// </summary>
        /// <param name="matter">The matter to add.</param>
        /// <param name="quantity">The amount of that matter to add (optional, defaults to 1).</param>
        public void AddMatter(Matter matter, int quantity = 1)
        {
            if (matter != null)
            {
                MatterIcon matterIcon = null;
                int newQuantity = quantity;

                if (!this.displayedMatter.ContainsKey(matter))
                {
                    GameObject matterIconObject = GameObject.Instantiate(this.matterIconPrefab.gameObject, this.transform);
                    matterIcon = matterIconObject.GetComponent<MatterIcon>();
                    this.displayedMatter.Add(matter, matterIcon);
                }
                else
                {
                    matterIcon = this.displayedMatter[matter];
                    newQuantity += matterIcon.DisplayedQuantity;
                }

                matterIcon.SetDisplay(matter, newQuantity);
            }
        }

        /// <summary>
        /// Removes the given matter from this UI.
        /// </summary>
        /// <param name="matter">The matter to remove.</param>
        /// <param name="quantity">The amount of that matter to remove (optional, defaults to 1).</param>
        public void RemoveMatter(Matter matter, int quantity = 1)
        {
            if (matter != null && this.displayedMatter.ContainsKey(matter))
            {
                MatterIcon matterIcon = this.displayedMatter[matter];

                if (matterIcon.DisplayedQuantity - quantity <= 0)
                {
                    GameObject.Destroy(matterIcon.gameObject);
                    this.displayedMatter.Remove(matter);
                }
                else
                    matterIcon.SetDisplay(matterIcon.DisplayedMatter, matterIcon.DisplayedQuantity - quantity);
            }
        }
    }
}
