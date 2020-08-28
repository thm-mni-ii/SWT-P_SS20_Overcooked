using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Mirror;

namespace Underconnected
{
    public class RemoveObjectSingleplayer : MonoBehaviour
    {
        int playerAmount = 1;
        // Start is called before the first frame update
        void Start()
        {
            if(playerAmount == 1)
            {
                this.gameObject.SetActive(false);
            }
        }

        // Update is called once per frame
        void Update()
        {

        }
    }


}

