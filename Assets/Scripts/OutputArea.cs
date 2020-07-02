using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Mirror;

public class OutputArea : MonoBehaviour
{
    private void OnTriggerEnter(Collider other)
    {
        ElementObject elementObject = other.gameObject.GetComponent<ElementObject>();

        if (elementObject != null && this.CanAccept(elementObject.Element))
            NetworkServer.Destroy(other.gameObject);
    }


    private bool CanAccept(Recipe recipe)
    {
        foreach (Recipe r in GameManager.Instance.GameDemandQueue.CurrentDemands)
            if (recipe.Equals(r))
                return true;

        return false;
    }
}
