﻿using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;
using Mirror;

public class PickableObject : NetworkBehaviour, IInteractable
{
    [Tooltip("The default parent to apply when this pickable is dropped.")]
    [SerializeField] Transform defaultParent = null;
    [SerializeField] bool canBeDropped = true;
    [SerializeField] Rigidbody[] nonKinematicRBs;


    public bool IsPickedUp => this.currentHolder != null;
    public bool CanBeDropped => this.canBeDropped;
    public Interactor CurrentHolder => this.currentHolder;


    public event UnityAction<PickableObject, Interactor> OnPickedUp;
    public event UnityAction<PickableObject, Interactor> OnDropped;


    private Interactor currentHolder;


    private void OnDisable()
    {
        if (this.IsPickedUp)
            this.Drop(this.CurrentHolder);
    }


    public void Interact(Interactor interactor)
    {
        if (!this.IsPickedUp)
            this.Pickup(interactor);
        else if (this.canBeDropped)
            this.Drop(interactor);
    }

    public void SetDroppable(bool canBeDropped)
    {
        this.canBeDropped = canBeDropped;
    }

    public virtual void Pickup(Interactor interactor)
    {
        if (interactor != this.currentHolder)
        {
            this.currentHolder = interactor;
            interactor.SetHeldObject(this);
            this.transform.SetParent(interactor.transform, true);

            foreach (Rigidbody rb in this.nonKinematicRBs)
                rb.isKinematic = true;

            this.OnPickedUp?.Invoke(this, interactor);
        }
    }
    public virtual void Drop(Interactor interactor)
    {
        if (interactor == this.currentHolder)
        {
            this.currentHolder = null;
            interactor.SetHeldObject(null);
            this.transform.SetParent(this.defaultParent);

            foreach (Rigidbody rb in this.nonKinematicRBs)
                rb.isKinematic = false;

            this.OnDropped?.Invoke(this, interactor);
        }
    }
}
