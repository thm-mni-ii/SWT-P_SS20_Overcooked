using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using Mirror;

public class ModifiableObject : NetworkBehaviour, IInteractable
{
    [Header("Settings")]
    [SerializeField] float secondsToFinish = 5.0F;
    [SerializeField] GameObject resultObject = null;
    [SerializeField] Vector3 resultSpawnOffset = Vector3.zero;
    [Header("References")]
    [SerializeField] Canvas objectInfoCanvas = null;
    [SerializeField] Slider progressBar = null;
    [Tooltip("The default parent to apply when this pickable is dropped.")]
    [SerializeField] Transform defaultParent = null;
    [SerializeField] bool canBeDropped = true;
    [SerializeField] Rigidbody[] nonKinematicRBs;

    public bool IsActivated { get; private set; }
    public bool IsFinished { get; private set; }
    public Interactor CurrentInteractor { get; private set; }
    public float SecondsPassed { get; private set; }
    public float TotalDuration => this.secondsToFinish;
    public bool IsPickedUp => this.currentHolder != null;
    public bool CanBeDropped => this.canBeDropped;
    public Interactor CurrentHolder => this.currentHolder;

    private Interactor currentHolder;

    private void Awake()
    {
        this.IsActivated = false;
        this.IsFinished = false;
        this.CurrentInteractor = null;
        this.SecondsPassed = 0.0F;

        this.objectInfoCanvas.gameObject.SetActive(false);
    }

    private void Update()
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
    
    public void Interact(Interactor interactor) {
        if (!this.IsPickedUp)
            this.OnPickup(interactor);
        else if (this.canBeDropped)
            this.OnDrop(interactor);
    }
    
    public void InteractWithDevice(Interactor interactor) {
        if (!this.IsActivated && !this.IsFinished)
            this.OnTimerStart(interactor);
    }
    
    public void SetDroppable(bool canBeDropped) {
        this.canBeDropped = canBeDropped;
    }

    protected virtual void OnPickup(Interactor interactor) {
        this.currentHolder = interactor;
        interactor.SetHeldModifiedObject(this);
        this.transform.SetParent(interactor.transform, true);

        foreach (Rigidbody rb in this.nonKinematicRBs)
            rb.isKinematic = true;
    }
    protected virtual void OnDrop(Interactor interactor) {
        if (interactor == this.currentHolder) {
            this.currentHolder = null;
            interactor.SetHeldModifiedObject(null);
            this.transform.SetParent(this.defaultParent);

            foreach (Rigidbody rb in this.nonKinematicRBs)
                rb.isKinematic = false;
        }
    }

    protected void OnTimerStart(Interactor interactor)
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

    protected void OnTimerFinish()
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
