using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using DG.Tweening;

public class CrushingBlock : Platform
{
    #region Variables

    /* Private */
    [SerializeField] private Animator animator;
    [SerializeField] private float shakeTime = 0.2f;
    [SerializeField] private float groundedTime = 1.0f;
    [SerializeField] private float fallMaxSpeed = 10.0f;
    [SerializeField] private float fallAcceleration = 10.0f;
    [SerializeField] private float returnSpeed = 5.0f;

    private float shakeTimer = 0f;
    private float groundedTimer = 0f;

    private Vector3 originalPosition;
    private Vector2 fallVelocity;
    private new Rigidbody2D rigidbody2D;

    #endregion // Variables

    #region Constants & Read Only

    private const int STATE_IDLE = 0;
    private const int STATE_SHAKING = 1;
    private const int STATE_FALLING = 2;
    private const int STATE_GROUNDED = 3;
    private const int STATE_RETURNING = 4;

    private readonly StateMachine stateMachine = new StateMachine();

    #endregion // Constants & Read Only

    #region Engine

    protected override void Awake()
    {
        base.Awake();
        SetupVariables();
        SetupStateMachine();
    }

    protected override void Start()
    {
        base.Start();
        stateMachine.CurrentState = STATE_IDLE;
    }

    private void Update()
    {
        stateMachine.UpdateState();
    }

    private void FixedUpdate()
    {
        stateMachine.FixedUpdateState();
        FixedUpdateOthersRigidbodies();
    }

    #endregion // Engine

    #region Setup

    private void SetupVariables()
    {
        rigidbody2D = GetComponent<Rigidbody2D>();
        rigidbody2D.bodyType = RigidbodyType2D.Dynamic;
        stateMachine.Setup(this);
        animator.enabled = false;
    }

    private void SetupStateMachine()
    {
        stateMachine.AddEnterState(STATE_IDLE, Enter_Idle);

        stateMachine.AddEnterState(STATE_SHAKING, Enter_Shaking);
        stateMachine.AddUpdateState(STATE_SHAKING, Update_Shaking);
        stateMachine.AddExitState(STATE_SHAKING, Exit_Shaking);

        stateMachine.AddEnterState(STATE_FALLING, Enter_Falling);
        stateMachine.AddFixedUpdateState(STATE_FALLING, FixedUpdate_Falling);
        stateMachine.AddExitState(STATE_FALLING, Exit_Falling);

        stateMachine.AddEnterState(STATE_GROUNDED, Enter_Grounded);
        stateMachine.AddUpdateState(STATE_GROUNDED, Update_Grounded);
        stateMachine.AddExitState(STATE_GROUNDED, Exit_Grounded);

        stateMachine.AddEnterState(STATE_RETURNING, Enter_Returning);
        stateMachine.AddFixedUpdateState(STATE_RETURNING, FixedUpdate_Returning);
        stateMachine.AddExitState(STATE_RETURNING, Exit_Returning);
    }

    #endregion // Setup

    #region Core

    private void FixedUpdateOthersRigidbodies()
    {
        if (characterMovement != null)
        {
            // CharacterMovement.AddBaseForce(rigidbody2D.velocity);
        }

        for (int i = 0; i < othersRigidbody2D.Count; i++)
        {
            othersRigidbody2D[i].AddForce(rigidbody2D.linearVelocity);
        }
    }

    #endregion // Core

    #region Events

    public void OnFall()
    {
        if (stateMachine.CurrentState == STATE_IDLE)
            stateMachine.CurrentState = STATE_SHAKING;
    }

    public void OnGroundEnter(Collider2D collider2D)
    {
        if (stateMachine.CurrentState == STATE_FALLING)
            stateMachine.CurrentState = STATE_GROUNDED;
    }

    #endregion Events

    #region State machine

    #region Idle State

    private void Enter_Idle() => rigidbody2D.linearVelocity = Vector2.zero;

    #endregion Idle State

    #region Shaking State

    private void Enter_Shaking()
    {
        animator.enabled = true;
        shakeTimer = Time.time + shakeTime;
    }

    private void Update_Shaking()
    {
        if (Time.time >= shakeTimer)
            stateMachine.CurrentState = STATE_FALLING;
    }

    private void Exit_Shaking()
    {
        // Ensure the visual ends at the middle
        animator.transform.DOLocalMoveX(0.0f, 0.1f);
        animator.enabled = false;
    }

    #endregion Shaking State

    #region Falling State

    private void Enter_Falling()
    {
        originalPosition = Transform.position;
        fallVelocity = -Transform.up * fallMaxSpeed;
        rigidbody2D.linearVelocity = Vector2.zero;
    }

    private void FixedUpdate_Falling()
    {
        rigidbody2D.linearVelocity = Vector2.MoveTowards(rigidbody2D.linearVelocity, fallVelocity, fallAcceleration * Time.fixedDeltaTime);
    }

    private void Exit_Falling()
    {
        rigidbody2D.linearVelocity = Vector2.zero;
    }

    #endregion Falling State

    #region Grounded State

    private void Enter_Grounded()
    {
        groundedTimer = Time.time + groundedTime;
    }

    private void Update_Grounded()
    {
        if (Time.time >= groundedTimer)
            stateMachine.CurrentState = STATE_RETURNING;
    }

    private void Exit_Grounded()
    {

    }

    #endregion Grounded State

    #region Returning State

    private void Enter_Returning()
    {

    }

    private void FixedUpdate_Returning()
    {
        Vector2 originalPositionDirection = originalPosition - Transform.position;
        float dot = Vector2.Dot(Transform.up, originalPositionDirection);
        if (dot <= 0.0f)
        {
            stateMachine.CurrentState = STATE_IDLE;
            return;
        }

        rigidbody2D.linearVelocity = returnSpeed * Transform.up;
    }

    private void Exit_Returning()
    {

    }

    #endregion Returning State

    #endregion // State machine
}