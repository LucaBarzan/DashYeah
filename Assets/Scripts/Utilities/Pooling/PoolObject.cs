using System;
using UnityEngine;

public class PoolObject : MonoBehaviour
{
    #region Variables

    private Action<PoolObject> OnReleasePoolObject;

    #endregion Variables

    #region Engine

    protected virtual void OnDisable()
    {
        OnReleasePoolObject?.Invoke(this);
    }

    #endregion Engine

    #region Setup

    public void Setup(Action<PoolObject> PoolerReleaseAction)
    {
        OnReleasePoolObject = PoolerReleaseAction;
    }

    #endregion Setup
}
