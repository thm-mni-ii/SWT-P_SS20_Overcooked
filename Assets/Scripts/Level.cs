using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Mirror;

public class Level : NetworkBehaviour
{
    [Header("Settings")]
    [SerializeField] int scorePerDelivery = 50;
    [SerializeField] int levelDurationSeconds = 180;
    [SerializeField] Matter[] demandsPool;
    [SerializeField] Transform[] spawnPoints;


    public int PlayerScore => this.PlayerScore;
    public Transform[] SpawnPoints => this.spawnPoints;

    [SyncVar(hook = nameof(PlayerScore_OnChange))] int playerScore;
    private Coroutine demandCoroutine;
    private WaitForSeconds demandCoroutineWait;


    private void Awake()
    {
        this.playerScore = 0;
        this.demandCoroutine = null;
        this.demandCoroutineWait = new WaitForSeconds(20.0F);
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

    public void DeliverObject(MatterObject matterObject)
    {
        Matter matter = matterObject != null ? matterObject.Matter : null;

        if (this.isServer && matter != null)
        {
            if (GameManager.UI.LevelUI.DemandQueue.HasDemand(matter))
            {
                this.IncrementPlayerScore(this.scorePerDelivery);
                GameManager.UI.LevelUI.DemandQueue.DeliverDemand(matter);
                NetworkServer.Destroy(matterObject.gameObject);
            }
        }
    }

    public void IncrementPlayerScore(int scoreDelta) => this.SetPlayerScore(this.playerScore + scoreDelta);
    public void SetPlayerScore(int newScore)
    {
        if (this.isServer)
            this.playerScore = newScore;
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

    private void PlayerScore_OnChange(int oldValue, int newValue)
    {
        GameManager.UI.LevelUI.ScoreDisplay.SetScore(newValue);
    }
}
