using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "EnvironmentStats", menuName = "Scriptable Objects/EnvironmentStats")]
public class SO_EnvironmentStats : ScriptableObject
{
    [Header("General")]
    [Tooltip("Is the player able to crawl in the surface of this environment?")]
    public bool CanSwimCrawl = false;

    [Tooltip("Is the player able to breath in this environment?")]
    public bool CanBreath = false;

    [Tooltip("The slowdown velocity when getting in the environment")]
    [Range(0, 1)]
    public float EnvironmentEntryVelocityMultiplier = 0.1f;

    [Header("Surface")]
    [Tooltip("Indicates if the environment has a surface and whether 'surface' features should be enabled")]
    public bool HasSurface = true;

    [Tooltip("The surface height for the player to be considered at the environment surface")]
    public float SurfaceThreshold = 0.2f;

    [Tooltip("The jump multiplier when jumping on the surface  level to help the player leave the ")]
    public float EnvironmentSurfaceJumpMultiplier = 5.0f;

    [Header("Buoyancy")]
    [Tooltip("The upwards force applied to the player when in environment")]
    public float BuoyancyForce = 10;

    [Tooltip("The upwards buoyancy max speed when in environment")]
    public float BuoyancyMaxSpeed = 10;
}