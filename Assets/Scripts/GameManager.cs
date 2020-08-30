/* Created by: SWT-P_SS20_Overcooked (Team Drai Studios) */
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.SceneManagement;
using GameFramework;
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
        /// Holds the player character prefab to spawn on levels.
        /// </summary>
        public static GameObject PlayerPrefab => Instance.playerPrefab.gameObject;
        /// <summary>
        /// Holds the game's main camera.
        /// </summary>
        public static Camera Camera => Instance.camera;

        /// <summary>
        /// Information about our own player.
        /// </summary>
        public static PlayerInfo PlayerInfo => Instance.playerInfo;


        /// <summary>
        /// An event that is called when the game manager has finished loading a level.
        /// Parameters: the level number and the actual level object that has been loaded
        /// </summary>
        public static UnityAction<int, Level> OnLevelLoaded;




        [Header("References")]
        [SerializeField] new Camera camera;
        [SerializeField] UIManager uiManager;
        [SerializeField] FrameworkAPI frameworkAPI;
        [SerializeField] TestNetworkManager networkManager;


        [Header("Settings")]
        [SerializeField] int levelBuildIndexStart = 1;
        [SerializeField] Player playerPrefab = null; // TODO: move elsewhere (e.g. a constants/globals class?)


        /// <summary>
        /// Holds the currently played level.
        /// </summary>
        private Level currentLevel;
        /// <summary>
        /// Holds the currently loaded level scene.
        /// </summary>
        private Scene currentLevelScene;

        /// <summary>
        /// Holds the information about our own player.
        /// </summary>
        private PlayerInfo playerInfo;


        private void Awake()
        {
            if (GameManager.Instance == null)
            {
                // Init
                GameManager.Instance = this;
                this.currentLevel = null;
                SceneManager.sceneLoaded += this.SceneManager_SceneLoaded;

                this.uiManager.HideAllUI();
                this.uiManager.ShowLevelUI(); // TODO: Show level UI only if necessary


                // Load player data
                this.frameworkAPI.Load();
                this.playerInfo = new PlayerInfo(this.frameworkAPI.PlayerName, Random.ColorHSV());
            }
            else
                GameObject.Destroy(this.gameObject);
        }
        private void Start()
        {
            // SceneManager does not fire its SceneLoaded event for the currently active scene.
            // Meaning if we start from inside the Unity Editor and the opened level scene is the active one,
            // it won't be registered by SceneManager_SceneLoaded.
            // We need to check manually if the active scene is a level, set it as the current level and fire the OnLevelLoaded event
            // (basically mimic SceneManager_SceneLoaded's logic).
            if (this.currentLevel == null)
                if (this.TrySetCurrentLevelScene(SceneManager.GetActiveScene()))
                    GameManager.OnLevelLoaded?.Invoke(GameManager.CurrentLevelNum, GameManager.CurrentLevel);
        }

        /// <summary>
        /// Loads the level with the given level number.
        /// </summary>
        /// <param name="levelNum">The level number to load.</param>
        public void LoadLevel(int levelNum)
        {
            Debug.Log($"Loading level #{levelNum}");

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
                Debug.Log($"Unloading level: {this.currentLevel.gameObject.scene.name}");
                this.currentLevel.Unload();
                this.currentLevel = null;
                SceneManager.UnloadSceneAsync(this.currentLevelScene);
            }
        }


        /// <summary>
        /// Disconnects and quits the game by calling the game framework's <see cref="FrameworkAPI.HandleGameResults(int, string)"/>.
        /// NOTE: The game won't quit immediately because the framework waits until all clients have disconnected (if we are running a server).
        /// </summary>
        public void ExitGame()
        {
            if (NetworkClient.active)
                this.networkManager.StopClient();
            if (NetworkServer.active)
                this.networkManager.StopServer();

            this.frameworkAPI.HandleGameResults(1, this.playerInfo.Name);
        }


        /// <summary>
        /// Tries to find a <see cref="Level"/> component on the given scene's root game objects and set it as the current level (see <see cref="CurrentLevel"/>).
        /// Has no effect if the loaded scene is not a level.
        /// </summary>
        /// <param name="levelScene">The scene to try to set as the current level.</param>
        /// <returns>Whether the given scene contained a level and it was set as the current level.</returns>
        private bool TrySetCurrentLevelScene(Scene levelScene)
        {
            if (levelScene.isLoaded)
            {
                GameObject[] rootGOs = levelScene.GetRootGameObjects();
                Level levelComponent;

                foreach (GameObject rootGO in rootGOs)
                {
                    levelComponent = rootGO.GetComponent<Level>();

                    if (levelComponent != null)
                    {
                        this.currentLevel = levelComponent;
                        this.currentLevelScene = levelScene;

                        SceneManager.SetActiveScene(levelScene);

                        Debug.Log($"Current level is now: {this.currentLevel.gameObject.scene.name}");
                        if (this.currentLevel.isTutorial)
                        {
                            this.uiManager.showTutorialUI();
                        }
                        return true;
                    }
                }
            }
            else
                Debug.LogError($"Scene with index {levelScene.buildIndex} could not be loaded in time.");

            return false;
        }


        /// <summary>
        /// Called when a new scene is loaded by the SceneManager.
        /// Tries to set the loaded scene as the current level and fires the <see cref="OnLevelLoaded"/> event if successful.
        /// Has no effect if the loaded scene is not a level.
        /// </summary>
        /// <param name="scene">The loaded scene.</param>
        /// <param name="mode">The mode the given <paramref name="scene"/> has been loaded with.</param>
        private void SceneManager_SceneLoaded(Scene scene, LoadSceneMode mode)
        {
            if (this.TrySetCurrentLevelScene(scene))
                GameManager.OnLevelLoaded?.Invoke(GameManager.CurrentLevelNum, GameManager.CurrentLevel);
        }
    }
}
