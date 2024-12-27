using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ChangeGameState : MonoBehaviour
{
    #region Variables

    public EGameState GameState;
    public bool OnStart = false;

    #endregion Variables

    #region Engine

    void Start()
    {
        if (OnStart)
            ChangeState();
    }

    #endregion Engine

    public void ChangeState() => GameManager.Instance.GameState = GameState;
}
