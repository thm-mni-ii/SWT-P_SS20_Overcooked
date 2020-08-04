using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Mirror;

namespace Underconnected
{
    public class DemandQueue : NetworkBehaviour
    {
        [SerializeField] GameObject queueElementsContainer;
        [SerializeField] GameObject demandedMatterUIPrefab;


        private List<Demand> currentDemands;


        private void Awake()
        {
            this.currentDemands = new List<Demand>();
        }


        public void AddDemand(Matter matter)
        {
            if (this.isServer)
                this.RpcAcceptDemand(matter.GetID());
            else
                this.AcceptDemand(matter);
        }
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


        private void AcceptDemand(Matter matter)
        {
            GameObject uiElement = GameObject.Instantiate(this.demandedMatterUIPrefab, Vector3.zero, Quaternion.identity, this.queueElementsContainer.transform);
            DemandedMatterUI demandedMatterUI = uiElement.GetComponent<DemandedMatterUI>();

            demandedMatterUI?.SetMatter(matter);
            this.currentDemands.Add(new Demand(matter, demandedMatterUI));
        }
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
        private void RpcAcceptDemand(string matterID)
        {
            Matter targetMatter = Matter.GetByID(matterID);
            if (targetMatter != null)
                this.AcceptDemand(targetMatter);
            else
                Debug.LogError($"Cannot accept nonexisting matter with id '{matterID}'.");
        }
        [ClientRpc]
        private void RpcRemoveDemand(string matterID)
        {
            Matter targetMatter = Matter.GetByID(matterID);
            if (targetMatter != null)
                this.RemoveDemand(targetMatter);
            else
                Debug.LogError($"Cannot remove nonexisting matter with id '{matterID}'.");
        }



        public struct Demand
        {
            public Matter demandedMatter;
            public DemandedMatterUI uiElement;


            public Demand(Matter demandedMatter, DemandedMatterUI uiElement)
            {
                this.demandedMatter = demandedMatter;
                this.uiElement = uiElement;
            }
        }
    }
}
