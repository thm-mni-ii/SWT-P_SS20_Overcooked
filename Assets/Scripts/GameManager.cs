using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.SceneManagement;
using Mirror;

namespace Underconnected
{
    /// <summary>
    /// Manages the current game state and the game UI.
    /// Holds references to various singletons such as the UI manager or the current level.
    /// </summary>
    public class GameManager : MonoBehaviour
    {
        /// <summary>
        /// The OG game manager.
        /// </summary>
        public static GameManager Instance { get; private set; }
        /// <summary>
        /// The game's UI.
        /// </summary>
        public static UIManager UI => Instance.uiManager;
        /// <summary>
        /// The currently played level. `null` if there is none.
        /// </summary>
        public static Level CurrentLevel => Instance.currentLevel;
        /// <summary>
        /// The level number of the currently played level. `0` if there is none.
        /// </summary>
        public static int CurrentLevelNum => (CurrentLevel != null) ? Instance.currentLevelScene.buildIndex - Instance.levelBuildIndexStart + 1 : 0;
        /// <summary>
        /// The global network manager.
        /// </summary>
        public static TestNetworkManager NetworkManager => Instance.networkManager;


        /// <summary>
        /// An event that is called when the game manager has finished loading a level.
        /// Parameters: the level number and the actual level object that has been loaded
        /// </summary>
        public static UnityAction<int, Level> OnLevelLoaded;




        [Header("References")]
        [SerializeField] UIManager uiManager;
        [SerializeField] TestNetworkManager networkManager;

        [Header("Settings")]
        [SerializeField] int levelBuildIndexStart = 1;


        /// <summary>
        /// Holds the currently played level.
        /// </summary>
        private Level currentLevel;
        /// <summary>
        /// Holds the currently loaded level scene.
        /// </summary>
        private Scene currentLevelScene;


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
            }
            else
                GameObject.Destroy(this.gameObject);
        }

        /// <summary>
        /// Loads the level with the given level number.
        /// </summary>
        /// <param name="levelNum">The level number to load.</param>
        public void LoadLevel(int levelNum)
        {
            Debug.Log($"Loading level: {levelNum}");

            if (this.currentLevel != null)
                this.UnloadCurrentLevel();

            SceneManager.LoadScene(this.levelBuildIndexStart + levelNum - 1, LoadSceneMode.Additive);
        }
        /// <summary>
        /// Loads the successor to the current level.
        /// </summary>
        public void LoadNextLevel()
        {
            this.LoadLevel(this.currentLevelScene.buildIndex + 1);
        }
        /// <summary>
        /// Unloads the level with the given level number.
        /// </summary>
        /// <param name="levelNum">The level number to load.</param>
        public void UnloadCurrentLevel()
        {
            if (this.currentLevel != null)
            {
                this.currentLevel = null;
                SceneManager.UnloadSceneAsync(this.currentLevelScene);
            }
        }


        /// <summary>
        /// Called when a scene is loaded by the SceneManager.
        /// </summary>
        /// <param name="scene">The loaded scene.</param>
        /// <param name="mode">The mode the given <paramref name="scene"/> has been loaded with.</param>
        private void SceneManager_SceneLoaded(Scene scene, LoadSceneMode mode)
        {
            this.currentLevel = null;

            if (scene.isLoaded)
            {
                GameObject[] rootGOs = scene.GetRootGameObjects();
                foreach (GameObject rootGO in rootGOs)
                {
                    this.currentLevel = rootGO.GetComponent<Level>();
                    this.currentLevelScene = scene;

                    if (this.currentLevel != null)
                    {
                        SceneManager.SetActiveScene(scene);
                        Debug.Log($"Level loaded: {this.currentLevel.gameObject.name}");

                        GameManager.OnLevelLoaded?.Invoke(GameManager.CurrentLevelNum, GameManager.CurrentLevel);
                        break;
                    }
                }
            }
            else
                Debug.LogError($"Scene with index {scene.buildIndex} could not be loaded in time.");
        }
    }
}
