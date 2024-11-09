using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using DG.Tweening;

public class UI_Menu : MonoBehaviour
{
    #region Variables

    [SerializeField] private EGameState gameState;
    [SerializeField] private UI_Fade fade;

    protected bool uiEnabled => gameState == GameManager.Instance.GameState;

    #endregion // Variables

    #region Engine

    protected virtual void Awake()
    {
        fade.GameObject = gameObject;
    }

    protected virtual void Start()
    {
        GameManager.Instance.OnGameStateChanged += OnGameStateChanged;
        fade.Hide(false);
    }

    protected virtual void Update()
    {
        fade.Update();
    }

    #endregion // Engine

    #region Events

    protected virtual void OnGameStateChanged(EGameState state)
    {
        if (state == gameState)
        {
            Show();
        }
        else
        {
            Hide();
        }
    }

    #endregion // Events

    #region Core
    protected virtual void Show() => fade.Show();

    protected virtual void Hide(bool fade = true) => this.fade.Hide(fade);

    #endregion // Core
}
