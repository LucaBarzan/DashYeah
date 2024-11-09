using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CharacterMovement : Object
{
    #region Variables

    // Public
    // Get
    public bool IsGrounded { get; private set; }
    public bool IsJumping { get; private set; }
    public float HorizontalDirection { get; protected set; }
    public Vector2 Velocity => velocity;

    // Get & Set
    [HideInInspector] public Vector2 MovementDirection;

    // Private
    [Header("Basics")]
    [SerializeField] private SO_CharacterMovement data;
    [SerializeField] private CapsuleCollider capsuleCollider;
    [SerializeField] private new Rigidbody rigidbody;

    // Base Movement
    private RaycastHit2D[] groundHits;
    private RaycastHit2D groundHit;

    protected Vector3 velocity;
    protected Vector3 frameOverrideVelocity;
    protected Vector3 frameAdditionalVelocity;
    protected Vector3 baseForce;
    private float horizontalSpeed = 0.0f;
    private float frameLeftGrounded = float.MinValue;
    private float lastHorizontalDirection;

    // Jump
    private int jumpSubState;
    private int jumpsAvailable;

    private float timeJumpWasPressed;
    private float jumpApexTime;

    private bool bufferedJumpUsable = false;
    private bool endedJumpEarly = false;
    private bool coyoteUsable = false;

    // Slope
    private Vector2 groundDirection = Vector2.zero;

    private bool walkableGround = false;

    private float slopeAngle = 0.0f;
    private float groundFlowSign = 1.0f;

    // Impulse
    protected Vector3 impulseForce;
    protected Vector3 initialImpulseForce;
    protected float impulseDecelerationSpeed;
    protected float impulseHorizontalMultiplier = 1.0f;

    #endregion Variables

    #region Constants

    // Private const
    private readonly StateMachine stateMachine = new StateMachine();
    private const float GROUNDED_GRAVITY = -2.0f;
    private const int STATE_GROUNDED = 0;
    private const int STATE_AIRBORNE = 1;

    #endregion Constants

    #region Setup

    private void SetupVariables()
    {
        rigidbody = GetComponent<Rigidbody>();

        // gravity = Physics.gravity.y * gravityScale;
    }

    private void SetupStateMachine()
    {
        stateMachine.AddEnterState(STATE_GROUNDED, Grounded_Enter);
        stateMachine.AddUpdateState(STATE_GROUNDED, Grounded_Update);
        stateMachine.AddExitState(STATE_GROUNDED, Grounded_Exit);

        stateMachine.AddEnterState(STATE_AIRBORNE, Airborne_Enter);
        stateMachine.AddUpdateState(STATE_AIRBORNE, Airborne_Update);
        stateMachine.AddExitState(STATE_AIRBORNE, Airborne_Exit);
    }

    #endregion Setup

    #region Engine

    protected override void Awake()
    {
        base.Awake();
        SetupVariables();
        SetupStateMachine();
    }

    private void Update()
    {
#if UNITY_EDITOR
        // Update the inspector values every frame to make it easier for the Designers to tweek the values in real time
        SetupVariables();
#endif
        stateMachine.UpdateState();
    }

    #endregion Engine

    #region Core

    private void CheckCollisions()
    {
        /*
        Vector3 point1 = Transform.position + capsuleCollider.center + (Transform.up * -capsuleCollider.height * 0.5f);
        Vector3 point2 = point1 + Transform.up * capsuleCollider.height;

        // Ground and Ceiling
        groundHits = Physics.CapsuleCastAll(point1,
            point2,
            capsuleCollider.radius,
            capsuleCollider.direction,
            data.GrounderDistance,
            Utils.GlobalSettings.WalkableLayers);

        CeilingHit = Physics.CapsuleCast(capsuleCollider.bounds.center,
            capsuleCollider.size,
            capsuleCollider.direction,
            0,
            Vector2.up,
            movementStats.GrounderDistance,
            Utils.GlobalSettings.ObstacleLayers);

        wallHits = Physics.CapsuleCastAll(capsuleCollider.bounds.center,
            capsuleCollider.size,
            capsuleCollider.direction,
            0,
            Vector2.right * HorizontalDirection,
            movementStats.WallCling_CheckDistance,
            Utils.GlobalSettings.ObstacleLayers);
        */
    }

    private void GatherCollisionsValues()
    {
        /*
        groundHit = new RaycastHit2D();

        for (int i = 0; i < groundHits.Length; i++)
        {
            if (!Physics2D.GetIgnoreCollision(capsuleCollider, groundHits[i].collider))
            {
                groundHit = groundHits[i];
                break;
            }
        }

        wallHit = new RaycastHit2D();
        for (int i = 0; i < wallHits.Length; i++)
        {
            if (!Physics2D.GetIgnoreCollision(capsuleCollider, wallHits[i].collider))
            {
                wallHit = wallHits[i];
                break;
            }
        }

        // Hit a Ceiling
        if (CeilingHit)
        {
            velocity.y = Mathf.Min(0, velocity.y);
        }

        // Cancel the impulse force it is not at the start of the impulse and if hit something 
        if (initialImpulseForce != impulseForce && (groundHit || wallHit || CeilingHit))
        {
            ResetImpulseValues();
        }
        */
    }

    private void GatherGroundValues()
    {
        /*
        // Landed on the Ground
        if (!isGrounded && groundHit)
        {
            SetPlayingGoatAnimation(false);
            isGrounded = true;
        }
        // Left the Ground
        else if (isGrounded && !groundHit)
        {
            isGrounded = false;
            frameLeftGrounded = Time.time;
        }

        if (!isGrounded)
            return;

        slopeAngle = Vector2.Angle(groundHit.normal, Vector2.up);
        walkableGround = slopeAngle <= movementStats.MaxSlopeAngle;

        groundDirection = -Vector2.Perpendicular(groundHit.normal).normalized;
        groundFlowSign = Mathf.Sign(Vector2.Dot(groundDirection, Vector2.down));
        */
    }

    private void CheckStates()
    {
        /*
        if (!isGrounded &&
            wallHit &&
            velocity.y <= 0.0f &&
            stateMachine.CurrentState != STATE_WALL_JUMP &&
            GameManager.Instance.CurrentSave.AbilityUnlocked(EPlayerAbilities.WallJump))
        {
            stateMachine.CurrentState = STATE_WALL_CLING;
            return;
        }

        if (isSubmerged &&
            environment.Stats.CanSwimCrawl &&
            stateMachine.CurrentState != STATE_JUMP &&
            player.Transform.position.y >= SwimYThresholdPosition &&
            velocity.y >= 0.0f)
        {
            stateMachine.CurrentState = STATE_SWIM_CRAWL;
            return;
        }

        if (HasBufferedJump && CanJump())
            ExecuteJump();

        if (HasBufferedFlappyJump && FlappyJumpFinished() && CanFlappyJump())
            ExecuteFlappyJump();
        */
    }

    private void HandleImpulseForce()
    {
        /*
        void IncreaseDecelerationSpeed() => impulseDecelerationSpeed += Stats.ImpulseIncreaseDecelerationSpeed * Time.fixedDeltaTime;

        if (impulseForce == Vector2.zero)
            return;

        // Always increase the Deceleration speed
        IncreaseDecelerationSpeed();

        // Increase the deceleration speed even more if the input direction is performed in the opposite direction to the impulse direction
        if (Vector2.Dot(player.InputDirection, impulseForce.normalized) < 0.0f)
            IncreaseDecelerationSpeed();

        impulseForce = Vector2.MoveTowards(impulseForce, Vector2.zero, Stats.ImpulseDeceleration * impulseDecelerationSpeed * Time.fixedDeltaTime);

        if (impulseForce == Vector2.zero)
            ResetImpulseValues();
        */
    }

    private void ApplyMovement()
    {
        /*
        if (frameOverrideVelocity != Vector2.zero)
            velocity = frameOverrideVelocity;

        // Main movement
        if (!usingHorizontalDeltaMovement)
        {
            velocity += frameAdditionalVelocity;
            HandleImpulseForce();

            Vector2 resultVelocity = velocity;
            resultVelocity *= frameResistance;
            resultVelocity += baseForce;
            resultVelocity += impulseForce;

            rigidbody2D.velocity = resultVelocity;
        }
        // Moving using delta vector (a.k.a pull platform movement)
        else
        {
            Vector2 newPosition = velocity.normalized * horizontalDeltaMovement + rigidbody2D.position;
            rigidbody2D.MovePosition(newPosition);
        }

        lastHorizontalDirection = HorizontalDirection;
        frameAdditionalVelocity = baseForce = frameOverrideVelocity = Vector2.zero;
        frameResistance = 1.0f;
        */
    }

    #endregion // Core

    #region State Machine

    #region Grounded State

    void Grounded_Enter()
    {
        ResetJumpValues();
    }

    void Grounded_Update()
    {
        if (!IsGrounded)
        {
            stateMachine.CurrentState = STATE_AIRBORNE;
            return;
        }

        HandleDirection_Grounded();
        HandleGravity_Grounded();
    }

    void Grounded_Exit()
    {

    }

    void HandleDirection_Grounded()
    {
        /*
        if (!walkableGround)
        {
            // Add reverse velocity to the player
            horizontalSpeed = Mathf.MoveTowards(horizontalSpeed,
                data.NotWalkableSlope_MaxSlideSpeed * groundFlowSign,
                data.NotWalkableSlope_SlideAcceleration * Time.fixedDeltaTime);
        }
        else if (MovementDirection == Vector2.zero)
        {
            var deceleration = data.GroundDeceleration;
            horizontalSpeed = Mathf.MoveTowards(horizontalSpeed, 0, deceleration * Time.fixedDeltaTime);
        }
        // Is grounded or on the air
        else
        {
            float horizontalTargetSpeed = data.MaxSpeed * MovementDirection;

            if (Stats.SnapControlOnChangeDirection && HorizontalDirection != lastHorizontalDirection)
                horizontalSpeed *= -1;

            horizontalSpeed = Mathf.MoveTowards(horizontalSpeed, horizontalTargetSpeed, data.Acceleration * Time.fixedDeltaTime);
        }

        // We must multiply the player momentum by the ground sign direction
        velocity = horizontalSpeed * groundDirection.x;

        /////===============/////

        // Add speed to the movement direction
        MovementDirection *= movementSpeed;

        // Apply the new movement direction to the velocity
        velocity = new Vector3(MovementDirection.x, velocity.y, MovementDirection.y);

        // Apply a default little gravity when grounded
        velocity.y = GROUNDED_GRAVITY;

        // Apply the velocity
        characterController.Move(velocity * Time.deltaTime);
        MovementDirection = Vector2.zero;
        */
    }

    void HandleGravity_Grounded()
    {
        /*
        // Apply the ground Y direction to the Y velocity
        velocity.y = groundDirection.y * horizontalSpeed;

        // Apply the grounded force to the ground perpendicular direction
        velocity += groundHit.normal * data.GroundingForce;
        */
    }



    #endregion Grounded State

    #region Jump

    public void Jump()
    {
        /*
        // Only can jump if the player is on the ground
        if (!characterController.isGrounded)
            return;

        // Add gravity to the velocity
        velocity.y = initialJumpVelocity;
        */
    }

    private void ResetJumpValues()
    {
        /*
        coyoteUsable = true;
        bufferedJumpUsable = false;
        endedJumpEarly = false;
        jumpsAvailable = jumpAmount;
        */
    }


    #endregion Jump

    #region Airbone State

    void Airborne_Enter()
    {

    }

    void Airborne_Update()
    {
        /*
        HandleMovement();

        // Add gravity to the velocity
        velocity.y += gravity * Time.deltaTime;
        */
    }

    void Airborne_Exit()
    {

    }

    #endregion Airbone State

    #endregion State Machine

    #region Public Methods

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
    public virtual void ApplyImpulseForce(Vector2 force)
    {
        impulseDecelerationSpeed = 0.0f;
        impulseHorizontalMultiplier = Mathf.Abs(Mathf.Abs(force.normalized.x) - 1.0f);
        initialImpulseForce = impulseForce = force;
    }

    /// <summary>
    /// Sobrescreve a velocidade do personagem
    /// </summary>
    public virtual void OverrideForce(Vector2 forceDirection)
    {
        velocity = frameOverrideVelocity = forceDirection;

        if (!isActiveAndEnabled)
            rigidbody.linearVelocity = forceDirection;
    }


    #endregion Public Methods
}
