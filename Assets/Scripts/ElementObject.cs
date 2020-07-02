using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ElementObject : MonoBehaviour
{
    [Header("References")]
    [SerializeField] Rigidbody[] rigidbodies;

    [Header("Settings")]
    [SerializeField] Recipe element;


    public Recipe Element => this.element;


    public void EnablePhysics()
    {
        foreach (Rigidbody rb in this.rigidbodies)
            rb.isKinematic = false;
    }
    public void DisablePhysics()
    {
        foreach (Rigidbody rb in this.rigidbodies)
            rb.isKinematic = true;
    }
}
