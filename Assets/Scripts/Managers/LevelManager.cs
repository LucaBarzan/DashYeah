using System.Collections;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using UnityEngine;
using UnityEngine.SceneManagement;

public class LevelManager : Singleton<LevelManager>
{
    #region Variables

    public System.Action OnPreLoad;
    public System.Action OnLoadStart;
    public System.Action<float> OnLoadUpdate;
    public System.Action<string> OnLoadFinish;
    public System.Action OnPostLoad;

    public string[] Scenes { get; private set; }
    public string[] Levels { get; private set; }

    private readonly Dictionary<string, int> sceneBuildIndexByName = new Dictionary<string, int>();

    #endregion Variables

    #region Engine

    protected override void Awake()
    {
        base.Awake();
        SetupVariables();
    }

    #endregion Engine

    #region Setup

    private void SetupVariables()
    {
        int sceneCount = SceneManager.sceneCountInBuildSettings;
        Scenes = new string[sceneCount];

        for (int i = 0; i < sceneCount; i++)
        {
            Scenes[i] = Path.GetFileNameWithoutExtension(SceneUtility.GetScenePathByBuildIndex(i));
            sceneBuildIndexByName.Add(Scenes[i], i);
        }

        // Create an array with all scenes from build except the NonGameplayScenes array
        Levels = Scenes.Except(GameManager.Instance.GlobalSettings.NonGameplayScenes).ToArray();
    }

    #endregion Setup

    #region Private

    private IEnumerator LoadSceneCoroutine(int sceneBuildIndex)
    {
        OnPreLoad?.Invoke();

        yield return StartCoroutine(PreLoadSceneCoroutine());

        OnLoadStart?.Invoke();

        yield return Utils.Wait(0.5f);
        yield return StartCoroutine(LoadSceneOperationCoroutine(sceneBuildIndex));
        
        string sceneName = SceneManager.GetActiveScene().name;
        OnLoadFinish?.Invoke(sceneName);

        yield return StartCoroutine(PostLoadSceneCoroutine(sceneName));

        OnPostLoad?.Invoke();
    }

    private IEnumerator PreLoadSceneCoroutine()
    {
        GameManager.Instance.GameState = EGameState.Loading;

        UI_ChangeSceneFade.Instance.SceneFadeIn();

        while (UI_ChangeSceneFade.Instance.Fading)
            yield return null;
    }

    private IEnumerator LoadSceneOperationCoroutine(int sceneBuildIndex)
    {
        var asyncOperation = SceneManager.LoadSceneAsync(sceneBuildIndex, LoadSceneMode.Single);
        asyncOperation.allowSceneActivation = false;

        do
        {
            OnLoadUpdate?.Invoke(asyncOperation.progress / 0.9f);
            yield return null;

        } while (asyncOperation.progress < 0.9f);

        asyncOperation.allowSceneActivation = true;

        while (GameManager.Instance.GameState == EGameState.Loading)
            yield return null;
    }

    private IEnumerator PostLoadSceneCoroutine(string sceneName)
    {
        UI_ChangeSceneFade.Instance.SceneFadeOut();

        while (UI_ChangeSceneFade.Instance.Fading)
            yield return null;
    }

    #endregion Private

    #region Public

    public void LoadScene(string sceneName)
    {
        StartCoroutine(LoadSceneCoroutine(sceneBuildIndexByName[sceneName]));
    }

    public void LoadScene(int sceneBuildIndex)
    {
        StartCoroutine(LoadSceneCoroutine(sceneBuildIndex));
    }

    #endregion Public
}