using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CloseLvlStartMessage : MonoBehaviour
{
    [SerializeField] GameObject startMessage;

    public void CloseStartMessage()
    {
        startMessage.SetActive(false);
    }

}
