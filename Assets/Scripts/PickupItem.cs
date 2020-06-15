using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Mirror;

public class PickupItem : NetworkBehaviour
{
    bool isPickedUp = false;
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
                if (!isPickedUp && pickup != null)
                {
                    pickup.transform.SetParent(player.transform);
                    //pickup.transform.position = pickup.gameObject.transform.position + new Vector3(0, 0, 0);
                    isPickedUp = true;
                }
                /*if (isPickedUp)
                {
                    pickup.transform.SetParent(interactables.transform);
                    isPickedUp = false;
                }*/
            }            
        }
        
    }

    private void OnTriggerEnter(Collider collider) 
    {
        if (collider.gameObject.tag == "Pickup")
        {
            pickup = collider.gameObject;
           
        }
    }
    private void OnTriggerExit(Collider collider)
    {
        if (collider.gameObject.tag == "Pickup")
        {
            pickup = null;

        }
    }
}
