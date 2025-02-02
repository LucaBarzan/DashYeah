using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;

public class Player : Character
{
    public override bool CanWalkOnOneWay => true;//!IsJumping;

    [SerializeField] private CharacterMovement movement;

    // Components
    private Input input;

    #region Engine

    protected override void Awake()
    {
        base.Awake();
        input = new Input();
    }

    protected override void OnEnable()
    {
        input.Player.Enable();
        movement.EnableMovement();
        SubscribeToInputEvents();
    }

    private void Update() => HandleInputValue();

    protected override void OnDisable()
    {
        SubscribeToInputEvents(false);
        movement.DisableMovement();
        input.Player.Disable();
    }

    #endregion Engine

    #region Events

    protected override void OnGameStateChanged(EGameState gameState)
    {
        // TODO
    }

    public override void OnTakeDamage(SDamageInfo damageInfo)
    {
        // TODO
    }

    public override void OnHealthEmpty()
    {
        // TODO
    }

    private void OnJumpInput_Performed(InputAction.CallbackContext context) => movement.OnJump_Perform();

    private void OnJumpInput_Canceled(InputAction.CallbackContext context) => movement.OnJump_Cancel();

    private void OnDashInput_Performed(InputAction.CallbackContext context)
    {
        // TODO
        // movement.Dash();
    }

    private void OnPlatformGoDownInput_Performed(InputAction.CallbackContext context)
    {
        // TODO
        // if (mauiMovement.IsGrounded && oneWayObject != null && mauiAbilities.CanDropFromOneWay)
        //     oneWayObject.GoDown();
    }

    public void OnInteracted_Ladder(Ladder ladder)
    {
        // TODO
    }



    #endregion Events

    #region Input

    void SubscribeToInputEvents(bool subscribe = true)
    {
        if (subscribe)
        {
            input.Player.Jump.performed += OnJumpInput_Performed;
            input.Player.Jump.canceled += OnJumpInput_Canceled;
            input.Player.Dash.performed += OnDashInput_Performed;
        }
        else
        {
            input.Player.Jump.performed -= OnJumpInput_Performed;
            input.Player.Jump.canceled -= OnJumpInput_Canceled;
            input.Player.Dash.performed -= OnDashInput_Performed;
        }
    }

    void HandleInputValue()
    {
        Vector3 movementDirection = input.Player.Movement.ReadValue<Vector2>();
        movementDirection = new Vector3 (movementDirection.x, 0.0f, movementDirection.y);

        movement.SetTargetDirection(movementDirection);
    }

    #endregion Input
}