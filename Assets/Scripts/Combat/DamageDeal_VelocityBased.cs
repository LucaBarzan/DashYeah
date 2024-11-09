using System;
using UnityEngine;

public class DamageDeal_VelocityBased : DamageDeal
{
    [SerializeField] private Rigidbody2D myRigidbody2D;
    [SerializeField] private float minVelocityToApplyDamage = 3.5f;

    protected override void DealDamage(Collider2D other, Health damageable, float damage, bool critical = false)
    {
        if (myRigidbody2D.linearVelocity.magnitude >= minVelocityToApplyDamage)
            base.DealDamage(other, damageable, damage);
    }
}