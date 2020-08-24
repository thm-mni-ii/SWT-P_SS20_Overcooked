using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Underconnected
{
    public class DemandQueueUI : MonoBehaviour
    {
        [SerializeField] GameObject queueElementsContainer;
        [SerializeField] GameObject demandedMatterUIPrefab;


        private Dictionary<int, DemandedMatterUI> demandUIElements;


        private void Awake()
        {
            this.demandUIElements = new Dictionary<int, DemandedMatterUI>();
        }


        /// <summary>
        /// Creates a UI element for the given demand and adds it to the demand queue.
        /// </summary>
        /// <param name="demand">The demand to add a UI element for.</param>
        public void AddDemand(Demand demand)
        {
            Debug.Log($"Added demand: {demand.Matter.GetFullName()}");
            if (!this.demandUIElements.ContainsKey(demand.ID))
            {
                GameObject uiElement = GameObject.Instantiate(this.demandedMatterUIPrefab, Vector3.zero, Quaternion.identity, this.queueElementsContainer.transform);
                DemandedMatterUI demandedMatterUI = uiElement.GetComponent<DemandedMatterUI>();

                if (demandedMatterUI != null)
                {
                    demandedMatterUI.SetDemand(demand);
                    this.demandUIElements.Add(demand.ID, demandedMatterUI);
                }
                else
                    Debug.LogWarning("Demanded Matter UI prefab is missing!", this);
            }
            else
                Debug.LogWarning($"Can't add demand with ID {demand.ID} as it already exists inside the queue.");
        }
        /// <summary>
        /// Removes the UI element for the given demand from this demand queue.
        /// </summary>
        /// <param name="demand">The demand whose UI element to remoe.</param>
        public void RemoveDemand(Demand demand)
        {
            DemandedMatterUI uiElement;
            Debug.Log($"Removed demand: {demand.Matter.GetFullName()}");

            if (this.demandUIElements.TryGetValue(demand.ID, out uiElement))
            {
                uiElement.Remove();
                this.demandUIElements.Remove(demand.ID);
            }
        }

        /// <summary>
        /// Clears this demand queue and removes all the demand UI elements.
        /// </summary>
        public void Clear()
        {
            Debug.Log("Clear called");

            Dictionary<int, DemandedMatterUI>.Enumerator enumerator = this.demandUIElements.GetEnumerator();
            while (enumerator.MoveNext())
                enumerator.Current.Value.Remove();

            this.demandUIElements.Clear();
        }
    }
}
