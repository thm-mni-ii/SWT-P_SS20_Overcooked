using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Mirror;

public class Flag : NetworkBehaviour
{
    [SerializeField] MeshRenderer flagPlaneRenderer;
    [SerializeField] string playerTag = "Player";
    [SerializeField] string baseTag = "Base";


    public Team Team { get => this.flagBase != null ? this.flagBase.Team : null; }


    private Material flagPlaneMaterial;
    private TeamBase flagBase;


    private void Awake()
    {
        this.flagPlaneMaterial = this.flagPlaneRenderer.material;
    }


    public void SetBase(TeamBase flagBase)
    {
        this.flagBase = flagBase;
        this.flagPlaneMaterial.color = this.Team != null ? this.Team.TeamColor : Color.white;

        if (this.isServer)
            this.RpcSetBase(flagBase.Team.TeamID);
    }

    public void ResetFlag()
    {
        this.transform.SetParent(this.flagBase.FlagSpawn, false);

        if (this.isServer)
            this.RpcResetFlag();
    }
    public void AttachFlagToPlayer(PlayerControls player)
    {
        this.transform.SetParent(player.transform, false);

        if (this.isServer)
            this.RpcAttachFlagToPlayer(player.transform);
    }


    [ServerCallback]
    private void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag(this.playerTag))
        {
            PlayerControls player = other.GetComponent<PlayerControls>();
            if (player.Team != this.Team)
                this.AttachFlagToPlayer(player);
        }
        else if (other.CompareTag(this.baseTag))
        {
            TeamBase teamBase = other.GetComponent<TeamBaseCaptureZone>().TeamBase;
            if (teamBase.Team != this.Team)
                this.ResetFlag();
        }
    }


    [ClientRpc]
    private void RpcSetBase(int teamID)
    {
        this.flagBase = Team.GetByID(teamID).TeamBase;
        this.flagPlaneMaterial.color = this.Team != null ? this.Team.TeamColor : Color.white;
        //this.SetBase(Team.GetByID(teamID).TeamBase);
    }
    [ClientRpc]
    private void RpcAttachFlagToPlayer(Transform playerTransform)
    {
        this.transform.SetParent(playerTransform, false);
        //this.AttachFlagToPlayer(playerTransform.GetComponent<PlayerControls>());
    }
    [ClientRpc]
    private void RpcResetFlag()
    {
        this.transform.SetParent(this.flagBase.FlagSpawn, false);
        //this.ResetFlag();
    }
}
