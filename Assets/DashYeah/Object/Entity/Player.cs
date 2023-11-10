using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;
using DashYeah.Components.Player;

namespace DashYeah.Object.Entity
{
    public class Player : Entity
    {
        [SerializeField] private PlayerMovement playerMovement;

        // Components
        private CharacterController characterController;
        private PlayerInputActions input;

        // Input
        Vector2 inputDirection;

        #region Engine

        protected override void Awake()
        {
            base.Awake();
            input = new PlayerInputActions();
            characterController = GetComponent<CharacterController>();
            playerMovement.Setup(this);
        }

        protected override void OnEnable()
        {
            base.OnEnable();
            input.InGame.Enable();
            SubscribeToInputEvents();
        }

        protected override void Start()
        {
            base.Start();
        }

        protected override void OnDisable()
        {
            base.OnDisable();
            SubscribeToInputEvents(false);
            input.InGame.Disable();
        }

        protected override void Update()
        {
            base.Update();

            HandleInputValue();
            playerMovement.Update();
        }

        #endregion // Engine

        #region Events

        private void OnJumpInput_Performed(InputAction.CallbackContext context)
        {
            playerMovement.Jump();
        }

        private void OnDashInput_Performed(InputAction.CallbackContext context)
        {
            playerMovement.Dash();
        }

        #endregion // Events

        #region Getters

        public CharacterController GetCharacterController()
        {
            return characterController;
        }

        public Vector2 GetInputDirection()
        {
            return inputDirection;
        }

        #endregion // Getters

        #region Input

        void SubscribeToInputEvents(bool subscribe = true)
        {
            if(subscribe)
            {
                input.InGame.Jump.performed += OnJumpInput_Performed;
                input.InGame.Dash.performed += OnDashInput_Performed;
            }
            else
            {
                input.InGame.Jump.performed -= OnJumpInput_Performed;
                input.InGame.Dash.performed -= OnDashInput_Performed;
            }
        }

        void HandleInputValue()
        {
            inputDirection = input.InGame.Movement.ReadValue<Vector2>();
        }

        #endregion
    }
}