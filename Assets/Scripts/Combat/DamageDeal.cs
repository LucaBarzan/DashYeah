using System;
using UnityEngine;

public class DamageDeal : EffectDealer<SO_DealDamage>
{
    public Action<SDamageableInfo> OnDealDamage;

    #region Events

    public override void OnCollisionDetected(Collider2D other) => DealDamage(other, other.GetComponent<Health>(), stats.Value);

    #endregion Events

    #region Core

    protected virtual void DealDamage(Collider2D other, Health damageable, float damage, bool critical = false)
    {
        if (!enabled)
            return;

        Vector2 hitPoint = other.ClosestPoint(transform.position);
        Vector2 direction = hitPoint - (Vector2)myTransform.position;

        // Trying to hit something that is not damageable
        if (damageable == null)
        {
            PlayHitEffect_Fail(hitPoint, direction);
            return;
        }

        if (ObstacleFound(direction))
            return;

        // Deal damage is successful!
        HandleDamageSuccess(other.gameObject, damageable, damage, hitPoint, direction, critical);
    }

    private void HandleDamageSuccess(GameObject other, Health damageable, float damage, Vector2 hitPoint, Vector2 direction, bool critical)
    {
        PlayHitEffect_Success(hitPoint, direction);

        SDamageInfo damageInfo = new SDamageInfo(damage, hitPoint, direction, myTransform.gameObject, stats.Type, critical);
        damageable.TakeDamage(damageInfo);

        SDamageableInfo damageableInfo = new SDamageableInfo(other, damageable);
        OnDealDamage?.Invoke(damageableInfo);
    }

    #endregion Core
}
