using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class UI_Loading : UI_Menu
{
    #region Variables

    [SerializeField] private Slider slider;
    [SerializeField] private float loadingSmoothSpeed = 3.0f;

    private float loadingProgress;

    #endregion Variables
    
    #region Engine
    
    protected override void Start()
    {
        base.Start();
        LevelManager.Instance.OnLoadStart += OnLoadStart;
        LevelManager.Instance.OnLoadUpdate += OnLoadUpdate;
        LevelManager.Instance.OnLoadFinish += OnLoadFinish;
    }

    protected override void Update()
    {
        base.Update();
        slider.value = Mathf.MoveTowards(slider.value, loadingProgress, loadingSmoothSpeed * Time.unscaledDeltaTime);
    }

    #endregion Engine

    #region Events

    protected override void OnGameStateChanged(EGameState state) { }

    private void OnLoadStart()
    {
        Show();
        slider.value = loadingProgress = 0.0f;
    }

    private void OnLoadUpdate(float loadingProgress)
    {
        this.loadingProgress = loadingProgress;
    }

    private void OnLoadFinish(string sceneName)
    {
        loadingProgress = slider.maxValue;
        Hide();
    }

    #endregion Events
}
