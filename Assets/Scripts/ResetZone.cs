using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Mirror;

namespace Underconnected
{
    /// <summary>
    /// Will teleport objects entering the zone back to their respective reset positions.
    /// </summary>
    public class ResetZone : MonoBehaviour
    {
        [SerializeField] Transform resetPoint;

        /// <summary>
        /// When an object with a collider enters the resetzone it will be teleported back to its reset position.
        /// </summary>
        /// <param name="other">collider of object entering resetzone</param>
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
