using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using Mirror;

namespace Underconnected
{
    /// <summary>
    /// Represents an object that shows a progress bar upon interaction and triggers something after the interaction is finished.
    /// </summary>
    public class ModifiableObject : NetworkBehaviour, IInteractable
    {
        [Header("Settings")]
        [SerializeField] float secondsToFinish = 5.0F;
        [SerializeField] GameObject resultObject = null;
        [SerializeField] Vector3 resultSpawnOffset = Vector3.zero;
        [Header("References")]
        [SerializeField] Canvas objectInfoCanvas = null;
        [SerializeField] Slider progressBar = null;


        /// <summary>
        /// Tells whether this object has been activated by an <see cref="Interactor"/>.
        /// </summary>
        public bool IsActivated { get; protected set; }
        /// <summary>
        /// Tells whether the progress bar has been filled and the process is finished.
        /// </summary>
        public bool IsFinished { get; protected set; }
        /// <summary>
        /// Returns the <see cref="Interactor"/> that has activated this object.
        /// </summary>
        public Interactor CurrentInteractor { get; private set; }
        /// <summary>
        /// Tells how many seconds have passed since activation.
        /// </summary>
        public float SecondsPassed { get; private set; }
        /// <summary>
        /// Tells how many seconds it takes to finish after this object has been activated.
        /// </summary>
        public float TotalDuration => this.secondsToFinish;

        /// <summary>
        /// Holds a reference to the object information canvas.
        /// </summary>
        protected Canvas ObjectInfoCanvas => this.objectInfoCanvas;


        private void Awake()
        {
            this.IsActivated = false;
            this.IsFinished = false;
            this.CurrentInteractor = null;
            this.SecondsPassed = 0.0F;

            this.objectInfoCanvas.gameObject.SetActive(false);
        }

        protected virtual void Update()
        {
            if (this.IsActivated && !this.IsFinished)
            {
                if (this.SecondsPassed >= this.secondsToFinish)
                    this.OnTimerFinish();
                else
                    this.SecondsPassed = Mathf.Clamp(this.SecondsPassed + Time.deltaTime, 0.0F, this.secondsToFinish);

                this.progressBar.value = this.SecondsPassed / this.TotalDuration;
            }
        }


        public virtual void Interact(Interactor interactor)
        {
            if (!this.IsActivated && !this.IsFinished)
                this.OnTimerStart(interactor);
        }


        /// <summary>
        /// Set how many seconds it takes to finish an action and fill the progress bar.
        /// </summary>
        /// <param name="secondsToFinish">The new amount of seconds.</param>
        protected void SetSecondsToFinish(float secondsToFinish)
        {
            this.secondsToFinish = secondsToFinish;
        }


        /// <summary>
        /// Called when this object has been interacted with and the timer is about to start.
        /// It resets the timer values and shows the progress bar.
        /// </summary>
        /// <param name="interactor">The <see cref="Interactor"/> object that has interacted with this object and caused the timer to start.</param>
        protected virtual void OnTimerStart(Interactor interactor)
        {
            if (!this.IsActivated && !this.IsFinished)
            {
                this.CurrentInteractor = interactor;
                this.SecondsPassed = 0.0F;
                this.progressBar.value = 0.0F;

                this.objectInfoCanvas.gameObject.SetActive(true);
                this.IsActivated = true;

                if (this.secondsToFinish <= Mathf.Epsilon)
                    this.OnTimerFinish();
            }
        }
        /// <summary>
        /// Called when the timer has finished.
        /// The default implementation of this method destroys this object and replaces it with the prefab set in the inspector.
        /// Should be overridden for custom behaviour.
        /// </summary>
        protected virtual void OnTimerFinish()
        {
            if (this.IsActivated && !this.IsFinished)
            {
                this.IsFinished = true;

                if (this.isServer)
                {
                    NetworkServer.Destroy(this.gameObject);
                    if (this.resultObject != null)
                        NetworkServer.Spawn(GameObject.Instantiate(this.resultObject, this.transform.position + this.resultSpawnOffset, Quaternion.identity, this.transform.parent));
                }
            }
        }
    }
}
