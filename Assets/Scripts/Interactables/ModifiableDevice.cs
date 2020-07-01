using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using Mirror;

public class ModifiableDevice : NetworkBehaviour, IInteractable {
    [Header("Settings")]
    [SerializeField] float secondsToFinish = 3.0F;
    [SerializeField] GameObject inputObject = null;
    [SerializeField] GameObject resultObject = null;
    [SerializeField] Vector3 resultSpawnOffset = Vector3.zero;
    [Header("References")]
    [SerializeField] Canvas objectInfoCanvas = null;
    [SerializeField] Slider progressBar = null;


    public bool IsActivated { get; private set; }
    public bool IsFinished { get; private set; }
    public Interactor CurrentInteractor { get; private set; }
    public float SecondsPassed { get; private set; }
    public float TotalDuration => this.secondsToFinish;


    private void Awake() {
        this.IsActivated = false;
        this.IsFinished = false;
        this.CurrentInteractor = null;
        this.SecondsPassed = 0.0F;

        this.objectInfoCanvas.gameObject.SetActive(false);
    }


    private void Update() {
        if (this.IsActivated && !this.IsFinished) {
            if (this.SecondsPassed >= this.secondsToFinish)
                this.OnTimerFinish();
            else
                this.SecondsPassed = Mathf.Clamp(this.SecondsPassed + Time.deltaTime, 0.0F, this.secondsToFinish);

            this.progressBar.value = this.SecondsPassed / this.TotalDuration;
        }
    }


    public void Interact(Interactor interactor) {
        if (!this.IsActivated && !this.IsFinished)
            this.OnTimerStart(interactor);
    }


    protected void OnTimerStart(Interactor interactor) {
        if (!this.IsActivated && !this.IsFinished) {
            this.CurrentInteractor = interactor;
            this.SecondsPassed = 0.0F;
            this.progressBar.value = 0.0F;

            this.objectInfoCanvas.gameObject.SetActive(true);
            this.IsActivated = true;

            if (this.secondsToFinish <= Mathf.Epsilon)
                this.OnTimerFinish();
        }
    }
    protected void OnTimerFinish() {
        if (this.IsActivated && !this.IsFinished) {
            this.IsFinished = true;

            if (this.isServer) {
                NetworkServer.Destroy(inputObject.gameObject);
                if (this.resultObject != null)
                    NetworkServer.Spawn(GameObject.Instantiate(this.resultObject, this.transform.position + this.resultSpawnOffset, Quaternion.identity, this.transform.parent));
            }
        }
    }
}
