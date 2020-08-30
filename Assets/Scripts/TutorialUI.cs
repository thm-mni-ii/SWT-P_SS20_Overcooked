using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Underconnected
{
    public class TutorialUI : MonoBehaviour
    {
        [SerializeField] TutorialUI tutorialUI;
        [SerializeField] GameObject introUI;
        [SerializeField] GameObject movementUI;
        [SerializeField] GameObject interactionUI;
        [SerializeField] GameObject trashOrbUI;
        [SerializeField] GameObject explain1UI;
        [SerializeField] GameObject explain2UI;
        [SerializeField] GameObject explain3UI;
        [SerializeField] GameObject createWaterUI;
        [SerializeField] GameObject deliverWaterUI;

        public void LoadTutorialUI()
        {
            tutorialUI.gameObject.SetActive(true);
            introUI.gameObject.SetActive(true);
        }
        public void LoadMovementUI(){
            introUI.gameObject.SetActive(false);
            movementUI.gameObject.SetActive(true);
        }
        public void LoadInteractionUI(){
            movementUI.gameObject.SetActive(false);
            interactionUI.gameObject.SetActive(true);
        }
        public void LoadTrashOrbUI(){
            interactionUI.gameObject.SetActive(false);
            trashOrbUI.gameObject.SetActive(true);
        }
        public void LoadExplain1UI(){
            trashOrbUI.gameObject.SetActive(false);
            explain1UI.gameObject.SetActive(true);
        }
        public void LoadExplain2UI(){
            explain1UI.gameObject.SetActive(false);
            explain2UI.gameObject.SetActive(true);

        }
        public void LoadExplain3UI(){
            explain2UI.gameObject.SetActive(false);
            explain3UI.gameObject.SetActive(true);

        }
        public void LoadCreateWaterUI(){
            explain3UI.gameObject.SetActive(false);
            createWaterUI.gameObject.SetActive(true);

        }
        public void LoadDelieverWaterUI(){
            createWaterUI.gameObject.SetActive(false);
            deliverWaterUI.gameObject.SetActive(true);

        }
        public void CloseTutorialUI()
        {
            deliverWaterUI.gameObject.SetActive(false);
            tutorialUI.gameObject.SetActive(false);
        }
        



    }
}

