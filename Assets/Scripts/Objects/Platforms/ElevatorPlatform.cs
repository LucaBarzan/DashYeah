using System.Collections;
using System.Collections.Generic;
using UnityEditor;
using UnityEngine;

[RequireComponent(typeof(Rigidbody2D))]
public class ElevatorPlatform : Platform
{
    [SerializeField] private float height = 5.0f;
    [SerializeField] private float elevatorSpeed = 2;
    [SerializeField] private float returnSpeed = 1;

    private float speed;

    private Vector2 initialPosition;
    private Vector2 targetPosition;
    private Vector2 currentPointPosition;
    private Vector2 direction;
    private Vector2 lastDirection;

    private Rigidbody2D myRigidbody2D;

    #region Constants & Read Only

    private int STATE_MOVING = 0;
    private int STATE_WAITING = 1;
    private readonly StateMachine stateMachine = new StateMachine();

    #endregion Constants & Read Only

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
        stateMachine.CurrentState = STATE_WAITING;
    }

    void Update()
    {
        stateMachine.UpdateState();
    }

    private void FixedUpdate()
    {
        stateMachine.FixedUpdateState();
        FixedUpdateOthersRigidbodies();
    }

    private void OnDrawGizmosSelected()
    {
        Gizmos.color = Color.gray;

        if(Application.isPlaying )
        {
            Gizmos.DrawRay(initialPosition, Vector2.up * height);
        }
        else
        {
            Gizmos.DrawRay(transform.position, Vector2.up * height);
        }
    }

    #endregion Engine

    #region Setup

    private void SetupVariables()
    {
        myRigidbody2D = GetComponent<Rigidbody2D>();
        stateMachine.Setup(this);

        lastDirection = direction;
        speed = elevatorSpeed;
        initialPosition = transform.position;
        targetPosition = new Vector2(initialPosition.x, initialPosition.y + height);
    }

    private void SetupStateMachine()
    {
        stateMachine.AddEnterState(STATE_MOVING, Enter_Moving);
        stateMachine.AddFixedUpdateState(STATE_MOVING, FixedUpdate_Moving);
        stateMachine.AddExitState(STATE_MOVING, Exit_Moving);

        stateMachine.AddEnterState(STATE_WAITING, Enter_Waiting);
        stateMachine.AddUpdateState(STATE_WAITING, Update_Waiting);
        stateMachine.AddExitState(STATE_WAITING, Exit_Waiting);
    }

    #endregion Setup

    #region Core

    private void FixedUpdateOthersRigidbodies()
    {
        Debug.DrawRay(transform.position, direction, Color.red);

        if (characterMovement != null && characterMovement.IsJumping == false)
        {
            characterMovement.AddForce(myRigidbody2D.linearVelocity);
        }

        for (int i = 0; i < othersRigidbody2D.Count; i++)
        {
            othersRigidbody2D[i].AddForce(myRigidbody2D.linearVelocity);
        }
    }

    private void CheckDirection()
    {

        // Set the direction to the initial position by default
        direction = (initialPosition - targetPosition).normalized;
        speed = returnSpeed;
        currentPointPosition = initialPosition;

        // If there is something on the elevator, go to the target position
        if (characterMovement != null || othersRigidbody2D.Count > 0)
        {
            direction *= -1;
            currentPointPosition = targetPosition;
            speed = elevatorSpeed;
        }
    }

    #endregion Core

    #region Events

    protected override void OnCollision2DEnter(Collider2D other)
    {
        if(other.gameObject.layer == 7){
            base.OnCollision2DEnter(other);
            CheckDirection();
        }

    }

    protected override void OnCollision2DExit(Collider2D other)
    {
        if(other.gameObject.layer == 7){
            base.OnCollision2DExit(other);
            CheckDirection();
        }
    }

    #endregion Events

    #region State machine

    #region Moving State

    private void Enter_Moving()
    {

    }

    private void FixedUpdate_Moving()
    {
        if (CheckArrivedDestination())
        {
            stateMachine.CurrentState = STATE_WAITING;
            return;
        }

        myRigidbody2D.linearVelocity = direction * speed * Time.fixedDeltaTime;
    }

    private void Exit_Moving()
    {
        lastDirection = direction;
    }

    private bool CheckArrivedDestination()
    {
        return Vector2.Dot(direction, currentPointPosition - (Vector2)Transform.position) <= 0.0f;
    }

    #endregion Moving State

    #region Waiting State

    private void Enter_Waiting()
    {
        myRigidbody2D.linearVelocity = Vector2.zero;
    }

    private void Update_Waiting()
    {
        if(lastDirection != direction)
        {
            stateMachine.CurrentState = STATE_MOVING;
            return;
        }    

        myRigidbody2D.linearVelocity = Vector2.zero;
    }

    private void Exit_Waiting()
    {

    }

    #endregion Waiting State

    #endregion State machine
}
