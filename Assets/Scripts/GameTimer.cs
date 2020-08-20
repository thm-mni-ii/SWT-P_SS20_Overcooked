using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Mirror;
using TMPro;
using UnityEngine.Events;

namespace Underconnected
{
    /// <summary>
    /// Manages all functions concerning timer and interaction between server and client timer
    /// </summary>
    public class GameTimer : NetworkBehaviour
    {
        [Header("Settings")]
        [SerializeField] int levelDurationSeconds = 180;
        [SerializeField] int syncAfterSeconds = 30;


        public event UnityAction OnTimerFinished;


        /// <summary>
        /// Represents the current timer value.
        /// </summary>
        public float SecondsLeft { get; private set; }
        /// <summary>
        /// Depends on status of timer.
        /// </summary>
        public bool IsTimerRunning { get; private set; }


        private void Awake()
        {
            this.StopTimer();
            this.SetTime(this.levelDurationSeconds);
        }
        private void Update() => this.TickTimer(Time.deltaTime);


        /// <summary>
        /// Starts a new timer if no timer is currently running or stops the running timer when there is one.
        /// </summary>
        public void Toggle()
        {
            if (this.IsTimerRunning)
                this.StopTimer();
            else
                this.StartTimer();
        }

        /// <summary>
        /// Starts a timer with current values and signals clients to toggle timer as well.
        /// </summary>
        public void StartTimer()
        {
            this.IsTimerRunning = true;

            if (this.isServer)
                this.RpcToggleTimer(this.IsTimerRunning, this.SecondsLeft);
        }

        /// <summary>
        /// Stops timer for client or server and all its clients
        /// </summary>
        public void StopTimer()
        {
            this.IsTimerRunning = false;

            if (this.isServer)
                this.RpcToggleTimer(this.IsTimerRunning, this.SecondsLeft);
        }

        /// <summary>
        /// Sets timer for client or server and all its clients in seconds. Updates the timerText as well so it can be started.
        /// </summary>
        /// <param name="seconds">value of timer in seconds</param>
        public void SetTime(float seconds)
        {
            this.SecondsLeft = seconds;

            if (this.isServer)
                this.RpcSetTime(this.SecondsLeft);
        }

        /// <summary>
        /// Sets the current timeValue and uses <see cref="UpdateTimerText"/> to update timertext.
        /// Server sends current time to its client every few seconds to synchronize timers.
        /// </summary>
        /// <param name="deltaTime">time difference between update calls</param>
        private void TickTimer(float deltaTime)
        {
            if (this.IsTimerRunning && this.SecondsLeft > 0.0F)
            {
                this.SecondsLeft = Mathf.Max(0.0F, this.SecondsLeft - Time.deltaTime);

                if (this.isServer && ((int)this.SecondsLeft) % this.syncAfterSeconds == 0 && ((int)this.SecondsLeft != (int)(this.SecondsLeft + Time.deltaTime)))
                    this.RpcTickCheckpoint(this.SecondsLeft);

                if (this.SecondsLeft == 0) this.OnTimerFinished?.Invoke();
            }
        }

        #region Network Code

        #region Serialize/Deserialize
        public override bool OnSerialize(NetworkWriter writer, bool initialState)
        {
            bool dataWritten = base.OnSerialize(writer, initialState);

            if (initialState)
            {
                writer.WriteDouble(this.SecondsLeft);
                writer.WriteBoolean(this.IsTimerRunning);
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
            if (this.isClientOnly && timeOnServer < this.SecondsLeft)
                this.SetTime(timeOnServer);
        }
        #endregion

        #endregion
    }
}
