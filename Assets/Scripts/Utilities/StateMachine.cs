using System;
using System.Collections.Generic;
using System.Collections;
using UnityEngine;

public sealed class StateMachine
{
    #region Variables

    /* Public Variables */
    public int CurrentState
    {
        get => currentState;
        set
        {
            if (currentState != value && stateEnterActions.ContainsKey(value))
            {
                lastState = currentState;
                currentState = value;

                OnStateChanged(lastState, currentState);
            }
        }
    }

    public int LastState
    {
        get => lastState;
    }

    /* Private Variables */

    private int currentState = -1;
    private int lastState = -1;
    private Action OnUpdate = null;
    private Action OnLateUpdate = null;
    private Action OnFixedUpdate = null;
    private MonoBehaviour monoBehaviour;
    private Coroutine changeUpdateCoroutine;

    #endregion Variables

    #region Constants

    private readonly Dictionary<int, Action> stateUpdateActions = new Dictionary<int, Action>(15);
    private readonly Dictionary<int, Action> stateLateUpdateActions = new Dictionary<int, Action>(15);
    private readonly Dictionary<int, Action> stateFixedUpdateActions = new Dictionary<int, Action>(15);
    private readonly Dictionary<int, Action> stateEnterActions = new Dictionary<int, Action>(15);
    private readonly Dictionary<int, Action> stateExitActions = new Dictionary<int, Action>(15);

    private readonly UnityEngine.WaitForEndOfFrame waitForEndOfFrame = new UnityEngine.WaitForEndOfFrame();

    #endregion Constants

    #region Private Methods

    private void OnStateChanged(int prevState, int nextState)
    {
        if (stateExitActions.ContainsKey(prevState))
            stateExitActions[prevState]?.Invoke();

        if (stateEnterActions.ContainsKey(nextState))
            stateEnterActions[nextState]?.Invoke();

        OnUpdate = null;
        OnLateUpdate = null;
        OnFixedUpdate = null;

        if (prevState >= 0 && monoBehaviour.gameObject.activeInHierarchy)
        {
            if (changeUpdateCoroutine != null)
                monoBehaviour.StopCoroutine(changeUpdateCoroutine);

            changeUpdateCoroutine = monoBehaviour.StartCoroutine(ChangeUpdateStateCoroutine(nextState));
        }
        else
        {
            ChangeUpdateState(nextState);
        }
    }

    private IEnumerator ChangeUpdateStateCoroutine(int newState)
    {
        yield return waitForEndOfFrame;

        ChangeUpdateState(newState);
    }

    private void ChangeUpdateState(int newState)
    {
        if (stateUpdateActions.ContainsKey(newState))
        {
            OnUpdate = stateUpdateActions[newState];
        }

        if (stateLateUpdateActions.ContainsKey(newState))
        {
            OnLateUpdate = stateLateUpdateActions[newState];
        }

        if (stateFixedUpdateActions.ContainsKey(newState))
        {
            OnFixedUpdate = stateFixedUpdateActions[newState];
        }
    }

    private void CheckSetup()
    {
#if UNITY_EDITOR
        if (monoBehaviour == null)
        {
            UnityEngine.Debug.LogError("State machine was not properly setup!");
            UnityEditor.EditorApplication.isPlaying = false;
        }
#endif
    }

    #endregion Private Methods

    #region Public Methods

    public void Setup(UnityEngine.MonoBehaviour monoBehaviour)
    {
        this.monoBehaviour = monoBehaviour;
    }

    public void AddEnterState(int state, Action callback)
    {
        CheckSetup();

        if (!stateEnterActions.ContainsKey(state))
        {
            stateEnterActions.Add(state, callback);
        }
    }

    public void AddUpdateState(int state, Action callback)
    {
        CheckSetup();

        if (!stateUpdateActions.ContainsKey(state))
        {
            stateUpdateActions.Add(state, callback);
        }
    }

    public void AddFixedUpdateState(int state, Action callback)
    {
        CheckSetup();

        if (!stateFixedUpdateActions.ContainsKey(state))
        {
            stateFixedUpdateActions.Add(state, callback);
        }
    }

    public void AddExitState(int state, Action callback)
    {
        CheckSetup();

        if (!stateExitActions.ContainsKey(state))
        {
            stateExitActions.Add(state, callback);
        }
    }

    public void UpdateState()
    {
        CheckSetup();

        OnUpdate?.Invoke();
    }

    public void LateUpdateState()
    {
        CheckSetup();

        OnLateUpdate?.Invoke();
    }

    public void FixedUpdateState()
    {
        CheckSetup();

        OnFixedUpdate?.Invoke();
    }

    public bool HasState(int state)
    {
        CheckSetup();

        return stateEnterActions.ContainsKey(state);
    }

    #endregion Public Methods
}