using UnityEngine;

[CreateAssetMenu(fileName = "CharacterMovement", menuName = "Scriptable Objects/CharacterMovement")]
public class SO_CharacterMovement : ScriptableObject
{
    [Header("Movement")]
    [Tooltip("The top horizontal movement speed")]
    public float MaxSpeed = 14;

    [Tooltip("The player's capacity to gain horizontal speed")]
    public float Acceleration = 120;

    [Tooltip("The pace at which the player comes to a stop")]
    public float GroundDeceleration = 60;

    [Tooltip("Deceleration in air only after stopping input mid-air")]
    public float AirDeceleration = 30;

    [Tooltip("A constant downward force applied while grounded. Helps on slopes"), Range(0f, -10f)]
    public float GroundingForce = -1.5f;

    [Tooltip("The detection distance for grounding and roof detection"), Range(0f, 0.5f)]
    public float GrounderDistance = 0.05f;

    [Tooltip("Reset the player velocity when change direction to avoid slinding?")]
    public bool SnapControlOnChangeDirection = true;

    [Header("Physics")]
    [Tooltip("The maximum vertical movement speed")]
    public float MaxFallSpeed = 40;

    [Tooltip("Gravity when moving up")]
    public float GoingUpFallAcceleration = 30;

    [Tooltip("Gravity when falling")]
    public float GoingDownFallAcceleration = 20;

    [Tooltip("Impulse Increase Deceleration")]
    public float ImpulseDeceleration = 10.0f;

    [Tooltip("The speed in wich the Deceleration value increases over time")]
    public float ImpulseIncreaseDecelerationSpeed = 10.0f;

    [Header("Jump")]
    [Tooltip("The immediate velocity applied when jumping")]
    public float JumpPower = 36;

    [Tooltip("The amount of times the player can execute a regular jump")]
    public int JumpAmount = 1;

    [Tooltip("The amount of time that the player is going to be at the jump apex. a.k.a The highest point of the jump")]
    [Range(0.0f, 0.5f)]
    public float JumpApexTime = 0.0f;

    [Tooltip("The gravity multiplier added when jump is released early")]
    public float JumpEndEarlyGravityModifier = 3;

    [Tooltip("The time before coyote jump becomes unusable. Coyote jump allows jump to execute even after leaving a ledge")]
    public float CoyoteTime = 0.15f;

    [Tooltip("The amount of time we buffer a jump. This allows jump input before actually hitting the ground")]
    public float JumpBuffer = 0.2f;

    [Header("Slope")]
    [Tooltip("The detection distance for slope detection")]
    public float SlopeCheckDistance = 0.2f;

    [Tooltip("The max slope angle that the player is able to walk on")]
    public float MaxSlopeAngle = 50.0f;

    [Tooltip("The max slide speed when the player is on a non walkable slope")]
    public float NotWalkableSlope_MaxSlideSpeed = 20;
    
    [Tooltip("The slide acceleration when the player is on a non walkable slope")]
    public float NotWalkableSlope_SlideAcceleration = 110;
}
