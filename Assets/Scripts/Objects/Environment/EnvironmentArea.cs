using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using DG.Tweening;

[RequireComponent(typeof(Collider))]
public class EnvironmentArea : MonoBehaviour
{
    #region Variables

    #region Getters

    public Vector2 UpperSurface { get; private set; }
    public Vector2 LowerSurface { get; private set; }
    public float SurfaceThresholdYPosition { get; private set; }
    public SO_EnvironmentStats Stats => environmentStats;
    public SO_CharacterMovement EnvironmentMovementStats => environmentMovementStats;

    #endregion // Getters

    [SerializeField] private SO_CharacterMovement environmentMovementStats;
    [SerializeField] private SO_EnvironmentStats environmentStats;

    private Collider myCollider;
    private Collider2D myCollider2D;

    #endregion // Variables

    #region Engine

    void Awake()
    {
        myCollider = GetComponent<Collider>();
        myCollider2D = GetComponent<Collider2D>();

        myCollider.OnCollisionEnter.AddListener(OnCollision2DEnter);
        myCollider.OnCollisionExit.AddListener(OnCollision2DExit);
    }

    void Start()
    {
        UpdateSurfaces();
    }

#if UNITY_EDITOR
    private void Update()
    {
        UpdateSurfaces();
    }
#endif

    #endregion // Engine

    #region Events

    private void OnCollision2DEnter(Collider2D collision)
    {
        EnvironmentObject environmentObject = collision.GetComponent<EnvironmentObject>();

        if (environmentObject != null)
            environmentObject.OnEnvironmentEnter(this);
           
    }

    private void OnCollision2DExit(Collider2D collision)
    {
        EnvironmentObject environmentObject = collision.GetComponent<EnvironmentObject>();

        if (environmentObject != null)
            environmentObject.OnEnvironmentExit(this);
    }

    #endregion // Events

    private void UpdateSurfaces()
    {
        float surfaceX = transform.position.x + myCollider2D.bounds.extents.x;
        float surfaceY = transform.position.y + myCollider2D.bounds.extents.y;
        UpperSurface = new Vector2(surfaceX, surfaceY);

        surfaceX = transform.position.x - myCollider2D.bounds.extents.x;
        surfaceY = transform.position.y - myCollider2D.bounds.extents.y;
        LowerSurface = new Vector2(surfaceX, surfaceY);

        SurfaceThresholdYPosition = UpperSurface.y - Stats.SurfaceThreshold;
    }
}