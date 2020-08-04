using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Mirror;

namespace Underconnected
{
    public class ResetZone : MonoBehaviour
    {
        [SerializeField] Transform resetPoint;


        private void OnTriggerEnter(Collider other)
        {
            NetworkIdentity netId = other.GetComponent<NetworkIdentity>();

            if (netId.isServer || netId.isLocalPlayer)
            {
                other.gameObject.transform.position = resetPoint.position;
                other.attachedRigidbody.velocity = Vector3.zero;
            }
        }
    }
}
