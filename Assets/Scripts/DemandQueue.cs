using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;
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
        /// Fired when a demand's time limit is reached and it is removed.
        /// Parameters: The expired demand's matter.
        /// </summary>
        public event UnityAction<Matter> OnDemandExpired;


        /// <summary>
        /// List of matter demands
        /// </summary>
        private List<Demand> currentDemands;


        private void Awake()
        {
            this.currentDemands = new List<Demand>();
        }


        /// <summary>
        /// Adds a matter to the demand queue with the given time limit and synchronizes it with the clients.
        /// Can only be called on the server (since demand spawn is exclusively server side).
        /// </summary>
        /// <param name="matter">The matter to add to the demand queue.</param>
        /// <param name="timeLimit">The time limit for the demand.</param>
        [Server]
        public void AddDemand(Matter matter, float timeLimit) => this.AcceptDemand(matter, timeLimit);
        /// <summary>
        /// Removes a matter from the demand queue and synchronizes it with the clients.
        /// Can only be called on the server.
        /// </summary>
        /// <param name="matter">The matter to remove from the demand queue.</param>
        /// <returns>Whether the demand queue contained the delivered matter and removed it.</returns>
        [Server]
        public bool DeliverDemand(Matter matter)
        {
            if (this.HasDemand(matter))
            {
                this.RemoveDemand(matter);
                return true;
            }

            return false;
        }
        /// <summary>
        /// Checks if given matter is currently demanded.
        /// </summary>
        /// <param name="matter">The matter to be checked</param>
        /// <returns>True if given matter is demanded or false if not</returns>
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
        /// Returns the oldest demand for the given matter.
        /// </summary>
        /// <param name="matter">The matter to find a demand for.</param>
        /// <returns>The oldest demand for <paramref name="matter"/> or `null` if none found.</returns>
        public Demand? GetDemand(Matter matter)
        {
            if (matter != null)
            {
                foreach (Demand demand in this.currentDemands)
                    if (demand.demandedMatter.Equals(matter))
                        return demand;
            }

            return null;
        }

        /// <summary>
        /// Creates a UI element for demanded matter and adds the same matter to the current demand queue object.
        /// Synchronizes it with the clients if this is called on the server.
        /// </summary>
        /// <param name="matter">The matter to be added to the demand queue UI element and object.</param>
        /// <param name="timeLimit">The time limit for the new demand.</param>
        private void AcceptDemand(Matter matter, float timeLimit)
        {
            GameObject uiElement = GameObject.Instantiate(this.demandedMatterUIPrefab, Vector3.zero, Quaternion.identity, this.queueElementsContainer.transform);
            DemandedMatterUI demandedMatterUI = uiElement.GetComponent<DemandedMatterUI>();

            if (demandedMatterUI != null)
            {
                demandedMatterUI.SetMatter(matter);
                demandedMatterUI.SetTimeLimit(timeLimit);
                this.currentDemands.Add(new Demand(matter, demandedMatterUI));

                if (this.isServer)
                {
                    demandedMatterUI.OnExpired += this.DemandedMatterUI_OnExpired_Server;
                    this.RpcAcceptDemand(matter.GetID(), timeLimit);
                }
            }
            else
                Debug.LogWarning("Demanded Matter UI prefab is missing!", this);
        }
        /// <summary>
        /// Iterates through queue and removes first instance of given matter
        /// from queue and UI element. Synchronizes it with the clients if this is called on the server.
        /// </summary>
        /// <param name="matter">The matter to be removed from demand queue.</param>
        private void RemoveDemand(Matter matter)
        {
            for (int i = 0; i < this.currentDemands.Count; i++)
            {
                if (this.currentDemands[i].demandedMatter.Equals(matter))
                {
                    if (this.isServer)
                    {
                        this.currentDemands[i].uiElement.OnExpired -= this.DemandedMatterUI_OnExpired_Server;
                        this.RpcRemoveDemand(matter.GetID());
                    }

                    this.currentDemands[i].uiElement.Remove();
                    this.currentDemands.RemoveAt(i);

                    break;
                }
            }
        }


        /// <summary>
        /// Is only called by server.
        /// Checks if a matter with given ID exists and then calls <see cref="AcceptDemand(Matter)"/>.
        /// </summary>
        /// <param name="matterID">The ID of the matter to add to queue.</param>
        /// <param name="timeLimit">The time limit for the demand.</param>
        [ClientRpc]
        private void RpcAcceptDemand(string matterID, float timeLimit)
        {
            if (this.isClientOnly)
            {
                Matter targetMatter = Matter.GetByID(matterID);
                if (targetMatter != null)
                    this.AcceptDemand(targetMatter, timeLimit);
                else
                    Debug.LogError($"Cannot accept nonexisting matter with id '{matterID}'.");
            }
        }
        /// <summary>
        /// Is only called by server and after checking if given matter is currently demanded.
        /// Checks if a matter with given ID exists and then calls <see cref="RemoveDemand(Matter)"/>.
        /// </summary>
        /// <param name="matterID">id of matter to be removed from queue</param>
        [ClientRpc]
        private void RpcRemoveDemand(string matterID)
        {
            if (this.isClientOnly)
            {
                Matter targetMatter = Matter.GetByID(matterID);
                if (targetMatter != null)
                    this.RemoveDemand(targetMatter);
                else
                    Debug.LogError($"Cannot remove nonexisting matter with id '{matterID}'.");
            }
        }


        /// <summary>
        /// Called when a demand expires.
        /// Applies a score penalty and removes the demand.
        /// Only called on the server since this event is only registered on the server side.
        /// </summary>
        /// <param name="demandedMatterUI">The element that has triggered this event.</param>
        private void DemandedMatterUI_OnExpired_Server(DemandedMatterUI demandedMatterUI)
        {
            this.RemoveDemand(demandedMatterUI.Matter);
            this.OnDemandExpired?.Invoke(demandedMatterUI.Matter);
        }


        /// <summary>
        /// A Demand struct which contains the demanded matter and its uiElement.
        /// </summary>
        [System.Serializable]
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
