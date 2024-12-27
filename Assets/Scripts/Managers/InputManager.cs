using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class InputManager : Singleton<InputManager>
{
    #region Variables

    /* Public */
    public Input.UIActions UIActions => input.UI;
    public Input.InGameActions InGameActions => input.InGame;
    public Input.PlayerActions PlayerActions => input.Player;

    /* Private */
    private Input input;

    #endregion Variables

    #region Engine

    protected override void Awake()
    {
        base.Awake();

        SetupComponents();
    }

    private void OnEnable() => SubscribeToGameManagerEvents(true);

    private void OnDisable() => SubscribeToGameManagerEvents(false);

    #endregion Engine

    #region Setup

    private void SetupComponents() => input = new Input();

    #endregion Setup

    #region Events

    #region Subscriptions

    private void SubscribeToGameManagerEvents(bool subscribe)
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

    #endregion Subscriptions

    private void OnGameStateChanged(EGameState gameState)
    {
        if (gameState == EGameState.InGame)
        {
            input.InGame.Enable();
            input.UI.Disable();
        }
        else
        {
            input.InGame.Disable();
            input.UI.Enable();
        }
    }

    #endregion Events
}
