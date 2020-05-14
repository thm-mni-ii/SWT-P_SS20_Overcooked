using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Mirror;
using TMPro;

public class TestNetworkManager : NetworkManager
{
    [SerializeField] Transform spawnPoint;
    [SerializeField] Timer timer;
    [SerializeField] TeamBase[] allBases;


    public override void OnStartServer()
    {
        this.timer.ShowControls();

        foreach (TeamBase tb in this.allBases)
            tb.SpawnFlag();
    }
    public override void OnStopServer()
    {
        this.timer.HideControls();
    }
    public override void OnClientConnect(NetworkConnection conn)
    {
        base.OnClientConnect(conn);
        conn.Send(new AddPlayerMessage());
    }


    public override void OnServerAddPlayer(NetworkConnection conn)
    {
        GameObject playerGO = GameObject.Instantiate(this.playerPrefab, this.spawnPoint.position, this.spawnPoint.rotation);
        NetworkServer.AddPlayerForConnection(conn, playerGO);
    }
}
