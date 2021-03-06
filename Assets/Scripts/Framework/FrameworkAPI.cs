﻿/* Created by: SWT-P_SS20_Framework */
/* Edited by: SWT-P_SS20_Overcooked (Team Drai Studios) */
using System;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using System.Runtime.InteropServices;
using Mirror;
using Mirror.Websocket;
using SimpleJSON;
using UnityEngine;
using UnityEngine.UI;
using Underconnected;

namespace GameFramework
{
    /// <summary>
    /// This class represents a game server.
    /// The server starts automatically and loads the player prefab into the online scene.
    /// </summary>
    public class FrameworkAPI : MonoBehaviour
    {
        /// <summary>
        /// Location of the file that holds playerInfo of
        /// the player that started the game.
        /// NOTE: This is not important for development.
        /// </summary>
        private const string FILE_NAME = "Info.txt";

        /// <summary>
        /// Defines how long the game server will try to connect the client before quitting the application.
        /// </summary>
        [SerializeField] private float disconnectWaitTime;

        /// <summary>
        /// Tells whether we should use the mirror HUD.
        /// `true` for testing purposes, `false` for production.
        /// </summary>
        [SerializeField] private bool useMirrorHUD;
        /// <summary>
        /// The reference to Mirror's network HUD.
        /// </summary>
        [SerializeField] private NetworkManagerHUD mirrorHUD;

        /// <summary>
        /// Stores the time until disconnect.
        /// </summary>
        private float disconnectTimer;

        /// <summary>
        /// Stores information about the player.
        /// </summary>
        private JSONNode playerInfos;

        /// <summary>
        /// Stores information about the game.
        /// </summary>
        private JSONNode gameInfos;

        /// <summary>
        /// Stores if the local player is the host.
        /// </summary>
        private bool isHost;

        /// <summary>
        /// Signals if the game can be quit.
        /// </summary>
        private bool readyToQuit;

        public JSONNode PlayerInfos => playerInfos;
        public JSONNode GameInfos => gameInfos;

        public bool IsHost => this.isHost;
        public string PlayerName => this.playerInfos != null ? this.playerInfos["name"].Value : null;


        /// <summary>
        /// Loads the player data.
        /// </summary>
        public void Load()
        {
            readyToQuit = false;
            isHost = false;

            //LoadPlayerInfoMockup();     // <- FOR DEVELOPMENT

            LoadPlayerInfo();             // <- FOR RELEASE
        }

        /// <summary>
        /// The server loads the player info of the local player, if:
        /// - the player is the host -> start server as host
        /// - the player is the client -> join server as client
        /// NOTE: This is not important for development.
        /// </summary>
        void Start()
        {
            if (!this.useMirrorHUD)
            {
                this.mirrorHUD.enabled = false;

                if (isHost)
                {
                    GameManager.NetworkManager.StartHost();
                }
                else
                {
                    disconnectTimer = disconnectWaitTime;
                }
            }
            else
                this.mirrorHUD.enabled = true;
        }

        /// <summary>
        /// Update is called once per frame.
        /// Checks if the local player is a client and not connected.
        /// If so the client tries to connect and the disconnect timer is updated.
        /// </summary>
        private void Update()
        {
            if (!this.useMirrorHUD)
            {
                // Try connecting if not host
                if (!isHost && !NetworkClient.isConnected)
                {
                    disconnectTimer -= Time.deltaTime;

                    if (disconnectTimer <= 0f)
                    {
                        Application.Quit();
                    }

                    GameManager.NetworkManager.StartClient();
                }
            }

            // Try quitting
            if (readyToQuit)
            {
                if (isHost)
                {
                    // Host waits for all players to quit first
                    if (NetworkServer.connections.Count <= 1)
                    {
                        Application.Quit();
                    }
                }
                else
                {
                    Application.Quit();
                }
            }
        }

        /// <summary>
        /// Loads player info from file.
        /// NOTE: This is not important for development.
        /// </summary>
        private void LoadPlayerInfo()
        {
            // Read file
            string filePath = "";
            switch (SystemInfo.operatingSystemFamily)
            {
                case OperatingSystemFamily.Windows:
                    filePath = Application.dataPath + @"\..\..\..\..\Framework\Windows\" + FILE_NAME;
                    break;
                case OperatingSystemFamily.Linux:
                    filePath = Application.dataPath + @"\..\..\..\..\Framework\Linux\" + FILE_NAME;
                    break;
                case OperatingSystemFamily.MacOSX:
                    filePath = Application.dataPath + @"\..\..\..\..\Framework\MACOSX\" + FILE_NAME;
                    break;
                default:
                    throw new ArgumentException("Illegal OS !");
            }

            if (File.Exists(filePath))
            {
                StreamReader file = new StreamReader(filePath);
                JSONNode jsonFile = JSON.Parse(file.ReadLine());

                // Load data
                isHost = jsonFile["playerInfo"]["isHost"].AsBool;
                playerInfos = jsonFile["playerInfo"];
                gameInfos = jsonFile["gameInfo"];

                // Close file
                file.Close();
                File.Delete(FILE_NAME);
            }
            else
            {
                this.useMirrorHUD = true;
                this.LoadPlayerInfoMockup();
            }
        }

        /// <summary>
        /// Sets mockup player info.
        /// The actual info is irrelevant. It does NOT matter if the info says the player is host.
        /// This will become relevant when using loadPlayerInfo.
        /// NOTE: This is important for development.
        /// </summary>
        private void LoadPlayerInfoMockup()
        {
            isHost = true;
            playerInfos = new JSONObject();
            playerInfos.Add("name", Utils.GetRandomPlayerName(UnityEngine.Random.Range(4, 8)));
        }

        /// <summary>
        /// Handles the results of the game and sets the readyToQuit flag.
        /// Needs the placement of the local player and the name of the winning player
        /// and writes them into a JSON that is used to give rewards in the framework.
        /// IMPORTANT: THIS HAS TO BE CALLED AT THE END OF THE GAME!
        /// </summary>
        /// <param name="localPlayerWinningPlacement">Placement of the local player</param>
        /// <param name="nameOfWinner">Name of the winner</param>
        public void HandleGameResults(int localPlayerWinningPlacement, string nameOfWinner)
        {
            // Create file
            if (File.Exists(FILE_NAME))
            {
                File.Delete(FILE_NAME);
            }

            var sr = File.CreateText(FILE_NAME);

            // Write file
            JSONObject fileJson = new JSONObject();

            fileJson.Add("placement", localPlayerWinningPlacement);
            fileJson.Add("nameOfWinner", nameOfWinner);

            sr.Write(fileJson.ToString());
            sr.Close();

            readyToQuit = true;
        }
    }
}
