using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Mirror;

public class PlayerControls : NetworkBehaviour
{
    [SerializeField] Rigidbody rigidBody;
    [SerializeField] MeshRenderer playerModelRenderer;
    [SerializeField] float moveSpeed = 100.0F;
    [SerializeField] float rotationSpeed = 5.0F;
    [SerializeField] float rotationFalloff = 0.6F;


    public Team Team { get; private set; }


    private float forwardInput;
    private float rotationInput;

    [SyncVar(hook = nameof(UpdatePlayerTeam))]
    private int teamID;
    [SyncVar(hook = nameof(UpdatePlayerColor))]
    private Color playerColor;


    public override void OnStartServer()
    {
        this.Team = Team.GetNextTeamToJoin();

        this.teamID = this.Team.TeamID;
        this.playerColor = this.Team.TeamColor;//Random.ColorHSV();
    }
    public override void OnStartClient()
    {
        //this.rigidBody.isKinematic = !this.hasAuthority;
    }
    public override void OnStartLocalPlayer()
    {
        PlayerExposeShader.Instance.Player = this;
    }


    private void Update()
    {
        if (this.isLocalPlayer)
        {
            this.forwardInput = Input.GetAxisRaw("Vertical");
            if (Input.GetAxisRaw("Horizontal") != 0.0F)
                this.rotationInput = Input.GetAxisRaw("Horizontal");
        }
    }
    private void FixedUpdate()
    {
        if (this.isLocalPlayer)
        {
            this.rigidBody.AddForce(this.transform.rotation * Vector3.forward * this.forwardInput * this.moveSpeed);
            this.rigidBody.MoveRotation(this.rigidBody.rotation * Quaternion.Euler(0.0F, this.rotationInput * this.rotationSpeed, 0.0F));
            this.rotationInput *= this.rotationFalloff;
        }
    }
    private void OnDestroy()
    {
        if (PlayerExposeShader.Instance.Player == this)
            PlayerExposeShader.Instance.Player = null;
    }

    private void UpdatePlayerColor(Color oldColor, Color newColor)
    {
        this.playerModelRenderer.material.color = newColor;
    }
    private void UpdatePlayerTeam(int oldTeamID, int newTeamID)
    {
        this.Team = Team.GetByID(newTeamID);
    }
}
