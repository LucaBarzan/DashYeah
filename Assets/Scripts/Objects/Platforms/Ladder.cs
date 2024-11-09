using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Ladder : MonoBehaviour
{
    #region Variables

    public Vector2 MaxHeightPosition {  get; private set; }
    public Vector2 MinHeightPosition {  get; private set; }
    public Vector2 Position => climbOriginTransform.position;

    [SerializeField] private Interactable interactable;
    [SerializeField] private Transform climbOriginTransform;

    [Tooltip("Vertical area where the player can climb represented by the gray line")]
    [SerializeField] private float height = 10.0f;
    #endregion // Variables

    #region Engine

    private void Awake()
    {
        CalculateHeightPositions();
    }

#if UNITY_EDITOR
    private void Update()
    {
        CalculateHeightPositions();
    }
#endif

    private void OnEnable()
    {
        interactable.OnInteracted_InteractInfo += OnInteracted;
    }

    private void OnDisable()
    {
        interactable.OnInteracted_InteractInfo -= OnInteracted;
    }

    private void OnDrawGizmosSelected()
    {
        Gizmos.color = Color.white;
        float halfHeight = height / 2.0f;

        if(climbOriginTransform  != null)
        {
            Gizmos.DrawRay(climbOriginTransform.position, climbOriginTransform.up * halfHeight);
            Gizmos.DrawRay(climbOriginTransform.position, -climbOriginTransform.up * halfHeight);
        }

    }

    #endregion // Engine

    #region Events

    private void OnInteracted(Interactable interactable, Player player)
    {
        // player.OnInteracted_Ladder(this);
    }

    #endregion //Events

    private void CalculateHeightPositions()
    {
        float halfHeight = height / 2.0f;
        MaxHeightPosition = climbOriginTransform.position + (climbOriginTransform.up * halfHeight);
        MinHeightPosition = climbOriginTransform.position - (climbOriginTransform.up * halfHeight);
    }
}