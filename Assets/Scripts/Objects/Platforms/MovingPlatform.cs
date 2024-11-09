using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MovingPlatform : Platform
{
    #region Variables

    /* Private */
    [SerializeField] private Transform[] pointTransforms;
    [SerializeField] private float waitTime = 2f;
    [SerializeField] private float speed = 0.6f;

    private float timer = 0f;

    private int currentPoint = 0;
    private int previousPoint = -1;
    private Vector2 direction;
    private Vector2[] pointsPositions;

    private Rigidbody2D myRigidbody2D;

    #endregion // Variables

    #region Constants & Read Only

    private const int STATE_MOVING = 0;
    private const int STATE_WAITING = 1;
    private Vector2 currentPointPosition => pointsPositions[currentPoint];
    private Vector2 previousPointPosition => pointsPositions[previousPoint];
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
        stateMachine.CurrentState = STATE_MOVING;
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

    private void OnDrawGizmosSelected()
    {
        Gizmos.color = Color.gray;
        if (pointsPositions == null)
            return;

        for (int i = 0; i < pointsPositions.Length; i++)
        {
            Gizmos.DrawWireSphere(pointsPositions[i], 0.5f);
        }
    }

    #endregion // Engine

    #region Setup

    private void SetupVariables()
    {
        myRigidbody2D = GetComponent<Rigidbody2D>();
        stateMachine.Setup(this);

        // Cache the points positions
        pointsPositions = new Vector2[pointTransforms.Length];
        for (int i = 0; i < pointsPositions.Length; i++)
        {
            pointsPositions[i] = pointTransforms[i].position;
            pointTransforms[i].gameObject.Disable();
        }
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

    #endregion // Setup

    #region Core

    private void FixedUpdateOthersRigidbodies()
    {
        if (characterMovement != null)
        {
            characterMovement.AddBaseForce(myRigidbody2D.linearVelocity);
        }

        for (int i = 0; i < othersRigidbody2D.Count; i++)
        {
            othersRigidbody2D[i].AddForce(myRigidbody2D.linearVelocity);
        }
    }

    private void ChangeCurrentPoint()
    {
        // Add to the points
        currentPoint++;
        previousPoint++;

        // Clamp the points to the array lenght
        currentPoint %= pointsPositions.Length;
        previousPoint %= pointsPositions.Length;

        direction = (currentPointPosition - previousPointPosition).normalized;
    }

    #endregion // Core

    #region State machine

    #region Moving State

    private void Enter_Moving()
    {
        ChangeCurrentPoint();
    }

    private void FixedUpdate_Moving()
    {
        if (CheckArrivedDestination())
        {
            if (waitTime <= 0.0f)
            {
                Enter_Moving();
            }
            else
            {
                stateMachine.CurrentState = STATE_WAITING;
            }

            return;
        }

        myRigidbody2D.linearVelocity = direction * speed * Time.fixedDeltaTime;
    }

    private void Exit_Moving()
    {

    }

    private bool CheckArrivedDestination()
    {
        return Vector2.Dot(direction, currentPointPosition - (Vector2)Transform.position) <= 0.0f;
    }

    #endregion // Moving State

    #region Waiting State

    private void Enter_Waiting()
    {
        timer = Time.time + waitTime;
        myRigidbody2D.linearVelocity = Vector2.zero;
    }

    private void Update_Waiting()
    {
        if (Time.time >= timer)
        {
            stateMachine.CurrentState = STATE_MOVING;
            return;
        }

        myRigidbody2D.linearVelocity = Vector2.zero;
    }

    private void Exit_Waiting()
    {

    }

    #endregion // Waiting State

    #endregion // State machine
}
