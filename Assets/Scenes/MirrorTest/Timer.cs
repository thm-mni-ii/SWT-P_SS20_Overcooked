using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Mirror;
using TMPro;

public class Timer : NetworkBehaviour
{
    [SerializeField] TMP_Text timerText;
    [SerializeField] GameObject timerControls;


    [SyncVar(hook = nameof(TimerValueChanged))]
    private float timerValue;
    private bool isTimerRunning;


    private void Start()
    {
        this.Reset();
    }


    public void ShowControls()
    {
        this.timerControls.SetActive(true);
    }
    public void HideControls()
    {
        this.timerControls.SetActive(false);
    }

    public void Toggle()
    {
        this.isTimerRunning = !this.isTimerRunning;
    }
    public void Reset()
    {
        this.isTimerRunning = false;
        this.timerValue = 60.0F;
    }


    [Server]
    private void FixedUpdate()
    {
        if (this.isTimerRunning && this.timerValue > 0.0F)
            this.timerValue = Mathf.Max(0.0F, this.timerValue - Time.fixedDeltaTime);
    }


    private void TimerValueChanged(float oldValue, float newValue)
    {
        this.timerText.text = $"<b>{Mathf.Ceil(newValue * 10.0F) * 0.1F}</b> s";
    }
}
