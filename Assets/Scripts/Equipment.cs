using Mirror;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Equipment : NetworkBehaviour
{

    [SerializeField] PickableObject[] acceptedElements;
    List<GameObject> availableObjects;

    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
     
    }

    private void OnTriggerEnter(Collider collider)
    {
        if (collider.gameObject.tag == "Pickup")
        {
            for (int i = 0; i < acceptedElements.Length; i++)
            {
                if (acceptedElements[i].gameObject.name == collider.gameObject.name)
                {                   
                    collider.gameObject.SetActive(false);
                    availableObjects.Add(collider.gameObject);
                }
            }
                
        }
    }

    
}
