using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Mirror;

namespace Underconnected
{
    /// <summary>
    /// Allows the player to move the game object this component is on.
    /// </summary>
    [RequireComponent(typeof(Player), typeof(Interactor))]
    public class PlayerControls : NetworkBehaviour
    {
        [SerializeField] Rigidbody rigidBody;
        [SerializeField] MeshRenderer playerModelRenderer;
        [SerializeField] Player player;
        [SerializeField] Interactor interactor;
        [SerializeField] float moveSpeed = 100.0F;
        [SerializeField] float rotationSpeed = 500.0F;


        /// <summary>
        /// The player these controls belong to.
        /// </summary>
        public Player Player => this.player;
        /// <summary>
        /// Tells whether the controls are enabled for this player.
        /// </summary>
        public bool ControlsEnabled { get; private set; }


        /// <summary>
        /// Stores the input from <see cref="Update"/> here to move the player inside of <see cref="FixedUpdate"/>.
        /// </summary>
        private Vector3 movementInput;
        /// <summary>
        /// The target yaw rotation to interpolate the current rotation.
        /// Used to animate the player's rotation.
        /// </summary>
        private float targetYaw;

        /// <summary>
        /// This player's color.
        /// </summary>
        [SyncVar(hook = nameof(UpdatePlayerColor))]
        private Color playerColor;


        private void Awake()
        {
            this.movementInput = Vector3.zero;
            this.EnableControls();
        }

        private void Start()
        {
            this.targetYaw = this.transform.rotation.eulerAngles.y;
        }
        public override void OnStartServer()
        {
            this.playerColor = Random.ColorHSV();
        }
        public override void OnStartClient()
        {
            //this.rigidBody.isKinematic = !this.hasAuthority;
        }

        /// <summary>
        /// Listens for player input and stores it to move the player inside of <see cref="FixedUpdate"/>.
        /// Called every frame.
        /// </summary>
        private void Update()
        {
            if (this.player.IsOwnPlayer)
            {
                this.movementInput.x = this.ControlsEnabled ? Input.GetAxisRaw("Horizontal") : 0;
                this.movementInput.y = 0;
                this.movementInput.z = this.ControlsEnabled ? Input.GetAxisRaw("Vertical") : 0;

                if (this.movementInput.sqrMagnitude > Mathf.Epsilon)
                    this.targetYaw = Vector3.SignedAngle(this.movementInput, Vector3.forward, Vector3.down);

                if (this.ControlsEnabled && Input.GetKeyDown(KeyCode.E))
                    this.interactor.Interact();
            }
        }
        /// <summary>
        /// Moves the player with the input values previously recorded inside of <see cref="Update"/>.
        /// The player needs to be moved here because Unity's physics system expects rigidbodies to move inside of FixedUpdate instead of regular Update.
        /// </summary>
        private void FixedUpdate()
        {
            if (this.player.IsOwnPlayer)
            {
                Vector3 currentRotation = this.rigidBody.rotation.eulerAngles;
                float yawDelta = this.targetYaw - currentRotation.y;

                if (Mathf.Abs(yawDelta) > Mathf.Epsilon)
                    this.rigidBody.MoveRotation(Quaternion.Euler(0.0F, Mathf.MoveTowardsAngle(currentRotation.y, this.targetYaw, this.rotationSpeed * Time.fixedDeltaTime), 0.0F));

                this.rigidBody.AddForce(this.movementInput.normalized * this.moveSpeed);
            }
        }


        /// <summary>
        /// Enables the player controls.
        /// </summary>
        public void EnableControls() => this.ControlsEnabled = true;
        /// <summary>
        /// Disables the player controls.
        /// </summary>
        public void DisableControls() => this.ControlsEnabled = false;


        /// <summary>
        /// Called when the value of <see cref="playerColor"/> changes on the server.
        /// Updates the player color on clients to synchronize it with the server.
        /// </summary>
        /// <param name="oldColor">The previous player color.</param>
        /// <param name="newColor">The new player color.</param>
        private void UpdatePlayerColor(Color oldColor, Color newColor)
        {
            this.playerModelRenderer.material.color = newColor;
        }
    }
}
