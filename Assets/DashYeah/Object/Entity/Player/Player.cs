using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;

namespace DashYeah.Object.Entity.Player
{
    public class Player : Entity
    {
        [SerializeField] private float movementSpeed = 30.0f;

        private Rigidbody rb;
        private PlayerInputActions input;
        private Vector2 inputMovement;

        #region Engine

        protected override void Awake()
        {
            base.Awake();
            input = new PlayerInputActions();
            rb = GetComponent<Rigidbody>();
        }

        

        protected override void OnEnable()
        {
            base.OnEnable();
            input.Enable();
            input.InGame.Movement.performed += OnMovementInputPerformed;
            input.InGame.Movement.canceled += OnMovementInputCanceled;
        }

        protected override void Start()
        {
            base.Start();
        }

        protected override void OnDisable()
        {
            base.OnDisable();
            input.Disable();
            input.InGame.Movement.performed -= OnMovementInputPerformed;
            input.InGame.Movement.canceled -= OnMovementInputCanceled;
        }

        protected override void Update()
        {
            base.Update();
        }

        private void FixedUpdate()
        {
            Vector3 movementDirection = new Vector3(inputMovement.x, 0.0f, inputMovement.y);
            rb.velocity = movementSpeed * Time.deltaTime * movementDirection;
        }

        #endregion // Engine

        #region Events

        private void OnMovementInputPerformed(InputAction.CallbackContext value)
        {
            inputMovement = value.ReadValue<Vector2>();
        }

        private void OnMovementInputCanceled(InputAction.CallbackContext value)
        {
            inputMovement = value.ReadValue<Vector2>();
        }

        #endregion // Events
    }
}