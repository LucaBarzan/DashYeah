using DG.Tweening;
using UnityEngine;

[System.Serializable]
public sealed class UI_Fade
{
    #region Variables
    [HideInInspector] public GameObject GameObject;

    [SerializeField] private CanvasGroup canvasGroup;
    [SerializeField] private bool interactable = true;
    [SerializeField] private float fadeInTime = 0.5f;
    [SerializeField] private float fadeOutTime = 0.5f;

    private Tween fadeAnim;
    #endregion Variables

    public void Update()
    {
        if (fadeAnim != null)
        {
            fadeAnim.ManualUpdate(Time.deltaTime, Time.unscaledDeltaTime);
        }
    }

    #region Core
    public void Show()
    {
        if (fadeAnim != null)
            fadeAnim.Kill();

        GameObject.Enable();

        Tween fadeInAnim = null;
        fadeInAnim = fadeAnim = canvasGroup.DOFade(1.0f, fadeInTime).SetUpdate(UpdateType.Manual, true).SetEase(Ease.InSine).OnComplete(() =>
        {
            if (fadeAnim == fadeInAnim)
            {
                SetCanvasGroupInteractable();
            }
        });
    }

    public void Hide(bool fade = true)
    {
        if (fadeAnim != null)
            fadeAnim.Kill();

        SetCanvasGroupInteractable(false);

        if (fade)
        {
            Tween fadeInAnim = null;
            fadeInAnim = fadeAnim = canvasGroup.DOFade(0.0f, fadeOutTime).SetUpdate(UpdateType.Manual, true).SetEase(Ease.OutSine).OnComplete(() =>
            {
                if (fadeAnim == fadeInAnim)
                {
                    GameObject.Disable();
                }
            });
        }
        else
        {
            canvasGroup.alpha = 0.0f;
            GameObject.Disable();
        }
    }

    public void SetCanvasGroupInteractable(bool interactable = true)
    {
        interactable = interactable && this.interactable;

        canvasGroup.blocksRaycasts = interactable;
        canvasGroup.interactable = interactable;
    }

    #endregion Core
}
