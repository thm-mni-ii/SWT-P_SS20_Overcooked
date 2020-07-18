using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[RequireComponent(typeof(PickableObject))]
public class ElementObject : MonoBehaviour
{
    [Header("References")]
    [SerializeField] Rigidbody[] rigidbodies;
    [SerializeField] Collider[] colliders;
    [SerializeField] PickableObject pickableObject;

    [Header("Settings")]
    [SerializeField] Matter element;


    public Matter Element => this.element;


    private void Awake()
    {
        // Setup events for reacting to picking up and dropping this element
        this.pickableObject.OnPickedUp += this.OnPickedUp;
        this.pickableObject.OnDropped += this.OnDropped;
    }


    public void EnablePhysics()
    {
        foreach (Rigidbody rb in this.rigidbodies)
            rb.isKinematic = false;
        foreach (Collider c in this.colliders)
            c.enabled = true;
    }
    public void DisablePhysics()
    {
        foreach (Rigidbody rb in this.rigidbodies)
            rb.isKinematic = true;
        foreach (Collider c in this.colliders)
            c.enabled = false;
    }


    private void OnPickedUp(PickableObject pickedUp, Interactor picker) => this.DisablePhysics();
    private void OnDropped(PickableObject dropped, Interactor dropper) => this.EnablePhysics();
}
