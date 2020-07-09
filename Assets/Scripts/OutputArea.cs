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
            // TODO: Remove matter from demands list
            NetworkServer.Destroy(other.gameObject);
    }


    private bool CanAccept(Matter matter)
    {
        if (matter != null)
        {
            foreach (Matter r in GameManager.Instance.GameDemandQueue.CurrentDemands)
                if (matter.Equals(r))
                    return true;
        }

        return false;
    }
}
