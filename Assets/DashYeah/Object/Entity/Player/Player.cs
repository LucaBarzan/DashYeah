using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;

namespace DashYeah.Object.Entity.Player
{
    public class Player : Entity
    {
        public float test = -2.0f;
        [SerializeField] float gravity = -9.81f;
        [SerializeField] private float movementSpeed = 30.0f;

        private CharacterController characterController;
        private PlayerInputActions input;
        private Vector3 velocity;
        private const float GROUNDED_GRAVITY = -0.1f;
        #region Engine

        protected override void Awake()
        {
            base.Awake();
            input = new PlayerInputActions();
            characterController = GetComponent<CharacterController>();
        }

        

        protected override void OnEnable()
        {
            base.OnEnable();
            input.InGame.Enable();
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
            
        }

        private void FixedUpdate()
        {
            ApplyMovement();
        }

        #endregion // Engine

        #region Events



        #endregion // Events

        #region Movement

        private void ApplyMovement()
        {
            // Get the input direction
            Vector2 movementDirection = input.InGame.Movement.ReadValue<Vector2>();

            // Add speed to the movement direction
            movementDirection *= movementSpeed;

            // Apply the new movement direction to the velocity
            velocity = new Vector3(movementDirection.x, velocity.y, movementDirection.y);

            // Apply a default little gravity if it is grounded
            if (characterController.isGrounded)
            {
                velocity.y = GROUNDED_GRAVITY;
            }

            // Add gravity to the velocity
            velocity.y += gravity * Time.deltaTime * Time.deltaTime;

            // Apply the velocity
            characterController.Move(velocity);
        }

        #endregion // Movement
    }
}