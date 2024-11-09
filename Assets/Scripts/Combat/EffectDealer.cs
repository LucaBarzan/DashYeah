using UnityEngine;

[RequireComponent(typeof(Collider))]
public abstract class EffectDealer<T> : MonoBehaviour where T : SO_DealEffect
{
    #region Variables

    public T Stats => stats;

    [SerializeField] protected T stats;
    protected Transform myTransform;
    protected Collider2D myCollider;

    #endregion // Variables

    #region Constants & ReadOnly

    private readonly Pooler<HitEffect> poolerHitEffect_Fail = new Pooler<HitEffect>();
    private readonly Pooler<HitEffect> poolerHitEffect_Success = new Pooler<HitEffect>();

    #endregion // Constants & ReadOnly

    #region Engine

    void Awake()
    {
        myTransform = transform;
        myCollider = GetComponent<Collider2D>();

        if (stats.HitEffectPrefab_Fail != null)
            poolerHitEffect_Fail.InitPool(gameObject, stats.HitEffectPrefab_Fail);

        if (stats.HitEffectPrefab_Success != null)
            poolerHitEffect_Success.InitPool(gameObject, stats.HitEffectPrefab_Success);
    }

    #endregion // Engine

    #region Events

    public abstract void OnCollisionDetected(Collider2D other);

    #endregion // Events

    // Check if there is any obstacle in between the hitbox and the other object
    protected bool ObstacleFound(Vector2 direction)
    {
        if (!stats.CheckForObstacles)
            return false;

        RaycastHit2D[] hits = Physics2D.RaycastAll(myTransform.position, direction.normalized, direction.magnitude, GameManager.Instance.GlobalSettings.ObstacleLayers);

        bool foundObstacle = false;
        Vector2 hitPoint = Vector2.zero;

        for(int i = 0; i < hits.Length; i++)
        {
            if (hits[i].collider != myCollider)
            {
                hitPoint = hits[i].point;
                foundObstacle = true;
                break;
            }
        }

        if (foundObstacle)
            PlayHitEffect_Fail(hitPoint, direction);

        return foundObstacle;
    }

    protected void PlayHitEffect_Fail(Vector2 position, Vector2 direction) => PlayerHitEffect(poolerHitEffect_Fail, position, direction);

    protected void PlayHitEffect_Success(Vector2 position, Vector2 direction) => PlayerHitEffect(poolerHitEffect_Success, position, direction);

    private void PlayerHitEffect(in Pooler<HitEffect> pooler, Vector2 position, Vector2 direction)
    {
        if (!pooler.Initialized)
            return;
        
        HitEffect hitEffect = pooler.Get(position);
        hitEffect.Direction = direction;
        hitEffect.Show();
    }
}
