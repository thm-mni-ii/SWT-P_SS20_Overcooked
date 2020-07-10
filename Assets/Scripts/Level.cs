using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Mirror;

public class Level : NetworkBehaviour
{
    [Header("Settings")]
    [SerializeField] int levelDurationSeconds = 180;
    [SerializeField] Matter[] demandsPool;
    [SerializeField] Transform[] spawnPoints;


    public Transform[] SpawnPoints => this.spawnPoints;

    private Coroutine demandCoroutine;
    private WaitForSeconds demandCoroutineWait;


    private void Awake()
    {
        this.demandCoroutineWait = new WaitForSeconds(5.0F);
    }

    public override void OnStartServer()
    {
        GameManager.UI.LevelUI.GameTimer.SetTimeLeft(levelDurationSeconds);
        GameManager.UI.LevelUI.GameTimer.StartTimer();

        this.demandCoroutine = this.StartCoroutine(this.Do_DemandCoroutine());
    }
    public override void OnStopServer()
    {
        this.StopCoroutine(this.demandCoroutine);
        GameManager.UI.LevelUI.GameTimer.StopTimer();
    }


    public Transform GetSpawnForPlayer(int playerNum) => this.spawnPoints.Length > 0 ? this.spawnPoints[playerNum % this.spawnPoints.Length] : null;

    public void DeliverElement(ElementObject elementObject)
    {
        Matter matter = elementObject != null ? elementObject.Element : null;

        if (this.isServer && matter != null)
        {
            if (GameManager.UI.LevelUI.DemandQueue.HasDemand(matter))
            {
                GameManager.UI.LevelUI.DemandQueue.DeliverDemand(matter);
                NetworkServer.Destroy(elementObject.gameObject);
            }
        }
    }


    private IEnumerator Do_DemandCoroutine()
    {
        while (true)
        {
            // add random demand from demand pool
            yield return this.demandCoroutineWait;
            GameManager.UI.LevelUI.DemandQueue.AddDemand(this.demandsPool[Random.Range(0, this.demandsPool.Length)]);
        }
    }
}
