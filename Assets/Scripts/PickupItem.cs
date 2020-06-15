using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Mirror;

public class PickupItem : NetworkBehaviour
{
    [SerializeField] float pickupReach = 0.5F;
    [SerializeField] LayerMask pickupLayers;
    [SerializeField] Transform reachOrigin;


    GameObject interactables;
    GameObject pickup;
    GameObject player;

    // Start is called before the first frame update
    void Start()
    {
        player = GameObject.FindGameObjectWithTag("Player");
        interactables = GameObject.Find("Interactables");
    }

    // Update is called once per frame
    void Update()
    {
        if (Input.GetKeyDown(KeyCode.Space))
        {
            if (isLocalPlayer)
            {
                if (pickup != null)
                {
                    pickup.transform.SetParent(interactables.transform);
                    pickup.GetComponent<Rigidbody>().isKinematic = false;
                    pickup = null;
                }
                else
                {
                    RaycastHit hitInfo;
                    if (Physics.Raycast(reachOrigin.position, reachOrigin.forward, out hitInfo, pickupReach, pickupLayers))
                    {
                        pickup = hitInfo.collider.gameObject;
                        pickup.transform.SetParent(player.transform);
                        pickup.GetComponent<Rigidbody>().isKinematic = true;
                    }
                }
            }
        }

    }

    // private void OnTriggerEnter(Collider collider) 
    // {
    //     if (collider.gameObject.tag == "Pickup")
    //     {
    //         pickup = collider.gameObject;

    //     }
    // }
    // private void OnTriggerExit(Collider collider)
    // {
    //     if (collider.gameObject.tag == "Pickup")
    //     {
    //         pickup = null;

    //     }
    // }
}
