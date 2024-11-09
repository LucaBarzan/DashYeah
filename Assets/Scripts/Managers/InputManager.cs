using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class InputManager : Singleton<InputManager>
{
    #region Variables

    /* Public */
    public PlayerInputActions.DebugActions DebugMap => input.Debug;
    public PlayerInputActions.UIActions UIMap => input.UI;
    public PlayerInputActions.InGameActions InGameMap => input.InGame;
    public PlayerInputActions.MauiActions MauiMap => input.Maui;
    public PlayerInputActions.DolphinActions DolphinMap => input.Dolphin;
    public PlayerInputActions.SpiderActions SpiderMap => input.Spider;
    public PlayerInputActions.GeckoActions GeckoMap => input.Gecko;

    /* Private */
    private PlayerInputActions input;

    #endregion // Variables

    #region Engine

    protected override void Awake()
    {
        base.Awake();

        SetupComponents();
    }

    private void OnEnable()
    {
        SubscribeToGameManagerEvents(true);
    }

    void Start()
    {
        
    }

    private void OnDisable()
    {
        SubscribeToGameManagerEvents(false);
    }

    #endregion // Engine

    #region Setup

    private void SetupComponents()
    {
        input = new PlayerInputActions();
    }

    #endregion // Setup

    #region Events

    #region Subscriptions

    private void SubscribeToGameManagerEvents(bool subscribe)
    {
        if(subscribe)
        {
            GameManager.Instance.OnGameStateChanged += OnGameStateChanged;
        }
        else
        {
            GameManager.Instance.OnGameStateChanged -= OnGameStateChanged;
        }
    }

    #endregion // Subscriptions

    private void OnGameStateChanged(EGameState gameState)
    {
        if(gameState == EGameState.InGame)
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

    #endregion // Events
}
