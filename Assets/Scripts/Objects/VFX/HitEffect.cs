using UnityEngine;

public class HitEffect : PoolObject
{
    private enum RotationType
    {
        None,
        Random,
        Direction
    }

    #region Variables

    [HideInInspector] public Vector2 Direction;

    [SerializeField] private SpriteRenderer spriteRenderer;
    [SerializeField] private RotationType rotationType;

    private Transform myTransform;

    #endregion Variables

    #region Constants & ReadOnly

    private const float ANGLE_MIN = 0.0f;
    private const float ANGLE_MAX = 360.0f;

    #endregion // Constants & ReadOnly

    #region Engine

    private void Awake() => myTransform = transform;

    protected override void OnDisable()
    {
        spriteRenderer.gameObject.Disable();
        base.OnDisable();
    }

    #endregion // Engine

    public void Show()
    {
        spriteRenderer.gameObject.Enable();
        // Assume that the effect direction is always right by default
        Vector2 rotation = Vector2.right;

        switch (rotationType)
        {
            case RotationType.Random:
                float angle = Random.Range(ANGLE_MIN, ANGLE_MAX);
                rotation = Utils.RotateVector2D(rotation, angle);
                break;

            case RotationType.Direction:
                rotation = Direction;
                // Assume that the effect direction is always right by default
                rotation.RotateVector2D(90.0f);
                break;
        }

        myTransform.rotation = Quaternion.LookRotation(Vector3.forward, rotation);
    }
}
