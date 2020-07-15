using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class GameManager : MonoBehaviour
{
    public static GameManager Instance { get; private set; }



    [SerializeField] int levelBuildIndexStart = 1;
    [SerializeField] Timer gameTimer = null;
    [SerializeField] DemandQueue gameDemandQueue = null;

    [Header("Prefabs")]
    [SerializeField] GameObject finishedLevelPrefab;

    public Level CurrentLevel { get; set; }
    public Timer GameTimer => this.gameTimer;
    public DemandQueue GameDemandQueue => this.gameDemandQueue;

    private GameObject levelFinished;

    private void Awake()
    {
        if (GameManager.Instance == null)
        {
            GameManager.Instance = this;
            this.CurrentLevel = null;

            // TODO: Load player data
            // TODO: Check startup parameters & connect to given server or show main menu

            this.LoadLevel(1);
        }
        else
            GameObject.Destroy(this.gameObject);
    }

    public void ShowLevelFinished() {
        //Time.timeScale = 0.0F;
        //this.UnloadUIScenes();
        this.levelFinished.SetActive(true);
    }

    public void LoadLevel(int levelNum)
    {
        SceneManager.LoadScene(this.levelBuildIndexStart + levelNum - 1, LoadSceneMode.Additive);
    }
}
