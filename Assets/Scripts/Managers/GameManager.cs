using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.Linq;
using UnityEngine.InputSystem;
using System;
// using DG.Tweening;

public class GameManager : Singleton<GameManager>
{
    #region Variables

    // Public

    public Action<EGameState> OnGameStateChanged;

    public EGameState GameState
    {
        get => gameState;
        set
        {
            if (gameState == value)
                return;

            if (value == EGameState.LastState)
                value = lastGameState;

            lastGameState = gameState;
            gameState = value;
            OnChangeGameState();
            OnGameStateChanged?.Invoke(gameState);
        }
    }

    public SO_GlobalSettings GlobalSettings => globalSettings;
    
    // Private

    // Game Data
    [SerializeField] private SO_GlobalSettings globalSettings;

    private Input.InGameActions input;

    // Game State
    private EGameState gameState;
    private EGameState lastGameState;

    #endregion Variables

    #region Engine

    protected override void Awake()
    {
        base.Awake();
    }

    private void Start()
    {
        input = InputManager.Instance.InGameActions;
        input.Pause.performed += OnPauseInput_Performed;
    }

    #endregion Engine

    #region Events

    private void OnChangeGameState()
    {
        switch (GameState)
        {
            case EGameState.Menu_Pause:
            case EGameState.Menu_Dialogue:
            case EGameState.Menu_Death:
            case EGameState.Menu_LevelSelect:
            case EGameState.Menu_FastTravel:
            case EGameState.Menu_Inventory:
                Time.timeScale = 0;
                break;
            case EGameState.Menu_Shop:
                Time.timeScale = 0;
                break;
            case EGameState.Menu_Map:
                // Deixar habilitado para pausar o jogo completamente
                // Time.timeScale = 0;
                break;

            default:
                Time.timeScale = 1;
                break;
        }

        if (GameState == EGameState.InGame)
            input.Enable();
        else
            input.Disable();
    }

    private void OnPauseInput_Performed(InputAction.CallbackContext context) => GameState = EGameState.Menu_Pause;

    public void OnPlayerDied()
    {
        // TODO
    }

    public void OnDeathAnimationFinished() => GameState = EGameState.Menu_Death;

    #endregion Events
}