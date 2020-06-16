using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Mirror;

public class Pickable : NetworkBehaviour, IInteractable
{
    [Tooltip("The default parent to apply when this pickable is dropped.")]
    [SerializeField] Transform defaultParent = null;
    [SerializeField] bool canBeDropped = true;
    [SerializeField] Rigidbody[] nonKinematicRBs;


    public bool IsPickedUp => this.currentHolder != null;
    public bool CanBeDropped => this.canBeDropped;
    public Interactor CurrentHolder => this.currentHolder;


    private Interactor currentHolder;


    public void Interact(Interactor interactor)
    {
        if (!this.IsPickedUp)
            this.OnPickup(interactor);
        else if (this.canBeDropped)
            this.OnDrop(interactor);
    }

    public void SetDroppable(bool canBeDropped)
    {
        this.canBeDropped = canBeDropped;
    }


    protected virtual void OnPickup(Interactor interactor)
    {
        this.currentHolder = interactor;
        this.transform.SetParent(interactor.transform, true);

        foreach (Rigidbody rb in this.nonKinematicRBs)
            rb.isKinematic = true;
    }
    protected virtual void OnDrop(Interactor interactor)
    {
        if (interactor == this.currentHolder)
        {
            this.currentHolder = null;
            this.transform.SetParent(this.defaultParent);

            foreach (Rigidbody rb in this.nonKinematicRBs)
                rb.isKinematic = false;
        }
    }
}
