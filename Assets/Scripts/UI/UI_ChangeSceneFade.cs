using UnityEngine;
using UnityEngine.UI;
using DG.Tweening;
using System;

public class UI_ChangeSceneFade : Singleton<UI_ChangeSceneFade>
{
    #region Variables

    public bool Fading { get; private set; }

    [SerializeField] private Image changeSceneFadeImage;
    [SerializeField] private float fadeDurantion = 2.0f;

    private Tween tween;

    #endregion Variables

    #region Constants & Readonly

    private const float FADE_IN_VALUE = 1.0f;
    private const float FADE_OUT_VALUE = 0.0f;

    #endregion Constants & Readonly

    public void SceneFadeIn(Action OnFadeFinished = null)
    {
        if (tween != null)
            tween.Kill();

        Fading = true;
        tween = changeSceneFadeImage.DOFade(FADE_IN_VALUE, fadeDurantion).OnComplete(() => this.OnFadeFinished(OnFadeFinished));
    }

    public void SceneFadeOut(Action OnFadeFinished = null)
    {
        if (tween != null)
            tween.Kill();

        Fading = true;
        tween = changeSceneFadeImage.DOFade(FADE_OUT_VALUE, fadeDurantion).OnComplete(() => this.OnFadeFinished(OnFadeFinished));
    }

    private void OnFadeFinished(Action OnFadeFinished)
    {
        Fading = false;
        OnFadeFinished?.Invoke();
    }
}