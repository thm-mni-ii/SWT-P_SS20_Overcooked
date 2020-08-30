/* Created by: SWT-P_SS20_Overcooked (Team Drai Studios) */
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

        /// <summary>
        /// Shows tutorial UI.
        /// </summary>
        public void LoadTutorialUI()
        {
            tutorialUI.gameObject.SetActive(true);
            introUI.gameObject.SetActive(true);
        }
        /// <summary>
        /// Loads the next elements of the tutorial UI, unloads current element.
        /// </summary>
        public void LoadMovementUI()
        {
            introUI.gameObject.SetActive(false);
            movementUI.gameObject.SetActive(true);
        }
        /// <summary>
        /// Loads the next elements of the tutorial UI, unloads current element.
        /// </summary>
        public void LoadInteractionUI()
        {
            movementUI.gameObject.SetActive(false);
            interactionUI.gameObject.SetActive(true);
        }
        /// <summary>
        /// Loads the next elements of the tutorial UI, unloads current element.
        /// </summary>
        public void LoadTrashOrbUI()
        {
            interactionUI.gameObject.SetActive(false);
            trashOrbUI.gameObject.SetActive(true);
        }
        /// <summary>
        /// Loads the next elements of the tutorial UI, unloads current element.
        /// </summary>
        public void LoadExplain1UI()
        {
            trashOrbUI.gameObject.SetActive(false);
            explain1UI.gameObject.SetActive(true);
        }
        /// <summary>
        /// Loads the next elements of the tutorial UI, unloads current element.
        /// </summary>
        public void LoadExplain2UI()
        {
            explain1UI.gameObject.SetActive(false);
            explain2UI.gameObject.SetActive(true);

        }
        /// <summary>
        /// Loads the next elements of the tutorial UI, unloads current element.
        /// </summary>
        public void LoadExplain3UI()
        {
            explain2UI.gameObject.SetActive(false);
            explain3UI.gameObject.SetActive(true);

        }
        /// <summary>
        /// Loads the next elements of the tutorial UI, unloads current element.
        /// </summary>
        public void LoadCreateWaterUI()
        {
            explain3UI.gameObject.SetActive(false);
            createWaterUI.gameObject.SetActive(true);

        }
        /// <summary>
        /// Loads the next elements of the tutorial UI, unloads current element.
        /// </summary>
        public void LoadDelieverWaterUI()
        {
            createWaterUI.gameObject.SetActive(false);
            deliverWaterUI.gameObject.SetActive(true);

        }/// <summary>
         /// Unloads the tutorial UI.
         /// </summary>
        public void CloseTutorialUI()
        {
            deliverWaterUI.gameObject.SetActive(false);
            tutorialUI.gameObject.SetActive(false);
        }
    }
}

