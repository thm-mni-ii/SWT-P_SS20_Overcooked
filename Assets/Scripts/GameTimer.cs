﻿using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Mirror;
using TMPro;

public class GameTimer : NetworkBehaviour
{
    [SerializeField] TMP_Text timerText;


    [SyncVar(hook = nameof(TimerValueChanged))]
    private float timerValue;
    private bool isTimerRunning;


    private void Awake()
    {
        this.StopTimer();
    }


    public void Toggle()
    {
        this.isTimerRunning = !this.isTimerRunning;
    }
    public void StartTimer()
    {
        this.isTimerRunning = true;
    }
    public void StopTimer()
    {
        this.isTimerRunning = false;
    }

    public void SetTimeLeft(float secondsLeft)
    {
        this.timerValue = secondsLeft;
    }


    [ServerCallback]
    private void FixedUpdate()
    {
        if (this.isTimerRunning && this.timerValue > 0.0F)
            this.timerValue = Mathf.Max(0.0F, this.timerValue - Time.fixedDeltaTime);
    }


    private void TimerValueChanged(float oldValue, float newValue)
    {
        int roundedSeconds = (int)Mathf.Ceil(newValue);

        if (roundedSeconds >= 60)
            this.timerText.text = $"<b>{(roundedSeconds / 60):#0}:{(roundedSeconds % 60):00}</b>";
        else
            this.timerText.text = $"<b>{roundedSeconds}</b>";
    }
}
