/* Created by: SWT-P_SS20_Overcooked (Team Drai Studios) */
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Underconnected
{
    /// <summary>
    /// Closes the tutorial UI at the beginning of each level
    /// </summary>
    public class CloseLvlStartMessage : MonoBehaviour
    {
        [SerializeField] GameObject startMessage;

        public void CloseStartMessage()
        {
            startMessage.SetActive(false);
        }

    }
}

