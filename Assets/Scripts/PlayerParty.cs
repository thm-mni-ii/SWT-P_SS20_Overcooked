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
        
        void Start() {
            GameManager.NetworkManager.OnClientJoin += NetworkManager_OnClientJoin;
            GameManager.NetworkManager.OnClientLeave += NetworkManager_OnClientLeave;
        }

        private void NetworkManager_OnClientJoin(PlayerConnection player) {
            switch (GameManager.NetworkManager.AllClients.Count) {
                case 1:
                    playerOneName.SetText(player.PlayerInfo.Name);
                    playerOneColor.GetComponent<Image>().color = player.PlayerInfo.Color;
                    break;
                case 2:
                    playerTwoName.SetText(player.PlayerInfo.Name);
                    playerTwoColor.GetComponent<Image>().color = player.PlayerInfo.Color;
                    break;
                case 3:
                    playerThreeName.SetText(player.PlayerInfo.Name);
                    playerThreeColor.GetComponent<Image>().color = player.PlayerInfo.Color;
                    break;
                case 4:
                    playerFourName.SetText(player.PlayerInfo.Name);
                    playerFourColor.GetComponent<Image>().color = player.PlayerInfo.Color;
                    break;
            }
        }

        private void NetworkManager_OnClientLeave(PlayerConnection player) {
            
        }
    }
}
