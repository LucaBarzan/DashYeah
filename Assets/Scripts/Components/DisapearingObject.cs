using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using DG.Tweening;

public class DisapearingObject : MonoBehaviour
{
    #region Variables

    /* Private */

    [Header("Required")]
    [SerializeField] private SpriteRenderer spriteRenderer;
    [SerializeField] private float initialDelay = 0.0f;
    [SerializeField] private float visibleTime = 3.0f;
    [SerializeField] private float hiddenTime = 1.0f;

    [Header("Optional")]
    [SerializeField] private new Collider2D collider2D;

    [Header("Fade In")]
    [SerializeField] private float fadeInTime = 1.0f;
    [Range(0f, 1f)]
    [SerializeField] private float timePercentage_EnableCollider = 0.5f;
    [SerializeField] private Ease fadeInEase = Ease.InQuint;

    [Header("Fade Out")]
    [SerializeField] private float fadeOutTime = 1.0f;
    [Range(0f, 1f)]
    [SerializeField] private float timePercentage_DisableCollider = 0.5f;
    [SerializeField] private Ease fadeOutEase = Ease.OutQuint;

    private float visibleTimer = 0f;
    private float hiddenTimer = 0f;
    private Tween fadeAnim;

    #endregion Variables

    #region Constants & Read Only

    private const int STATE_VISIBLE = 0;
    private const int STATE_HIDDEN = 1;
    private const int STATE_FADING = 2;

    private readonly StateMachine stateMachine = new StateMachine();

    #endregion Constants & Read Only

    #region Engine

    private void Awake()
    {
        SetupVariables();
        SetupStateMachine();
    }

    private IEnumerator Start()
    {
        yield return Utils.Wait(initialDelay);
        stateMachine.CurrentState = STATE_VISIBLE;
    }

    private void Update()
    {
        stateMachine.UpdateState();
    }

    #endregion Engine

    #region Setup

    private void SetupVariables()
    {
        stateMachine.Setup(this);
    }

    private void SetupStateMachine()
    {
        stateMachine.AddEnterState(STATE_FADING, Enter_Fading);

        stateMachine.AddEnterState(STATE_VISIBLE, Enter_Visible);
        stateMachine.AddUpdateState(STATE_VISIBLE, Update_Visible);
        stateMachine.AddExitState(STATE_VISIBLE, Exit_Visible);

        stateMachine.AddEnterState(STATE_HIDDEN, Enter_Hidden);
        stateMachine.AddUpdateState(STATE_HIDDEN, Update_Hidden);
        stateMachine.AddExitState(STATE_HIDDEN, Exit_Hidden);
    }

    #endregion Setup

    #region Core

    private void ToggleCollider()
    {
        if (collider2D != null)
            collider2D.enabled = !collider2D.enabled;
    }

    #endregion Core

    #region Events

    private void OnFadeFinished()
    {
        stateMachine.CurrentState = stateMachine.LastState == STATE_VISIBLE ? STATE_HIDDEN : STATE_VISIBLE;
    }

    #endregion Events

    #region State machine

    #region Visible State

    private void Enter_Visible()
    {
        visibleTimer = Time.time + visibleTime;
    }

    private void Update_Visible()
    {
        if (Time.time >= visibleTimer)
            stateMachine.CurrentState = STATE_FADING;
    }

    private void Exit_Visible()
    {

    }

    #endregion Visible State

    #region Hidden State

    private void Enter_Hidden()
    {
        hiddenTimer = Time.time + hiddenTime;
    }

    private void Update_Hidden()
    {
        if (Time.time >= hiddenTimer)
            stateMachine.CurrentState = STATE_FADING;
    }

    private void Exit_Hidden()
    {

    }

    #endregion Hidden State

    #region Fading State

    private void Enter_Fading()
    {
        bool showObject = stateMachine.LastState == STATE_HIDDEN;
        float fadeTime = showObject ? fadeInTime : fadeOutTime;

        // enable/Disable collider when fading
        float toggleTimePercentage = showObject ? timePercentage_EnableCollider : timePercentage_DisableCollider;
        Invoke(nameof(ToggleCollider), fadeTime * toggleTimePercentage);

        // Fade the sprite
        if (fadeAnim != null)
            fadeAnim.Kill();

        float endValue = showObject ? 1.0f : 0.0f;
        Ease ease = showObject ? fadeInEase : fadeOutEase;
        fadeAnim = spriteRenderer.DOFade(endValue, fadeTime).SetEase(ease).OnComplete(OnFadeFinished);
    }

    #endregion Fading State

    #endregion State machine
}
