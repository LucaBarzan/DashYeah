using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;

namespace DashYeah.Object.Entity.Player
{
    public class Player : Entity
    {
        [Header("Movement")]
        [SerializeField] private float movementSpeed = 30.0f;
        [SerializeField] private float jumpHeight = 5.0f;
        [SerializeField] private float gravityScale = 5.0f;

        private CharacterController characterController;
        private PlayerInputActions input;

        // Movement Variables
        private Vector3 velocity;
        private float gravity;
        private float initialJumpVelocity;
        private const float GROUNDED_GRAVITY = -0.1f;

        #region Engine

        protected override void Awake()
        {
            base.Awake();
            input = new PlayerInputActions();
            characterController = GetComponent<CharacterController>();
            SetupMovementVariables();
        }

        protected override void OnEnable()
        {
            base.OnEnable();
            input.InGame.Enable();
            input.InGame.Jump.performed += OnJumpInput_Performed;
        }

        protected override void Start()
        {
            base.Start();
        }

        protected override void OnDisable()
        {
            base.OnDisable();
            input.InGame.Disable();
        }

        protected override void Update()
        {
            base.Update();

#if UNITY_EDITOR
            // Update the inspector values every frame to make it easier for the Designers to tweek the values in real time
            SetupMovementVariables();
#endif
            HandleMovement();
        }

        #endregion // Engine

        #region Events

        private void OnJumpInput_Performed(InputAction.CallbackContext context)
        {
            Jump();
        }

        #endregion // Events

        #region Movement

        private void SetupMovementVariables()
        {
            gravity = Physics.gravity.y * gravityScale;
            initialJumpVelocity = Mathf.Sqrt(jumpHeight * -2.0f * gravity);
        }

        private void HandleMovement()
        {
            // Get the input direction
            Vector2 movementDirection = input.InGame.Movement.ReadValue<Vector2>();

            // Add speed to the movement direction
            movementDirection *= movementSpeed;

            // Apply the new movement direction to the velocity
            velocity = new Vector3(movementDirection.x, velocity.y, movementDirection.y);

            // Apply a default little gravity if it is grounded
            if (characterController.isGrounded && velocity.y < 0.0f)
            {
                velocity.y = GROUNDED_GRAVITY;
            }

            // Add gravity to the velocity
            velocity.y += gravity * Time.deltaTime;

            // Apply the velocity
            characterController.Move(velocity * Time.deltaTime);
        }

        private void Jump()
        {
            // Only can jump if the player is on the ground
            if (!characterController.isGrounded)
                return;

            // Add gravity to the velocity
            velocity.y = initialJumpVelocity;
        }

        #endregion // Movement
    }
}