using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Mirror;

public class OutputArea : MonoBehaviour
{
    private void OnTriggerEnter(Collider other) => GameManager.CurrentLevel.DeliverElement(other.GetComponent<ElementObject>());
}
