using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GameManager : MonoBehaviour
{
    public static GameManager Instance { get; private set; }


    public Level CurrentLevel { get; private set; }



    private void Awake()
    {
        if (GameManager.Instance == null)
        {
            GameManager.Instance = this;
            this.CurrentLevel = null;
            GameObject.DontDestroyOnLoad(this.gameObject);

            // TODO: Load player data
            // TODO: Check startup parameters & connect to given server
        }
        else
            GameObject.Destroy(this.gameObject);
    }
}
