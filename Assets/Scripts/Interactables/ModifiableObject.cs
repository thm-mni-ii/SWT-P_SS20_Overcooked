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
    [Header("References")]
    [SerializeField] Canvas objectInfoCanvas = null;
    [SerializeField] Slider progressBar = null;


    public bool IsActivated { get; private set; }
    public bool IsFinished { get; private set; }

    private Interactor currentInteractor;
    private float secondsPassed;


    private void Awake()
    {
        this.IsActivated = false;
        this.IsFinished = false;
        this.currentInteractor = null;
        this.secondsPassed = 0.0F;

        this.objectInfoCanvas.gameObject.SetActive(false);
    }

    private void Update()
    {
        this.UpdateTimer();
    }


    public void Interact(Interactor interactor)
    {
        if (!this.IsActivated && !this.IsFinished)
        {
            this.currentInteractor = interactor;
            this.StartTimer();
        }
    }


    private void StartTimer()
    {
        if (!this.IsActivated && !this.IsFinished)
        {
            this.secondsPassed = 0.0F;
            this.progressBar.value = 0.0F;

            this.objectInfoCanvas.gameObject.SetActive(true);
            this.IsActivated = true;

            if (this.secondsToFinish <= Mathf.Epsilon)
                this.Finish();
        }
    }
    private void UpdateTimer()
    {
        if (this.IsActivated && !this.IsFinished)
        {
            if (this.secondsPassed >= this.secondsToFinish)
                this.Finish();
            else
                this.secondsPassed = Mathf.Clamp(this.secondsPassed + Time.deltaTime, 0.0F, this.secondsToFinish);

            this.progressBar.value = this.secondsPassed / this.secondsToFinish;
        }
    }
    private void Finish()
    {
        if (this.IsActivated && !this.IsFinished)
        {
            this.IsFinished = true;
            GameObject.Destroy(this.gameObject);
        }
    }
}
