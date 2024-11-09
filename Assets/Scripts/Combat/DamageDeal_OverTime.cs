using System.Collections.Generic;
using UnityEngine;

public class DamageDeal_OverTime : DamageDeal
{
    #region Variables
    [SerializeField] private float timeToNextDamage = 0.2f;

    private float timer;

    private readonly List<Health> damageables = new List<Health>();
    private readonly List<Collider2D> colliders = new List<Collider2D>();
    #endregion // Variables

    #region Engine
    private void Update()
    {
        if (Time.time < timer && damageables.Count > 0)
            return;

        timer = Time.time + timeToNextDamage;
        for (int i = 0; i < damageables.Count; i++)
            DealDamage(colliders[i], damageables[i], stats.Value);

    }
    #endregion // Engine

    #region Events

    public override void OnCollisionDetected(Collider2D other)
    {
        Health damageable = other.GetComponent<Health>();
        if (damageable != null && !damageables.Contains(damageable))
        {
            colliders.Add(other);
            damageables.Add(damageable);
        }

        base.OnCollisionDetected(other);
    }

    public void OnCollisionExited(Collider2D other)
    {
        Health damageable = other.GetComponent<Health>();
        if (damageable != null && damageables.Contains(damageable))
        {
            colliders.Remove(other);
            damageables.Remove(damageable);
        }
    }

    #endregion // Events
}
