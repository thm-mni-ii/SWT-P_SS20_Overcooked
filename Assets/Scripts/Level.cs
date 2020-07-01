using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Mirror;

public class Level : NetworkBehaviour
{
    [Header("Settings")]
    [SerializeField] int levelDurationSeconds = 180;
    [SerializeField] Transform[] spawnPoints;


    public Transform[] SpawnPoints => this.spawnPoints;


    private void Awake()
    {
        GameManager.Instance.CurrentLevel = this;
    }

    public override void OnStartServer()
    {
        GameManager.Instance.GameTimer.SetTimeLeft(levelDurationSeconds);
        GameManager.Instance.GameTimer.StartTimer();
    }
    public override void OnStopServer()
    {
        GameManager.Instance.GameTimer.StopTimer();
    }


    public Transform GetSpawnForPlayer(int playerNum) => this.spawnPoints.Length > 0 ? this.spawnPoints[playerNum % this.spawnPoints.Length] : null;
}
