using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class LevelUI : MonoBehaviour
{
    [SerializeField] GameTimer gameTimer = null;
    [SerializeField] DemandQueue gameDemandQueue = null;

    public GameTimer GameTimer => this.gameTimer;
    public DemandQueue DemandQueue => this.gameDemandQueue;
}
