using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Mirror;

public class DemandQueue : NetworkBehaviour
{
    [SerializeField] GameObject queueElementsContainer;
    [SerializeField] GameObject demandedMatterUIPrefab;


    public List<Matter> CurrentDemands => this.currentDemands;


    private List<Matter> currentDemands;


    private void Awake()
    {
        this.currentDemands = new List<Matter>();
    }


    public void AddDemand(Matter matter)
    {
        if (this.isServer)
            this.RpcAcceptDemand(matter.GetID());
        else
            this.AcceptDemand(matter);
    }


    private void AcceptDemand(Matter matter)
    {
        GameObject uiElement = GameObject.Instantiate(this.demandedMatterUIPrefab, Vector3.zero, Quaternion.identity, this.queueElementsContainer.transform);
        DemandedMatterUI demandedMatterUI = uiElement.GetComponent<DemandedMatterUI>();

        demandedMatterUI?.SetMatter(matter);
        this.currentDemands.Add(matter);
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
}
