﻿using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Mirror;

/// <summary>
/// Will use the collider of triggering object and pass on its component to <see cref="DeliverObject"/>.
/// </summary>
public class OutputArea : MonoBehaviour
{
    /// <summary>
    /// Will use collider of triggering object and pass on its component to <see cref="DeliverObject"/>.
    /// </summary>
    /// <param name="other">Collider of triggering matter</param>
    private void OnTriggerEnter(Collider other) => GameManager.CurrentLevel.DeliverObject(other.GetComponent<MatterObject>());
}
