using System.Collections.Generic;
using UnityEngine;
using System.Linq;

[RequireComponent(typeof(Collider2D))]
public class OneWayObject : MonoBehaviour
{
    #region Variables

    // Public get & set
    public virtual bool CanWalkOnOneWay { get => canWalkOnOneWay; set => canWalkOnOneWay = value; }

    // Private Inspector
    [SerializeField] private float threshold = 0.05f;
    [SerializeField] private Transform feetTransform;

    // Private get & set
    private Collider2D myCollider2D;
    private float originalThreshold = 0.0f;
    private bool canWalkOnOneWay = true;

    #endregion Variables

    #region Constants & Readonly

    private readonly List<SOneWayPlatformData> data = new List<SOneWayPlatformData>();
    private const float THRESHOLD_GOING_DOWN = 0.0f;

    #endregion Constants & Readonly

    #region Engine

    protected virtual void Awake()
    {
        originalThreshold = threshold;
    }

    private void OnEnable()
    {
        UpdatePlatformsCollisions(true);
    }

    private void Start()
    {
        myCollider2D = GetComponent<Collider2D>();

        if (OneWayPlatform.OneWayPlatforms != null)
        {
            for (int i = 0; i < OneWayPlatform.OneWayPlatforms.Count; i++)
            {
                SOneWayPlatformData sOneWayPlatformData = new SOneWayPlatformData();
                sOneWayPlatformData.Platform = OneWayPlatform.OneWayPlatforms[i];
                data.Add(sOneWayPlatformData);
            }
        }
    }

    private void FixedUpdate()
    {
        UpdatePlatformsCollisions();
    }

    #endregion Engine

    #region Core

    private void UpdatePlatformsCollisions(bool force = false)
    {
        if (myCollider2D == null)
            return;

        for (int i = 0; i < data.Count; i++)
        {
            if (data[i].Platform == null)
                continue;

            Vector3 feetPosition = feetTransform.position + (Vector3.up * originalThreshold);

            // Get a positive or negative value if the player is above or under the platform, respectively
            float dotProduct = Vector2.Dot(data[i].Platform.Transform.up, feetPosition - data[i].Platform.Transform.position);

            dotProduct += data[i].Threshold;

            switch (data[i].State)
            {
                // Check the player collision
                case EOneWayState.CheckCollision:
                    bool ignoreCollision = dotProduct < 0.0f;

                    // If the player is above the platform, but can't walk in one way (e.g Character still jumping), force ignore collision
                    if (!ignoreCollision && !CanWalkOnOneWay)
                        ignoreCollision = true;

                    IgnoreCollision(i, ignoreCollision, force);
                    break;

                // Ignore the player collision when going down
                case EOneWayState.GoingDown:

                    if (dotProduct < -originalThreshold)
                    {
                        SetPlatformData(i, EOneWayState.CheckCollision, data[i].Ignoring, originalThreshold);
                    }
                    break;
            }
        }
    }

    private bool ContainsIndex(int dataIndex) => dataIndex >= 0 && dataIndex < data.Count;

    private void IgnoreCollision(int dataIndex, bool ignore, bool force = false)
    {
        if (!ContainsIndex(dataIndex))
            return;

        if (!force && data[dataIndex].Ignoring == ignore)
            return;

        EOneWayState state = data[dataIndex].State;
        float threshold = data[dataIndex].Threshold;
        SetPlatformData(dataIndex, state, ignore, threshold);
        Physics2D.IgnoreCollision(myCollider2D, data[dataIndex].Platform.Collider2D, ignore);
    }

    private void SetPlatformData(int dataIndex, EOneWayState state, bool ignoring, float threshold)
    {
        if (!ContainsIndex(dataIndex))
            return;

        SOneWayPlatformData newData = data[dataIndex];
        newData.Ignoring = ignoring;
        newData.State = state;
        newData.Threshold = threshold;
        data[dataIndex] = newData;
    }

    #endregion Core

    #region Public Methods

    public void GoDown()
    {
        if (Utils.GlobalSettings == null)
            return;

        RaycastHit2D[] hits = Physics2D.BoxCastAll(
            myCollider2D.bounds.center,
            myCollider2D.bounds.size,
            myCollider2D.transform.rotation.z,
            Vector2.down,
            threshold * 2.0f,
            Utils.GlobalSettings.PlatformLayers);

        for (int i = 0; i < hits.Length; i++)
        {
            OneWayPlatform platform = hits[i].collider.GetComponent<OneWayPlatform>();

            if (platform == null)
                continue;

            int index = data.FindIndex(platformData => platformData.Platform == platform);

            if (index < 0)
                continue;

            IgnoreCollision(index, true);
            SetPlatformData(index, EOneWayState.GoingDown, true, THRESHOLD_GOING_DOWN);
        }
    }

    #endregion Public Methods
}