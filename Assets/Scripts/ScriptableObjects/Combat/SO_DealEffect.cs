using UnityEngine;

public abstract class SO_DealEffect : ScriptableObject
{
    [Header("Optional")]
    public HitEffect HitEffectPrefab_Fail;
    public HitEffect HitEffectPrefab_Success;

    [Header("Required")]
    public bool CheckForObstacles = true;
}
