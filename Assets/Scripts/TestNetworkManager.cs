using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Mirror;
using TMPro;

public class TestNetworkManager : NetworkManager
{
    [SerializeField] Transform spawnPoint1;
    [SerializeField] Transform spawnPoint2;
    [SerializeField] Transform spawnPoint3;
    [SerializeField] Transform spawnPoint4;
    Transform spawn;


    public override void OnClientConnect(NetworkConnection conn)
    {
        base.OnClientConnect(conn);
        conn.Send(new AddPlayerMessage());
    }

    public override void OnServerAddPlayer(NetworkConnection conn)
    {
        switch (numPlayers)
        {
            case 0:
                spawn = spawnPoint1;
                break;
            case 1:
                spawn = spawnPoint2;
                break;
            case 2:
                spawn = spawnPoint3;
                break;
            case 3:
                spawn = spawnPoint4;
                break;

        }
        GameObject playerGO = GameObject.Instantiate(this.playerPrefab/*, this.spawn.position, this.spawn.rotation*/);
        NetworkServer.AddPlayerForConnection(conn, playerGO);
    }
}
