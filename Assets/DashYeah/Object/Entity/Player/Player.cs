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
            Vector2 inputMovement = input.InGame.Movement.ReadValue<Vector2>();

            // Get the input direction
            velocity = new Vector3(inputMovement.x, velocity.y, inputMovement.y);

            // Add gravity
            if (characterController.isGrounded && velocity.y < 0.0f)
                velocity.y = -2.0f;

            velocity.y += gravity * Time.deltaTime;

            // Apply the final movement vector
            characterController.Move(movementSpeed * Time.deltaTime * velocity);
        }

        #endregion // Engine

        #region Events



        #endregion // Events
    }
}