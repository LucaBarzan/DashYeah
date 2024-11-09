using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;

public class Player : Entity
{
    public override bool CanWalkOnOneWay => true;//!IsJumping;

    [SerializeField] private PlayerMovement playerMovement;

    // Components
    private CharacterMovement movement;
    private PlayerInputActions input;

    // Input
    Vector2 inputDirection;

    #region Engine

    protected override void Awake()
    {
        base.Awake();
        input = new PlayerInputActions();
        movement = GetComponent<CharacterMovement>();
        playerMovement.Setup(this);
    }

    private void OnEnable()
    {
        input.InGame.Enable();
        SubscribeToInputEvents();
    }

    private void OnDisable()
    {
        SubscribeToInputEvents(false);
        input.InGame.Disable();
    }

    private void Update()
    {
        HandleInputValue();
        playerMovement.Update();
    }

    #endregion Engine

    #region Events

    private void OnJumpInput_Performed(InputAction.CallbackContext context)
    {
        playerMovement.Jump();
    }

    private void OnDashInput_Performed(InputAction.CallbackContext context)
    {
        playerMovement.Dash();
    }

    #endregion Events

    #region Getters

    public Vector2 GetInputDirection()
    {
        return inputDirection;
    }

    #endregion Getters

    #region Input

    void SubscribeToInputEvents(bool subscribe = true)
    {
        if (subscribe)
        {
            // input.InGame.Jump.performed += OnJumpInput_Performed;
            // input.InGame.Dash.performed += OnDashInput_Performed;
        }
        else
        {
            // input.InGame.Jump.performed -= OnJumpInput_Performed;
            // input.InGame.Dash.performed -= OnDashInput_Performed;
        }
    }

    void HandleInputValue()
    {
        // inputDirection = input.InGame.Movement.ReadValue<Vector2>();
    }

    #endregion
}