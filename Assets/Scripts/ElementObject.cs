﻿using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ElementObject : MonoBehaviour
{
    [Header("References")]
    [SerializeField] Rigidbody[] rigidbodies;

    [Header("Settings")]
    [SerializeField] Matter element;


    public Matter Element => this.element;


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
