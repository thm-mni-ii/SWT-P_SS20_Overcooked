using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Mirror;
using TMPro;

namespace Underconnected
{
    /// <summary>
    /// Manages all functions concerning timer and interaction between server and client timer
    /// </summary>
    public class GameTimer : NetworkBehaviour
    {
        [Header("References")]
        [SerializeField] TMP_Text timerText;

        [Header("Settings")]
        [SerializeField] int syncAfterSeconds = 30;

        /// <summary>
        /// Represents the current timer value.
        /// </summary>
        private float timerValue;
        /// <summary>
        /// Depends on status of timer.
        /// </summary>
        private bool isTimerRunning;


        private void Awake()
        {
            this.StopTimer();
        }
        private void Update()
        {
            this.TickTimer(Time.deltaTime);
        }

        /// <summary>
        /// Starts a new timer if no timer is currently running or stops the running timer when there is one.
        /// </summary>
        public void Toggle()
        {
            if (this.isTimerRunning)
                this.StopTimer();
            else
                this.StartTimer();
        }

        /// <summary>
        /// Starts a timer with current values and signals clients to toggle timer as well.
        /// </summary>
        public void StartTimer()
        {
            this.isTimerRunning = true;

            if (this.isServer)
                this.RpcToggleTimer(this.isTimerRunning, this.timerValue);
        }

        /// <summary>
        /// Stops timer for client or server and all its clients
        /// </summary>
        public void StopTimer()
        {
            this.isTimerRunning = false;

            if (this.isServer)
                this.RpcToggleTimer(this.isTimerRunning, this.timerValue);
        }

        /// <summary>
        /// Sets timer for client or server and all its clients in seconds. Updates the timerText as well so it can be started.
        /// </summary>
        /// <param name="seconds">value of timer in seconds</param>
        public void SetTime(float seconds)
        {
            this.timerValue = seconds;
            this.UpdateTimerText();

            if (this.isServer)
                this.RpcSetTime(this.timerValue);
        }

        /// <summary>
        /// Sets the current timeValue and uses <see cref="UpdateTimerText"/> to update timertext.
        /// Server sends current time to its client every few seconds to synchronize timers.
        /// </summary>
        /// <param name="deltaTime">time differnce between update calls</param>
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
        /// <summary>
        /// Rounds timerValue and converts it to 00:00 format.
        /// </summary>
        private void UpdateTimerText()
        {
            int roundedSeconds = (int)Mathf.Ceil(this.timerValue);

            if (roundedSeconds >= 60)
                this.timerText.text = $"<b>{(roundedSeconds / 60):#0}:{(roundedSeconds % 60):00}</b>";
            else
                this.timerText.text = $"<b>{roundedSeconds}</b>";
        }


        #region Network Code

        #region Serialize/Deserialize
        public override bool OnSerialize(NetworkWriter writer, bool initialState)
        {
            bool dataWritten = base.OnSerialize(writer, initialState);

            if (initialState)
            {
                writer.WriteDouble(this.timerValue);
                writer.WriteBoolean(this.isTimerRunning);
                dataWritten = true;
            }

            return dataWritten;
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
        /// <summary>
        /// Can only be called by server to notify clients to toggle their respective timers
        /// </summary>
        /// <param name="isTimerEnabled">checks if timer is enabled</param>
        /// <param name="currentTime">current timerValue on server</param>
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
        /// <summary>
        /// Can only be called by server to change timerValue for all clients.
        /// </summary>
        /// <param name="currentTime">current timerValue on server</param>
        private void RpcSetTime(float currentTime)
        {
            if (this.isClientOnly)
                this.SetTime(currentTime);
        }
        [ClientRpc]
        /// <summary>
        /// Can only be called by server to synchronize client timers every 30 seconds
        /// </summary>
        /// <param name="timeOnServer">current timerValue on server</param>
        private void RpcTickCheckpoint(float timeOnServer)
        {
            if (this.isClientOnly && timeOnServer < this.timerValue)
                this.SetTime(timeOnServer);
        }
        #endregion

        #endregion
    }
}
