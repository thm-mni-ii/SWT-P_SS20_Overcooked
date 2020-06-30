using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class GameManager : MonoBehaviour
{
    public static GameManager Instance { get; private set; }



    [SerializeField] int levelBuildIndexStart = 1;


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

            this.LoadLevel(1);
        }
        else
            GameObject.Destroy(this.gameObject);
    }


    public void LoadLevel(int levelNum)
    {
        SceneManager.LoadScene(this.levelBuildIndexStart + levelNum - 1, LoadSceneMode.Additive);
    }
}
