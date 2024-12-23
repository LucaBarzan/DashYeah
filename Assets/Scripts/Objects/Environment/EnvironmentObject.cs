using System.Collections.Generic;
using UnityEngine;

[System.Serializable]
public class EnvironmentObject : MonoBehaviour
{
    #region Variables

    /* Public */
    public System.Action<EnvironmentArea> OnEnvironmentChanged;

    public List<EnvironmentArea> Environments { get; private set; }
    public EnvironmentArea Environment { get; private set; }

    private Transform myTransform;

    #endregion // Variables

    #region Engine

    private void Awake()
    {
        myTransform = transform;
        Environments = new List<EnvironmentArea>();
    }

    private void Update()
    {
        // GatherEnvironments();
    }

    #endregion // Engine

    #region Core

    protected void GatherEnvironments()
    {
        if (Environments.Count == 0)
        {
            ChangeEnvironment(null);
            return;
        }

        ChangeEnvironment(Environments[Environments.Count - 1]);
    }

    #endregion // Core

    #region Events

    public virtual void OnEnvironmentEnter(EnvironmentArea newEnvironment)
    {
        if (!Environments.Contains(newEnvironment))
            Environments.Add(newEnvironment);

        GatherEnvironments();
    }

    public virtual void OnEnvironmentExit(EnvironmentArea oldEnvironment)
    {
        if (Environments.Contains(oldEnvironment))
            Environments.Remove(oldEnvironment);

        GatherEnvironments();
    }

    #endregion // Events

    #region Utils

    private void ChangeEnvironment(EnvironmentArea newEnvironment)
    {
        Environment = newEnvironment;
        OnEnvironmentChanged?.Invoke(Environment);
    }

    public void CopyEnvironmentObjectValues(EnvironmentObject other)
    {
        Environment = other.Environment;
        Environments = other.Environments;
    }

    #endregion // Utils

}
