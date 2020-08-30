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

        int numOfPlayers;
        bool playerOneExists = false;
        bool playerTwoExists = false;
        bool playerThreeExists = false;
        bool playerFourExists = false;

        // Start is called before the first frame update
        void Start() {
            setPlayerInfos();
        }

        // Update is called once per frame
        void Update() {
        }

        public void setPlayerInfos() {

            playerThreeName.SetText("xXx--SniperZZZkiler370-xXx");
            playerThreeColor.GetComponent<Image>().color = new Color(1f, 110f, 0f);

        }
    }
}
