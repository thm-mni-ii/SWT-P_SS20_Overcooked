using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Underconnected
{
    /// <summary>
    /// Represents a table with interactable drawers.
    /// </summary>
    public class DrawersTable : MonoBehaviour, IInteractable
    {
        [Header("References")]
        [Tooltip("The drawer animator.")]
        [SerializeField] Animator drawerAnimator;

        [Header("Settings")]
        [Tooltip("Whether the drawer should be opened at level start.")]
        [SerializeField] bool isDrawerOpened = false;
        [Tooltip("Whether players are allowed to interact with this drawer.")]
        [SerializeField] bool canBeInteractedWith = false;
        [Tooltip("The animation parameter controlling opening/closing a drawer.")]
        [SerializeField] string isOpenedAnimParameter = "IsOpen";
        [Tooltip("The animation to play on level start when the drawer is closed.")]
        [SerializeField] string drawerClosedState = "close_idle";
        [Tooltip("The animation to play on level start when the drawer is open.")]
        [SerializeField] string drawerOpenedState = "open_idle";


        /// <summary>
        /// Tells whether this table's drawer is closed.
        /// </summary>
        public bool IsDrawerClosed => !this.isDrawerOpened;
        /// <summary>
        /// Tells whether this table's drawer is opened.
        /// </summary>
        public bool IsDrawerOpened => this.isDrawerOpened;


        /// <summary>
        /// Play the correct animation on level start based on whether the drawer should be opened or not.
        /// </summary>
        private void Start()
        {
            this.drawerAnimator.SetBool(this.isOpenedAnimParameter, this.isDrawerOpened);

            if (this.isDrawerOpened)
                this.drawerAnimator.Play(this.drawerOpenedState);
            else
                this.drawerAnimator.Play(this.drawerClosedState);
        }


        /// <summary>
        /// Opens the drawer.
        /// </summary>
        public void OpenDrawer()
        {
            if (this.IsDrawerClosed)
            {
                this.isDrawerOpened = true;
                this.drawerAnimator.SetBool(this.isOpenedAnimParameter, this.isDrawerOpened);
            }
        }
        /// <summary>
        /// Closes the drawer.
        /// </summary>
        public void CloseDrawer()
        {
            if (this.IsDrawerOpened)
            {
                this.isDrawerOpened = false;
                this.drawerAnimator.SetBool(this.isOpenedAnimParameter, this.isDrawerOpened);
            }
        }
        /// <summary>
        /// Toggles the drawer. Closes it when it's open and vice versa.
        /// </summary>
        public void ToggleDrawer()
        {
            if (this.IsDrawerOpened)
                this.CloseDrawer();
            else
                this.OpenDrawer();
        }

        /// <summary>
        /// Interacts with this drawer by toggling it (<see cref="ToggleDrawer"/>).
        /// </summary>
        /// <param name="interactor">The initiator of this interaction.</param>
        public void Interact(Interactor interactor) => this.ToggleDrawer();
    }
}
