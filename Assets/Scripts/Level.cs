﻿using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Mirror;

public class Level : NetworkBehaviour
{
    [Header("Settings")]
    [SerializeField] int levelDurationSeconds = 180;
    [SerializeField] Recipe[] demandsPool;
    [SerializeField] Transform[] spawnPoints;


    public Transform[] SpawnPoints => this.spawnPoints;

    private Coroutine demandCoroutine;


    private void Awake()
    {
        GameManager.Instance.CurrentLevel = this;
    }

    public override void OnStartServer()
    {
        GameManager.Instance.GameTimer.SetTimeLeft(levelDurationSeconds);
        GameManager.Instance.GameTimer.StartTimer();

        this.demandCoroutine = this.StartCoroutine(this.Do_DemandCoroutine());
    }
    public override void OnStopServer()
    {
        this.StopCoroutine(this.demandCoroutine);
        GameManager.Instance.GameTimer.StopTimer();
    }


    public Transform GetSpawnForPlayer(int playerNum) => this.spawnPoints.Length > 0 ? this.spawnPoints[playerNum % this.spawnPoints.Length] : null;


    private IEnumerator Do_DemandCoroutine()
    {
        while (true)
        {
            // add random demand from demand pool
            yield return new WaitForSeconds(5.0F);
            GameManager.Instance.GameDemandQueue.AddDemand(this.demandsPool[Random.Range(0, this.demandsPool.Length)]);
        }
    }
}