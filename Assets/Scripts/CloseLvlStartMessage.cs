/* Created by: SWT-P_SS20_Overcooked (Team Drai Studios) */
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
