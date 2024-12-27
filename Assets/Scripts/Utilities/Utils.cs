using System.Collections.Generic;
using System.Runtime.InteropServices.WindowsRuntime;
using UnityEngine;
using UnityEngine.UI;

public static class Utils
{
    public static SO_GlobalSettings GlobalSettings
    {
        get
        {
            if (GameManager.Instance == null)
                return null;

            return GameManager.Instance.GlobalSettings;
        }
    }

    #region Variables

    private static readonly Dictionary<float, WaitForSeconds> WaitDictionary = new Dictionary<float, WaitForSeconds>();
    private static readonly Dictionary<float, WaitForSecondsRealtime> WaitDictionaryRealtime = new Dictionary<float, WaitForSecondsRealtime>();

    private static readonly WaitForEndOfFrame waitForEndOfFrame = new WaitForEndOfFrame();

    private static Camera camera;
    public static Camera Camera
    {
        get
        {
            if (camera == null)
                camera = Camera.main;

            return camera;
        }
    }

    #endregion Variables

    #region Basic

    /// <summary> Simplifies the process of enabling a GameObject by setting its active state to true,
    /// ensuring that the activation operation is performed only if the GameObject reference is not null </summary>
    public static void Enable(this GameObject gameObject)
    {
        if (gameObject != null)
            gameObject.SetActive(true);
    }

    /// <summary> Simplifies the process of disabling a GameObject by setting its active state to false,
    /// ensuring that the deactivation operation is executed only if the GameObject reference is not null </summary>
    public static void Disable(this GameObject gameObject)
    {
        if (gameObject != null)
            gameObject.SetActive(false);
    }

    public static bool IsNull(this RaycastHit raycastHit) => raycastHit.collider == null;

    #endregion Basic

    #region Coroutine

    /// <summary> This approach minimizes the creation of redundant WaitForSeconds objects, improving performance in scenarios where WaitForSeconds instances with the same duration are frequently used </summary>
    public static WaitForSeconds Wait(float time)
    {
        if (WaitDictionary.TryGetValue(time, out var wait))
            return wait;

        WaitDictionary[time] = new WaitForSeconds(time);
        return WaitDictionary[time];
    }

    /// <summary> This approach minimizes the creation of redundant WaitForSecondsRealtime objects, improving performance in scenarios where WaitForSecondsRealtime instances with the same duration are frequently used </summary>
    public static WaitForSecondsRealtime WaitRealtime(float time)
    {
        if (WaitDictionaryRealtime.TryGetValue(time, out var wait))
            return wait;

        WaitDictionaryRealtime[time] = new WaitForSecondsRealtime(time);
        return WaitDictionaryRealtime[time];
    }

    public static WaitForEndOfFrame WaitForEndOfFrame() => waitForEndOfFrame;

    #endregion Coroutine

    #region Math

    public static bool Between(this float num, float lower, float upper, bool inclusive = true)
    {
        return inclusive
            ? lower <= num && num <= upper
            : lower < num && num < upper;
    }

    public static bool Between(this int num, int lower, int upper, bool inclusive = true)
    {
        return inclusive
            ? lower <= num && num <= upper
            : lower < num && num < upper;
    }

    public static int Sign(this Vector2 vector2)
    {
        if (vector2.x < 0.0f || vector2.y < 0.0f)
            return -1;

        return 1;
    }

    public static Vector2 AngleToDirection2D(float angle)
    {
        var rot = Quaternion.AngleAxis(angle, Vector3.forward);
        return rot * Vector2.right;
    }

    public static void RotateVector2D(ref this Vector2 vector2, float angle) => vector2 = Quaternion.Euler(0.0f, 0.0f, angle) * vector2;
    public static Vector2 RotateVector2D(Vector2 vector2, float angle) => Quaternion.Euler(0.0f, 0.0f, angle) * vector2;

    public static bool PercentualChance(float chance) => PercentualChance(chance, 0.0f, 1.0f);
    public static bool PercentualChance(float chance, float min, float max) => chance <= Random.Range(min, max);

    #endregion Math

    #region UI

    /// <summary> It simplifies the process of adjusting the alpha (transparency) value of an Image component </summary>
    public static void SetAlpha(this Image image, float alpha)
    {
        Color color = image.color;
        color.a = alpha;
        image.color = color;
    }

    #endregion UI

    #region Layermask

    /// <summary> Checks whether a specified layer is included in the given LayerMask. </summary>
    public static bool ContainsLayer(this LayerMask layerMask, int layer)
    {
        return layer.IsInLayerMask(layerMask);
    }

    /// <summary> Checks whether the layer of the GameObject associated with the Collider2D is included in a specified LayerMask. </summary>
    public static bool IsInLayerMask(this Collider2D collider2D, LayerMask layerMask)
    {
        return collider2D.gameObject.layer.IsInLayerMask(layerMask);
    }

    /// <summary> Checks whether a given layer is included in a specified LayerMask </summary>
    /// <returns> Returns a boolean indicating whether the given layer is part of the specified LayerMask</returns>
    public static bool IsInLayerMask(this int layer, LayerMask layerMask)
    {
        return ((1 << layer) & layerMask) != 0;
    }

    #endregion Layermask

    #region Transform

    /// <summary> Enhanced version of Unity's Vector3 MoveTowards method that update the object's position based on the specified maximum distance delta </summary>
    /// <returns> Returns a boolean indicating whether the destination has been reached during the movement </returns>
    public static bool MoveTowards(this Transform transform, Vector3 targetPosition, float maxDistanceDelta, float threshold = 0.0f)
    {
        transform.position = Vector3.MoveTowards(transform.position, targetPosition, maxDistanceDelta);

        return transform.position.HasReachedDestination(targetPosition, threshold);
    }

    /// <summary> Checks if a current position has reached a target position within a specified threshold distance </summary>
    /// <returns> Returns a boolean indicating whether the current position is considered to have reached the target position
    /// based on the given threshold (default threshold is 0.1 units) </returns>
    public static bool HasReachedDestination(this Vector3 currentPosition, Vector3 targetPosition, float threshold = 0.0f)
    {
        return Vector3.Distance(currentPosition, targetPosition) <= threshold;
    }

    #endregion Transform
}
