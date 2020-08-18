using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Mirror;

namespace Underconnected
{
    /// <summary>
    /// Manages all functionalities concerning the queue of demanded matters like adding, removing and delivering.
    /// </summary>
    public class DemandQueue : NetworkBehaviour
    {
        [SerializeField] GameObject queueElementsContainer;
        [SerializeField] GameObject demandedMatterUIPrefab;

        /// <summary>
        /// List of matter demands
        /// </summary>
        private List<Demand> currentDemands;


        private void Awake()
        {
            this.currentDemands = new List<Demand>();
        }

        /// <summary>
        /// Adds a matter to demand queue with <see cref="RpcAcceptDemand(string)"/> for server
        /// or <see cref="AcceptDemand(Matter)"/> for clients.
        /// </summary>
        /// <param name="matter">The matter to be added to demand queue.</param>
        /// <param name="timeLimit">The time limit for the demand.</param>
        public void AddDemand(Matter matter, float timeLimit)
        {
            if (this.isServer)
                this.RpcAcceptDemand(matter.GetID(), timeLimit);
            else
                this.AcceptDemand(matter, timeLimit);
        }
        /// <summary>
        /// Checks if given matter is currently demanded and 
        /// if so removes matter from queue with <see cref="RpcRemoveDemand(string)"/> for server
        /// and <see cref="RemoveDemand(Matter)"/> for clients.
        /// </summary>
        /// <param name="matter"></param>
        public void DeliverDemand(Matter matter)
        {
            if (this.HasDemand(matter))
            {
                if (this.isServer)
                    this.RpcRemoveDemand(matter.GetID());
                else
                    this.RemoveDemand(matter);
            }
        }
        /// <summary>
        /// Checks if given matter is currently demanded.
        /// </summary>
        /// <param name="matter">given matter to be checked</param>
        /// <returns>true if given matter is demanded or false if not</returns>
        public bool HasDemand(Matter matter)
        {
            if (matter != null)
            {
                foreach (Demand demand in this.currentDemands)
                    if (demand.demandedMatter.Equals(matter))
                        return true;
            }

            return false;
        }

        /// <summary>
        /// Creates an UI element for demanded matter and adds the same matter to the current demand queue object.
        /// </summary>
        /// <param name="matter">The matter to be added to the demand queue UI element and object.</param>
        /// <param name="timeLimit">The time limit for the new demand.</param>
        private void AcceptDemand(Matter matter, float timeLimit)
        {
            GameObject uiElement = GameObject.Instantiate(this.demandedMatterUIPrefab, Vector3.zero, Quaternion.identity, this.queueElementsContainer.transform);
            DemandedMatterUI demandedMatterUI = uiElement.GetComponent<DemandedMatterUI>();

            demandedMatterUI?.SetMatter(matter);
            demandedMatterUI?.SetTimeLimit(timeLimit);
            this.currentDemands.Add(new Demand(matter, demandedMatterUI));
        }
        /// <summary>
        /// Is only called after checking if given matter is currently demanded.
        /// Iterates through queue and removes first instance of given matter
        /// from queue and UI element.
        /// </summary>
        /// <param name="matter">matter to be removed from demand queue</param>
        private void RemoveDemand(Matter matter)
        {
            for (int i = 0; i < this.currentDemands.Count; i++)
            {
                if (this.currentDemands[i].demandedMatter.Equals(matter))
                {
                    this.currentDemands[i].uiElement.Remove();
                    this.currentDemands.RemoveAt(i);
                    break;
                }
            }
        }


        [ClientRpc]
        /// <summary>
        /// Is only called by server.
        /// Checks if a matter with given ID exists and then calls <see cref="AcceptDemand(Matter)"/>.
        /// </summary>
        /// <param name="matterID">The ID of the matter to add to queue.</param>
        /// <param name="timeLimit">The time limit for the demand.</param>
        private void RpcAcceptDemand(string matterID, float timeLimit)
        {
            Matter targetMatter = Matter.GetByID(matterID);
            if (targetMatter != null)
                this.AcceptDemand(targetMatter, timeLimit);
            else
                Debug.LogError($"Cannot accept nonexisting matter with id '{matterID}'.");
        }
        [ClientRpc]
        /// <summary>
        /// Is only called by server and after checking if given matter is currently demanded.
        /// Checks if a matter with given ID exists and then calls <see cref="RemoveDemand(Matter)"/>.
        /// </summary>
        /// <param name="matterID">id of matter to be removed from queue</param>
        private void RpcRemoveDemand(string matterID)
        {
            Matter targetMatter = Matter.GetByID(matterID);
            if (targetMatter != null)
                this.RemoveDemand(targetMatter);
            else
                Debug.LogError($"Cannot remove nonexisting matter with id '{matterID}'.");
        }


        /// <summary>
        /// A Demand struct which contains the demanded matter and its uiElement.
        /// </summary>
        public struct Demand
        {
            /// <summary>
            /// The demanded matter.
            /// </summary>
            public Matter demandedMatter;
            /// <summary>
            /// The corresponding UI element inside of the demand queue.
            /// </summary>
            public DemandedMatterUI uiElement;


            public Demand(Matter demandedMatter, DemandedMatterUI uiElement)
            {
                this.demandedMatter = demandedMatter;
                this.uiElement = uiElement;
            }
        }
    }
}
