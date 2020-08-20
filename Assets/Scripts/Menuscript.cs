/*using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Underconnected {

    /// <summary>
    /// 
    /// </summary>
    public class Menuscript : MonoBehaviour {
        Transform menuPanel;
        Event keyEvent;
        TextMesh buttonText;
        KeyCode newKey;

        bool waitingForKey;
        void Start() {
            menuPanel = transform.Find("Panel");
            menuPanel.gameObject.SetActive(false);
            waitingForKey = false;

            for (int i = 0; i < menuPanel.childCount; i++) {
                if (menuPanel.GetChild(i).name == "ForwardKey")
                    menuPanel.GetChild(i).GetComponentInChildren<TextMesh>().text = GameManager.Instance.forward.ToString();
                if (menuPanel.GetChild(i).name == "LeftKey")
                    menuPanel.GetChild(i).GetComponentInChildren<TextMesh>().text = GameManager.Instance.left.ToString();
                if (menuPanel.GetChild(i).name == "RightKey")
                    menuPanel.GetChild(i).GetComponentInChildren<TextMesh>().text = GameManager.Instance.right.ToString();
                if (menuPanel.GetChild(i).name == "BackwardKey")
                    menuPanel.GetChild(i).GetComponentInChildren<TextMesh>().text = GameManager.Instance.backward.ToString();
                if (menuPanel.GetChild(i).name == "ActionKey")
                    menuPanel.GetChild(i).GetComponentInChildren<TextMesh>().text = GameManager.Instance.action.ToString();
            }
        }

        // Update is called once per frame
        void Update() {
            if (Input.GetKeyDown(KeyCode.Escape) && !menuPanel.gameObject.activeSelf)
                menuPanel.gameObject.SetActive(true);
            else if (Input.GetKeyDown(KeyCode.Escape) && menuPanel.gameObject.activeSelf)
                menuPanel.gameObject.SetActive(false);
        }

        void OnGUI() {
            keyEvent = Event.current;

            if (keyEvent.isKey && waitingForKey) {
                newKey = keyEvent.keyCode;
                waitingForKey = false;
            }
        }

        public void StartAssignment(string keyName) {
            if (!waitingForKey)
                StartCoroutine(AssignKey(keyName));
        }

        public void SendText(TextMesh text) {
            buttonText = text;
        }

        IEnumerator WaitForKey() {
            while (!keyEvent.isKey)
                yield return null;
        }

        public IEnumerator AssignKey(string keyName) {
            waitingForKey = true;
            yield return WaitForKey();

            switch (keyName) {
                case "forward":
                    GameManager.Instance.forward = newKey;
                    buttonText.text = GameManager.Instance.forward.ToString();
                    PlayerPrefs.SetString("forwardKey", GameManager.Instance.forward.ToString());
                    break;

                case "backward":
                    GameManager.Instance.backward = newKey;
                    buttonText.text = GameManager.Instance.backward.ToString();
                    PlayerPrefs.SetString("backwardKey", GameManager.Instance.backward.ToString());
                    break;

                case "left":
                    GameManager.Instance.left = newKey;
                    buttonText.text = GameManager.Instance.left.ToString();
                    PlayerPrefs.SetString("leftKey", GameManager.Instance.left.ToString());
                    break;

                case "right":
                    GameManager.Instance.right = newKey;
                    buttonText.text = GameManager.Instance.right.ToString();
                    PlayerPrefs.SetString("rightKey", GameManager.Instance.right.ToString());
                    break;

                case "action":
                    GameManager.Instance.action = newKey;
                    buttonText.text = GameManager.Instance.action.ToString();
                    PlayerPrefs.SetString("actionKey", GameManager.Instance.action.ToString());
                    break;

            }

            yield return null;
        }

    }
}
*/