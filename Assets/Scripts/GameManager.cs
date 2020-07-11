using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class GameManager : MonoBehaviour
{
    public static GameManager Instance { get; private set; }
    public static UIManager UI => Instance.uiManager;
    public static Level CurrentLevel => Instance.currentLevel;



    [Header("References")]
    [SerializeField] UIManager uiManager;

    [Header("Settings")]
    [SerializeField] int levelBuildIndexStart = 1;


    private Level currentLevel;



    private void Awake()
    {
        if (GameManager.Instance == null)
        {
            // Init
            GameManager.Instance = this;
            this.currentLevel = null;
            SceneManager.sceneLoaded += this.SceneManager_SceneLoaded;

            // TODO: Load player data
            // TODO: Check startup parameters & connect to given server or show main menu

            this.LoadLevel(1);
        }
        else
            GameObject.Destroy(this.gameObject);
    }


    public void LoadLevel(int levelNum)
    {
        SceneManager.LoadScene(this.levelBuildIndexStart + levelNum - 1, LoadSceneMode.Additive);
    }


    private void SceneManager_SceneLoaded(Scene scene, LoadSceneMode mode)
    {
        this.currentLevel = null;

        if (scene.isLoaded)
        {
            GameObject[] rootGOs = scene.GetRootGameObjects();
            foreach (GameObject rootGO in rootGOs)
            {
                this.currentLevel = rootGO.GetComponent<Level>();
                if (this.currentLevel != null)
                {
                    SceneManager.SetActiveScene(scene);
                    Debug.Log($"Level loaded: {this.currentLevel.gameObject.name}");
                    break;
                }
            }
        }
        else
            Debug.LogError($"Scene with index {scene.buildIndex} could not be loaded in time.");
    }
}
