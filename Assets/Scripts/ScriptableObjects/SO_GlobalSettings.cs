using UnityEngine;
using System.Collections.Generic;

[CreateAssetMenu(fileName = "PlayerSettings", menuName = "Aumakua/Player/PlayerSettings")]
public class SO_GlobalSettings : ScriptableObject
{
    [Header("Layers")]
    [Tooltip("The layer that the player is signed to")]
    public LayerMask PlayerLayer;

    [Tooltip("Set this to the layer your player is on")]
    public LayerMask WalkableLayers;

    [Tooltip("Physical objects that the player collides with")]
    public LayerMask ObstacleLayers;

    [Tooltip("The layer that the enemies are signed to")]
    public LayerMask EnemiesLayers;

    [Tooltip("The layer that the environments are signed to")]
    public LayerMask EnvironmentLayers;

    [Tooltip("The layer that the platforms are signed to")]
    public LayerMask PlatformLayers;

    [Tooltip("The layer that the hitboxes are signed to")]
    public LayerMask HitboxesLayers;

    public LayerMask IgnoreRaycastLayer;

    [Header("Tags")]
    [Tooltip("The tag name of the Hazard tag")]
    public string HazardTag;

    [Header("Scenes")]
    public string[] NonGameplayScenes;
}