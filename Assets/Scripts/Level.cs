using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Mirror;

public class Level : NetworkBehaviour
{
    [Header("Settings")]
    [SerializeField] int levelDurationSeconds = 180;


    public override void OnStartServer()
    {
        GameManager.Instance.GameTimer.SetTimeLeft(levelDurationSeconds);
        GameManager.Instance.GameTimer.StartTimer();
    }
    public override void OnStopServer()
    {
        GameManager.Instance.GameTimer.StopTimer();
    }
}
