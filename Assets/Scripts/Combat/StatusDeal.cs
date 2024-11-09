using System;
using UnityEngine;

public class StatusDeal : EffectDealer<SO_DealStatus>
{
    public Action<StatusAffectable> OnDealStatus;

    #region Events

    public override void OnCollisionDetected(Collider2D other) => DealStatus(other);

    #endregion // Events

    #region Core

    public void DealStatus(Collider2D other)
    {
        if (!enabled)
            return;

        StatusAffectable statusAffectable = other.GetComponent<StatusAffectable>();

        if (statusAffectable != null)
        {
            for (int i = 0; i < stats.Statuses.Length; i++)
                statusAffectable.AddStatus(stats.Statuses[i]);

            OnDealStatus?.Invoke(statusAffectable);
        }
    }

    #endregion // Core
}
