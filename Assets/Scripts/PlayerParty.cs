using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;

namespace Underconnected 
{
    public class PlayerParty : MonoBehaviour 
    {
        [Header("Players")]
        [Tooltip("Name")]
        [SerializeField] TextMeshProUGUI playerOneName;
        [SerializeField] Image playerOneColor;
        [SerializeField] TextMeshProUGUI playerTwoName;
        [SerializeField] Image playerTwoColor;
        [SerializeField] TextMeshProUGUI playerThreeName;
        [SerializeField] Image playerThreeColor;
        [SerializeField] TextMeshProUGUI playerFourName;
        [SerializeField] Image playerFourColor;

        string[] names = new string[5];
        Color[] colors = new Color[5];
        //string[] textfields = new string[] { "playerOneName", "playerTwoName", "playerThreeName", "playerFourName" };

        void Start() {
            GameManager.NetworkManager.OnClientJoin += NetworkManager_OnClientJoin;
            GameManager.NetworkManager.OnClientLeave += NetworkManager_OnClientLeave;
        }

        private void NetworkManager_OnClientJoin(PlayerConnection player) {
            switch (GameManager.NetworkManager.AllClients.Count) {
                case 1:
                    names[0] = player.PlayerInfo.Name;
                    playerOneName.SetText(names[0]);
                    colors[0] = player.PlayerInfo.Color;
                    playerOneColor.GetComponent<Image>().color = colors[0];
                    break;
                case 2:
                    names[1] = player.PlayerInfo.Name;
                    playerTwoName.SetText(names[1]);
                    colors[1] = player.PlayerInfo.Color;
                    playerTwoColor.GetComponent<Image>().color = colors[1];
                    break;
                case 3:
                    names[2] = player.PlayerInfo.Name;
                    playerThreeName.SetText(names[2]);
                    colors[2] = player.PlayerInfo.Color;
                    playerThreeColor.GetComponent<Image>().color = colors[2];
                    break;
                case 4:
                    names[3] = player.PlayerInfo.Name;
                    playerFourName.SetText(names[3]);
                    colors[3] = player.PlayerInfo.Color;
                    playerFourColor.GetComponent<Image>().color = colors[3];
                    break;
            }
        }

        private void NetworkManager_OnClientLeave(PlayerConnection player) {
            playerFourName.SetText(" ");
            playerFourColor.GetComponent<Image>().color = new Color(0f, 0f, 0f, 0f);

            Debug.Log($"Count after leave from {player.PlayerInfo.Name}: {GameManager.NetworkManager.AllClients.Count}");

            if (player.PlayerInfo.Name == names[0])  //player one leaves
                {
                colors[0] = new Color(0f, 0f, 0f, 0f);
                for (int i = 3; i >= GameManager.NetworkManager.AllClients.Count; i--) {
                    names[i+1] = "";
                    colors[i+1] = new Color(0f, 0f, 0f, 0f);
                }
                playerOneName.SetText(names[1]);
                playerOneColor.GetComponent<Image>().color = colors[1];
                names[0] = names[1];
                colors[0] = colors[1];

                playerTwoName.SetText(names[2]);
                playerTwoColor.GetComponent<Image>().color = colors[2];
                names[1] = names[2];
                colors[1] = colors[2];

                playerThreeName.SetText(names[3]);
                playerThreeColor.GetComponent<Image>().color = colors[3];
                names[2] = names[3];
                colors[2] = colors[3];

                } else {
                if (player.PlayerInfo.Name == names[1]) { //player two leaves
                    for (int i = 3; i >= GameManager.NetworkManager.AllClients.Count; i--) {
                        names[i + 1] = "";
                        colors[i + 1] = new Color(0f, 0f, 0f, 0f);
                    }
                    playerTwoName.SetText(names[2]);
                    playerTwoColor.GetComponent<Image>().color = colors[2];
                    names[1] = names[2];
                    colors[1] = colors[2];

                    playerThreeName.SetText(names[3]);
                    playerThreeColor.GetComponent<Image>().color = colors[3];
                    names[2] = names[3];
                    colors[2] = colors[3];

                } else {
                    if (player.PlayerInfo.Name == names[2])  //player three leaves
                    {  //4 players before
                        names[2] = "";
                        colors[2] = new Color(0f, 0f, 0f, 0f);
                        playerThreeName.SetText(names[3]);
                        playerThreeColor.GetComponent<Image>().color = colors[3];
                        names[2] = names[3];
                        colors[2] = colors[3];
                    }
                }
            }

            names[3] = "";
            colors[3] = new Color(0f, 0f, 0f, 0f);
        }
    }
}
