using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Mirror;
using TMPro;

public class GameTimer : NetworkBehaviour
{
    [Header("References")]
    [SerializeField] TMP_Text timerText;

    [Header("Settings")]
    [SerializeField] int syncAfterSeconds = 30;


    private float timerValue;
    private bool isTimerRunning;


    private void Awake()
    {
        this.StopTimer();
    }
    private void Update()
    {
        this.TickTimer(Time.deltaTime);
    }


    public void Toggle()
    {
        if (this.isTimerRunning)
            this.StopTimer();
        else
            this.StartTimer();
    }
    public void StartTimer()
    {
        this.isTimerRunning = true;

        if (this.isServer)
            this.RpcToggleTimer(this.isTimerRunning, this.timerValue);
    }
    public void StopTimer()
    {
        this.isTimerRunning = false;

        if (this.isServer)
            this.RpcToggleTimer(this.isTimerRunning, this.timerValue);
    }

    public void SetTime(float seconds)
    {
        this.timerValue = seconds;
        this.UpdateTimerText();

        if (this.isServer)
            this.RpcSetTime(this.timerValue);
    }


    private void TickTimer(float deltaTime)
    {
        if (this.isTimerRunning && this.timerValue > 0.0F)
        {
            this.timerValue = Mathf.Max(0.0F, this.timerValue - Time.deltaTime);
            this.UpdateTimerText();

            if (this.isServer && ((int)this.timerValue) % this.syncAfterSeconds == 0 && ((int)this.timerValue != (int)(this.timerValue + Time.deltaTime)))
                this.RpcTickCheckpoint(this.timerValue);
        }
    }
    private void UpdateTimerText()
    {
        this.timerText.text = $"<b>{Mathf.Ceil(this.timerValue * 10.0F) * 0.1F}</b> s";
    }


    #region Network Code

    #region Serialize/Deserialize
    public override bool OnSerialize(NetworkWriter writer, bool initialState)
    {
        int roundedSeconds = (int)Mathf.Ceil(newValue);

        if (roundedSeconds >= 60)
            this.timerText.text = $"<b>{(roundedSeconds / 60):#0}:{(roundedSeconds % 60):00}</b>";
        else
            this.timerText.text = $"<b>{roundedSeconds}</b>";
    }
    public override void OnDeserialize(NetworkReader reader, bool initialState)
    {
        base.OnDeserialize(reader, initialState);

        if (initialState)
        {
            this.SetTime((float)reader.ReadDouble());

            if (reader.ReadBoolean())
                this.StartTimer();
            else
                this.StopTimer();
        }
    }
    #endregion

    #region RPC/Commands
    [ClientRpc]
    private void RpcToggleTimer(bool isTimerEnabled, float currentTime)
    {
        if (this.isClientOnly)
        {
            this.SetTime(currentTime);

            if (isTimerEnabled)
                this.StartTimer();
            else
                this.StopTimer();
        }
    }
    [ClientRpc]
    private void RpcSetTime(float currentTime)
    {
        if (this.isClientOnly)
            this.SetTime(currentTime);
    }
    [ClientRpc]
    private void RpcTickCheckpoint(float timeOnServer)
    {
        if (this.isClientOnly && timeOnServer < this.timerValue)
            this.SetTime(timeOnServer);
    }
    #endregion

    #endregion
}
