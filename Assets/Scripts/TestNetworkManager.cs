using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Mirror;
using TMPro;

public class TestNetworkManager : NetworkManager
{
    public override void OnClientConnect(NetworkConnection conn)
    {
        base.OnClientConnect(conn);
        conn.Send(new AddPlayerMessage());
    }

    public override void OnServerAddPlayer(NetworkConnection conn)
    {
        Transform spawnPos = GameManager.Instance.CurrentLevel.GetSpawnForPlayer(numPlayers);
        GameObject playerGO;

        if (spawnPos != null)
            playerGO = GameObject.Instantiate(this.playerPrefab, spawnPos.position, spawnPos.rotation);
        else
            playerGO = GameObject.Instantiate(this.playerPrefab);

        NetworkServer.AddPlayerForConnection(conn, playerGO);
    }
}
