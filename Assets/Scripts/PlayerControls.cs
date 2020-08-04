using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Mirror;

namespace Underconnected
{
    public class PlayerControls : NetworkBehaviour
    {
        [SerializeField] Rigidbody rigidBody;
        [SerializeField] MeshRenderer playerModelRenderer;
        [SerializeField] float moveSpeed = 100.0F;
        [SerializeField] float rotationSpeed = 500.0F;


        private Vector3 movementInput;
        private float targetYaw;

        [SyncVar(hook = nameof(UpdatePlayerColor))]
        private Color playerColor;


        private void Awake()
        {
            this.movementInput = Vector3.zero;
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

        private void Update()
        {
            if (this.isLocalPlayer)
            {
                this.movementInput.x = Input.GetAxisRaw("Horizontal");
                this.movementInput.y = 0;
                this.movementInput.z = Input.GetAxisRaw("Vertical");

                if (this.movementInput.sqrMagnitude > Mathf.Epsilon)
                    this.targetYaw = Vector3.SignedAngle(this.movementInput, Vector3.forward, Vector3.down);
            }
        }
        private void FixedUpdate()
        {
            if (this.isLocalPlayer)
            {
                Vector3 currentRotation = this.rigidBody.rotation.eulerAngles;
                float yawDelta = this.targetYaw - currentRotation.y;

                if (Mathf.Abs(yawDelta) > Mathf.Epsilon)
                    this.rigidBody.MoveRotation(Quaternion.Euler(0.0F, Mathf.MoveTowardsAngle(currentRotation.y, this.targetYaw, this.rotationSpeed * Time.fixedDeltaTime), 0.0F));

                this.rigidBody.AddForce(this.movementInput.normalized * this.moveSpeed);
            }
        }

        private void UpdatePlayerColor(Color oldColor, Color newColor)
        {
            this.playerModelRenderer.material.color = newColor;
        }
    }
}
