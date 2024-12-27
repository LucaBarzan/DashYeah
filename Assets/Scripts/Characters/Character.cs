using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public abstract class Character : Object
{
    public virtual bool CanWalkOnOneWay => true;

    protected virtual void OnEnable()
    {
        SubscribeToGameManagerEvents();
    }

    protected virtual void OnDisable()
    {
        SubscribeToGameManagerEvents(false);
    }

    private void SubscribeToGameManagerEvents(bool subscribe = true)
    {
        if (GameManager.Instance == null)
            return;

        if (subscribe)
        {
            GameManager.Instance.OnGameStateChanged += OnGameStateChanged;
        }
        else
        {
            GameManager.Instance.OnGameStateChanged -= OnGameStateChanged;
        }
    }

    protected abstract void OnGameStateChanged(EGameState gameState);
    public abstract void OnTakeDamage(SDamageInfo damageInfo);
    public abstract void OnHealthEmpty();


}