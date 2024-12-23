using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CharacterMovement : Object
{
    #region Variables

    /* Public */

    public bool IsJumping => stateMachine.CurrentState == STATE_JUMP || stateMachine.CurrentState == STATE_WALL_JUMP;

    /* Private */

    // Components
    [SerializeField] private SO_CharacterMovement defaultMovementStats;
    [SerializeField] private CapsuleCollider capsuleCollider;
    [SerializeField] private EnvironmentObject environmentObject;
    [SerializeField] private new Rigidbody rigidbody;

    private SO_CharacterMovement movementStats;

    // Base Movement
    private bool canMove = false;
    private Vector3 movementDirection;
    private Vector3 lastMovementDirection;

    private RaycastHit[] groundHits;
    private RaycastHit groundHit;
    private bool ceilingHit;

    private bool isGrounded = false;
    private float horizontalSpeed = 0.0f;
    private float frameLeftGrounded = float.MinValue;
    private float frameResistance = 1.0f;
    private float horizontalDeltaMovement;

    private Vector3 velocity;
    private Vector3 frameOverrideVelocity;
    private Vector3 frameAdditionalVelocity;
    private Vector3 baseForce;

    // Impulse
    private Vector3 impulseForce;
    private Vector3 initialImpulseForce;
    private float impulseDecelerationSpeed;
    private float impulseHorizontalMultiplier = 1.0f;

    // Jump
    private int jumpSubState;
    private int jumpsAvailable;

    private float timeJumpWasPressed;
    private float jumpApexTime;

    private bool bufferedJumpUsable = false;
    private bool endedJumpEarly = false;
    private bool coyoteUsable = false;

    // Slope
    private Vector3 groundDirection = Vector3.zero;
    private bool walkableGround = false;
    private float slopeAngle = 0.0f;
    private float groundFlowSign = 1.0f;

    // Wall Cling State
    private RaycastHit wallHit;
    private RaycastHit[] wallHits;
    private float wallClingDirectionSign;
    private float wallClingSlideTime;
    private float wallClingExitTime;
    private float wallPositionX;

    // Wall Jump State
    private float wallJumpHorizontalControl;
    private Vector3 wallJumpDirection;

    // Swimming State
    private float SwimYThresholdPosition;

    #endregion // Variables

    #region Constants

    // State machine
    private readonly StateMachine stateMachine = new StateMachine();
    private const int STATE_GROUNDED = 0;
    private const int STATE_JUMP = 1;
    private const int STATE_AIRBORNE = 2;
    private const int STATE_SWIM = 3;
    private const int STATE_WALL_CLING = 4;
    private const int STATE_WALL_JUMP = 5;

    // Environment
    private EnvironmentArea environment => environmentObject.Environment;
    private bool isSubmerged => environment != null && Transform.position.y <= environment.UpperSurface.y;
    private bool isOnEnvironmentSurfaceLevel => environment != null && Transform.position.y >= environment.SurfaceThresholdYPosition;

    // Jump State
    private bool HasBufferedJump => bufferedJumpUsable && Time.time < timeJumpWasPressed + movementStats.JumpBuffer;
    private bool CanUseCoyote => coyoteUsable && !isGrounded && Time.time < frameLeftGrounded + movementStats.CoyoteTime;

    #endregion // Constants

    #region Engine

    protected override void Awake()
    {
        base.Awake();
        SetupStateMachine();
    }

    private void FixedUpdate()
    {
        if (!canMove)
            return;

        stateMachine.FixedUpdateState();
        ApplyMovement();
    }

    #endregion // Engine

    private void SetupStateMachine()
    {
        stateMachine.Setup(this);

        stateMachine.AddEnterState(STATE_GROUNDED, Enter_Grounded);
        stateMachine.AddFixedUpdateState(STATE_GROUNDED, FixedUpdate_Grounded);
        stateMachine.AddExitState(STATE_GROUNDED, Exit_Grounded);

        stateMachine.AddEnterState(STATE_JUMP, Enter_Jump);
        stateMachine.AddFixedUpdateState(STATE_JUMP, FixedUpdate_Jump);
        stateMachine.AddExitState(STATE_JUMP, Exit_Jump);

        stateMachine.AddEnterState(STATE_AIRBORNE, Enter_Airborne);
        stateMachine.AddFixedUpdateState(STATE_AIRBORNE, FixedUpdate_Airborne);
        stateMachine.AddExitState(STATE_AIRBORNE, Exit_Airborne);

        stateMachine.AddEnterState(STATE_SWIM, Enter_Swim_Crawl);
        stateMachine.AddFixedUpdateState(STATE_SWIM, FixedUpdate_Swim_Crawl);
        stateMachine.AddExitState(STATE_SWIM, Exit_Swim_Crawl);

        stateMachine.AddEnterState(STATE_WALL_CLING, Enter_WallCling);
        stateMachine.AddFixedUpdateState(STATE_WALL_CLING, FixedUpdate_WallCling);
        stateMachine.AddExitState(STATE_WALL_CLING, Exit_WallCling);

        stateMachine.AddEnterState(STATE_WALL_JUMP, Enter_WallJump);
        stateMachine.AddFixedUpdateState(STATE_WALL_JUMP, FixedUpdate_WallJump);
        stateMachine.AddExitState(STATE_WALL_JUMP, Exit_WallJump);
    }

    #region Core

    private void CheckCollisions()
    {
        Vector3 p1 = Transform.position + capsuleCollider.center + Vector3.up * -capsuleCollider.height * 0.5f;
        Vector3 p2 = p1 + Vector3.up * capsuleCollider.height;

        // Ground and Ceiling
        groundHits = Physics.CapsuleCastAll(p1, p2, capsuleCollider.radius, Vector3.down, movementStats.CheckDistance_Vertical, Utils.GlobalSettings.WalkableLayers);

        ceilingHit = Physics.CapsuleCast(p1, p2, capsuleCollider.radius, Vector3.up, movementStats.CheckDistance_Vertical, Utils.GlobalSettings.ObstacleLayers);

        wallHits = Physics.CapsuleCastAll(p1, p2, capsuleCollider.radius, movementDirection, movementStats.CheckDistance_Horizontal, Utils.GlobalSettings.ObstacleLayers);
    }

    private void GatherCollisionsValues()
    {
        groundHit = new RaycastHit();

        for (int i = 0; i < groundHits.Length; i++)
        {
            if (!Physics.GetIgnoreCollision(capsuleCollider, groundHits[i].collider))
            {
                groundHit = groundHits[i];
                break;
            }
        }

        wallHit = new RaycastHit();
        for (int i = 0; i < wallHits.Length; i++)
        {
            if (!Physics.GetIgnoreCollision(capsuleCollider, wallHits[i].collider))
            {
                wallHit = wallHits[i];
                break;
            }
        }

        // Hit a Ceiling
        if (ceilingHit)
        {
            velocity.y = Mathf.Min(0, velocity.y);
        }

        bool hitSomething = !groundHit.IsNull() || !wallHit.IsNull() || ceilingHit;
        // Cancel the impulse force if it is not at the start of the impulse and if hit something 
        if (initialImpulseForce != impulseForce && hitSomething)
        {
            ResetImpulseValues();
        }
    }

    private void GatherGroundValues()
    {
        // Landed on the Ground
        if (!isGrounded && !groundHit.IsNull())
        {
            isGrounded = true;
        }
        // Left the Ground
        else if (isGrounded && groundHit.IsNull())
        {
            isGrounded = false;
            frameLeftGrounded = Time.time;
        }

        if (!isGrounded)
            return;

        slopeAngle = Vector3.Angle(groundHit.normal, Vector3.up);
        walkableGround = slopeAngle <= movementStats.MaxSlopeAngle;

        groundDirection = -Vector3.Perpendicular(groundHit.normal).normalized;
        groundFlowSign = Mathf.Sign(Vector3.Dot(groundDirection, Vector3.down));
    }

    private void CheckStates()
    {
        if (!isGrounded &&
            !wallHit.IsNull() &&
            velocity.y <= 0.0f &&
            stateMachine.CurrentState != STATE_WALL_JUMP)
        {
            stateMachine.CurrentState = STATE_WALL_CLING;
            return;
        }

        if (isSubmerged &&
            environment.Stats.CanSwimCrawl &&
            stateMachine.CurrentState != STATE_JUMP &&
            Transform.position.y >= SwimYThresholdPosition &&
            velocity.y >= 0.0f)
        {
            stateMachine.CurrentState = STATE_SWIM;
            return;
        }

        if (HasBufferedJump && CanJump())
            ExecuteJump();
    }

    private void HandleImpulseForce()
    {
        void IncreaseDecelerationSpeed() => impulseDecelerationSpeed += movementStats.ImpulseIncreaseDecelerationSpeed * Time.fixedDeltaTime;

        if (impulseForce == Vector3.zero)
            return;

        // Always increase the Deceleration speed
        IncreaseDecelerationSpeed();

        // Increase the deceleration speed even more if the input direction is performed in the opposite direction to the impulse direction
        if (Vector3.Dot(movementDirection, impulseForce.normalized) < 0.0f)
            IncreaseDecelerationSpeed();

        impulseForce = Vector3.MoveTowards(impulseForce, Vector3.zero, movementStats.ImpulseDeceleration * impulseDecelerationSpeed * Time.fixedDeltaTime);

        if (impulseForce == Vector3.zero)
            ResetImpulseValues();
    }

    private void ApplyMovement()
    {
        if (frameOverrideVelocity != Vector3.zero)
            velocity = frameOverrideVelocity;

        velocity += frameAdditionalVelocity;
        HandleImpulseForce();

        Vector3 resultVelocity = velocity;
        resultVelocity *= frameResistance;
        resultVelocity += baseForce;
        resultVelocity += impulseForce;

        rigidbody.linearVelocity = resultVelocity;

        lastMovementDirection = movementDirection;
        frameAdditionalVelocity = baseForce = frameOverrideVelocity = Vector3.zero;
        frameResistance = 1.0f;
    }

    private void ResetImpulseValues()
    {
        impulseForce = initialImpulseForce = Vector3.zero;
        impulseDecelerationSpeed = 0.0f;
        impulseHorizontalMultiplier = 1.0f;
    }

    private void ResetGravity()
    {
        velocity.y = 0.0f;
        OverrideForce(velocity);
    }

    #endregion // Core

    #region Movement States

    #region Grounded State

    private void Enter_Grounded()
    {
        // RESET DASH
        ResetJumpValues();
    }

    private void FixedUpdate_Grounded()
    {
        if (!isGrounded)
        {
            stateMachine.CurrentState = STATE_AIRBORNE;
            return;
        }

        HandleDirection_Grounded();
        HandleGravity_Grounded();
    }

    private void Exit_Grounded()
    {
        if (stateMachine.CurrentState != STATE_JUMP)
            jumpsAvailable--;
    }

    private void HandleDirection_Grounded()
    {
        float horizontalInput = movementDirection.x;

        if (!walkableGround)
        {
            // Add reverse velocity to the player
            horizontalSpeed = Mathf.MoveTowards(horizontalSpeed,
                movementStats.NotWalkableSlope_MaxSlideSpeed * groundFlowSign,
                movementStats.NotWalkableSlope_SlideAcceleration * Time.fixedDeltaTime);
        }
        else if (horizontalInput == 0)
        {
            var deceleration = movementStats.GroundDeceleration;
            horizontalSpeed = Mathf.MoveTowards(horizontalSpeed, 0, deceleration * Time.fixedDeltaTime);
        }
        // Is grounded or on the air
        else
        {
            float horizontalTargetSpeed = movementStats.MaxSpeed * horizontalInput;

            if (movementStats.SnapControlOnChangeDirection && movementDirection != lastMovementDirection)
                horizontalSpeed *= -1;

            horizontalSpeed = Mathf.MoveTowards(horizontalSpeed, horizontalTargetSpeed, movementStats.Acceleration * Time.fixedDeltaTime);
        }

        // We must multiply the player momentum by the ground sign direction
        velocity.x = horizontalSpeed * groundDirection.x;
    }

    private void HandleGravity_Grounded()
    {
        // If it is grounded but there is upwards force in the , apply upwards force anyway
        if (environment != null && environment.Stats.BuoyancyForce > 0 && isSubmerged)
        {
            velocity.y = Mathf.MoveTowards(velocity.y, environment.Stats.BuoyancyForce, environment.Stats.BuoyancyForce * Time.fixedDeltaTime);
            return;
        }

        if (frameAdditionalVelocity != Vector3.zero)
            return;

        // Apply the ground Y direction to the Y velocity
        velocity.y = groundDirection.y * horizontalSpeed;

        // Apply the grounded force to the ground perpendicular direction
        velocity += groundHit.normal * movementStats.GroundingForce;
    }

    #endregion // Grounded State

    #region Jump State

    private void Enter_Jump()
    {
        jumpsAvailable--;

        endedJumpEarly = false;
        bufferedJumpUsable = false;
        coyoteUsable = false;
        timeJumpWasPressed = 0;
        jumpSubState = 0;
    }

    private void FixedUpdate_Jump()
    {
        HandleDirection_Airborne();
        HandleGravity_Jump(movementStats.GoingUpFallAcceleration, movementStats.GoingDownFallAcceleration);
    }

    private void Exit_Jump()
    {

    }

    private void HandleJumpInputPerformed()
    {
        timeJumpWasPressed = Time.time;

        if (stateMachine.CurrentState == STATE_WALL_CLING)
        {
            stateMachine.CurrentState = STATE_WALL_JUMP;
            return;
        }

        // Check if the player cannot jump because there is no more jumps available
        if (!CanJump())
            return;

        ExecuteJump();
    }

    private void HandleJumpInputReleased()
    {
        if (stateMachine.CurrentState == STATE_JUMP ||
            stateMachine.CurrentState == STATE_WALL_JUMP)
        {
            endedJumpEarly = true;
            stateMachine.CurrentState = STATE_AIRBORNE;
        }
    }

    private void HandleGravity_Jump(float goingUpGravity, float goingDownGravity)
    {
        switch (jumpSubState)
        {
            // Going Up
            case 0:
                // If it is still going up, continue to apply gravity
                if (velocity.y > 0.0f)
                {
                    HandleGravity_Airborne(goingUpGravity, goingDownGravity);
                    break;
                }

                // If there is no jump apex time, then go to Airborne state
                if (movementStats.JumpApexTime <= 0.0f)
                {
                    stateMachine.CurrentState = STATE_AIRBORNE;
                    return;
                }

                velocity.y = 0.0f;
                jumpApexTime = Time.time + movementStats.JumpApexTime;
                jumpSubState++;
                break;

            // Jump Apex
            case 1:
                if (Time.time >= jumpApexTime)
                {
                    stateMachine.CurrentState = STATE_AIRBORNE;
                    return;
                }
                break;
        }
    }

    public void ExecuteJump()
    {
        ResetImpulseValues();

        velocity.y = movementStats.JumpPower;

        if (isOnEnvironmentSurfaceLevel)
            velocity.y *= environment.Stats.EnvironmentSurfaceJumpMultiplier;

        stateMachine.CurrentState = STATE_JUMP;
    }

    private void ResetJumpValues()
    {
        coyoteUsable = true;
        bufferedJumpUsable = false;
        endedJumpEarly = false;
        jumpsAvailable = movementStats.JumpAmount;
    }

    private bool CanJump()
    {
        if (stateMachine.CurrentState == STATE_WALL_CLING ||
            stateMachine.CurrentState == STATE_WALL_JUMP)
            return false;

        if (isGrounded && !walkableGround)
            return false;

        // Check if the player still has jumps available to use
        if (jumpsAvailable <= 0 && !CanUseCoyote)
            return false;

        return true;
    }

    #endregion // Jump State

    #region Airborne State

    private void Enter_Airborne()
    {
        bufferedJumpUsable = true;
    }

    private void FixedUpdate_Airborne()
    {
        if (isGrounded)
        {
            stateMachine.CurrentState = STATE_GROUNDED;
            return;
        }

        HandleDirection_Airborne();
        HandleGravity_Airborne(movementStats.GoingUpFallAcceleration, movementStats.GoingDownFallAcceleration);
    }

    private void Exit_Airborne()
    {

    }

    private void HandleDirection_Airborne() => HandleDirection_Airborne(movementStats.Acceleration);

    private void HandleDirection_Airborne(float acceleration)
    {
        float horizontalInput = movementDirection.x;

        if (horizontalInput == 0)
        {
            horizontalSpeed = Mathf.MoveTowards(horizontalSpeed, 0, movementStats.AirDeceleration * impulseHorizontalMultiplier * Time.fixedDeltaTime);
        }
        // Is grounded or on the air
        else
        {
            float horizontalTargetSpeed = movementStats.MaxSpeed * horizontalInput;

            if (movementStats.SnapControlOnChangeDirection && movementDirection != lastMovementDirection)
                horizontalSpeed *= -1;

            horizontalSpeed = Mathf.MoveTowards(horizontalSpeed, horizontalTargetSpeed, acceleration * impulseHorizontalMultiplier * Time.fixedDeltaTime);
        }

        velocity.x = horizontalSpeed;
    }

    private void HandleGravity_Airborne(float goingUpGravity, float goingDownGravity)
    {
        float targetYvelocity = -movementStats.MaxFallSpeed;
        bool goingUp = velocity.y > 0;
        float inAirGravity = goingUp ? goingUpGravity : goingDownGravity;

        if (isSubmerged)
        {
            if (environment.Stats.BuoyancyForce > 0)
            {
                targetYvelocity = environment.Stats.BuoyancyMaxSpeed;
            }

            inAirGravity += environment.Stats.BuoyancyForce;
        }

        if (endedJumpEarly && goingUp)
        {
            inAirGravity *= movementStats.JumpEndEarlyGravityModifier;
        }

        velocity.y = Mathf.MoveTowards(velocity.y, targetYvelocity, inAirGravity * Time.fixedDeltaTime);
    }

    #endregion // Airborne State

    #region Swimming State

    private void Enter_Swim_Crawl()
    {
        velocity.y = 0;

        ResetJumpValues();
    }

    private void FixedUpdate_Swim_Crawl()
    {
        if (environment == null || !environment.Stats.CanSwimCrawl)
        {
            stateMachine.CurrentState = GetDefaultMovementState();
            return;
        }

        HandleDirection_Airborne();
        HandleSwimming();
    }

    private void Exit_Swim_Crawl()
    {

    }

    private void HandleSwimming()
    {
        Vector3 swimPosition = new Vector3(Transform.position.x, environment.UpperSurface.y);
        Transform.MoveTowards(swimPosition, 3.0f * Time.deltaTime);
    }

    #endregion // Swimming State

    #region Wall Cling State

    private void Enter_WallCling()
    {
        wallClingDirectionSign = HorizontalDirection;

        wallClingSlideTime = Time.time + movementStats.WallCling_TimeToSlide;
        wallClingExitTime = 0.0f;
        wallPositionX = wallHit.point.x + capsuleCollider.bounds.extents.x;
    }

    private void FixedUpdate_WallCling()
    {
        if (isGrounded || (wallHit.IsNull() && HorizontalDirection == wallClingDirectionSign))
        {
            stateMachine.CurrentState = GetDefaultMovementState();
            return;
        }

        if (wallClingDirectionSign != HorizontalDirection)
        {
            wallClingExitTime += Time.fixedDeltaTime;

            if (wallClingExitTime > movementStats.WallCling_WallJumpTime)
            {
                stateMachine.CurrentState = GetDefaultMovementState();
                return;
            }
        }

        HandleDirection_WallCling();
        HandleGravity_WallCling();
    }

    private void Exit_WallCling()
    {
        horizontalSpeed = HorizontalDirection * 0.1f;
        velocity.x = HorizontalDirection * 0.1f;
    }

    private void HandleGravity_WallCling()
    {
        if (Time.time >= wallClingSlideTime)
            velocity.y = Mathf.MoveTowards(velocity.y, -movementStats.WallCling_SlideSpeed, movementStats.WallCling_SlideAcceleration * Time.fixedDeltaTime);
    }

    private void HandleDirection_WallCling()
    {
        if (velocity.x != 0.0f)
            return;

        Vector3 newposition = new Vector3(wallPositionX, Transform.position.y);
        Transform.MoveTowards(newposition, movementStats.WallCling_StickToWallSpeed * Time.fixedDeltaTime);
    }

    #endregion // Wall Cling State

    #region Wall Jump State

    private void Enter_WallJump()
    {
        // Convert the angle to Vector3 direction
        wallJumpDirection = Utils.AngleToDirection2D(movementStats.WallJump_JumpAngleDirection);

        if (wallClingDirectionSign > 0)
            wallJumpDirection = Vector3.Reflect(wallJumpDirection, Vector3.left);

        float jumpForce = movementStats.WallJump_JumpForce;

        Vector3 wallJumpResult = wallJumpDirection * jumpForce;

        OverrideForce(wallJumpResult);

        SetHorizontalDirection(wallJumpResult.x);
        horizontalSpeed = wallJumpResult.x;
        wallJumpHorizontalControl = 0.0f;

        // Setup Jump variables
        endedJumpEarly = false;
        bufferedJumpUsable = false;
        coyoteUsable = false;
        timeJumpWasPressed = 0;
        jumpSubState = 0;
    }

    private void FixedUpdate_WallJump()
    {
        HandleDirection_WallJump();
        HandleGravity_Jump(movementStats.WallJump_FallAcceleration, movementStats.GoingDownFallAcceleration);
    }

    private void Exit_WallJump()
    {

    }

    private void HandleDirection_WallJump()
    {
        wallJumpHorizontalControl = Mathf.MoveTowards(wallJumpHorizontalControl,
            movementStats.Acceleration,
            movementStats.WallJump_HorizontalInputDeceleration * Time.fixedDeltaTime);

        HandleDirection_Airborne(wallJumpHorizontalControl);
    }

    #endregion // Wall Jump State

    #endregion // Movement States

    #region Public methods

    public void SetMovementDirection(Vector3 direction) => movementDirection = direction;

    /// <summary> Adiciona uma força à velocidade do personagem </summary>
    public virtual void AddForce(in Vector3 forceDirection) => frameAdditionalVelocity += forceDirection;

    /// <summary>
    /// Essa é a força de referência do objeto (força base), usada para forças constantes
    /// Exemplos de uso são: esteira, Fluxo de ar / água 
    /// </summary>
    public virtual void AddBaseForce(in Vector3 force) => baseForce += force;

    /// <summary>
    /// Sobrescreve a velocidade do personagem proporcionalmente ao valor das forças nos axes
    /// Exemplo: Bounce
    /// </summary>
    public virtual void ApplyImpulseForce(Vector3 force)
    {
        impulseDecelerationSpeed = 0.0f;
        impulseHorizontalMultiplier = Mathf.Abs(Mathf.Abs(force.normalized.x) - 1.0f);
        initialImpulseForce = impulseForce = force;
        ResetGravity();
    }

    /// <summary>
    /// Sobrescreve a velocidade do personagem
    /// </summary>
    public virtual void OverrideForce(Vector3 forceDirection)
    {
        velocity = frameOverrideVelocity = forceDirection;

        if (!isActiveAndEnabled || !canMove)
            rigidbody.linearVelocity = forceDirection;
    }

    public virtual void EnableMovement()
    {
        if (canMove)
            return;

        canMove = true;
        OverrideForce(rigidbody.linearVelocity);
    }

    public virtual void DisableMovement(bool resetVelocity = true, bool canUpdateAnimations = true)
    {
        if (!canMove)
            return;

        canMove = false;

        if (resetVelocity)
        {
            rigidbody.linearVelocity = velocity = Vector3.zero;
        }
    }

    #endregion

    #region Utils

    private int GetDefaultMovementState() => isGrounded ? STATE_GROUNDED : STATE_AIRBORNE;

    #endregion Utils
}
