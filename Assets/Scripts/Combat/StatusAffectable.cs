using System;
using System.Linq;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;

public class StatusAffectable : MonoBehaviour
{
    #region Variables

    public UnityEvent<EStatus> OnStatusAdded;
    public UnityEvent<EStatus> OnStatusRemoved;

    [SerializeField] private StatusInfoSO info;
    [SerializeField] private List<EStatus> isNotAffectedBy;

    #endregion Variables

    #region Constants & ReadOnlys

    private readonly List<EStatus> statuses = new List<EStatus>();
    private readonly Dictionary<EStatus, float> timers = new Dictionary<EStatus, float>();

    #endregion Constants & ReadOnlys

    #region Engine

    private void Update()
    {
        for (int i = 0; i < statuses.Count; i++)
        {
            if (timers.ContainsKey(statuses[i]))
            {
                
                timers[statuses[i]] -= Time.deltaTime;

                if (timers[statuses[i]] <= 0.0f){
                    RemoveStatus(statuses[i]);
                }
                    
            }
            
        }
    }

    #endregion Engine

    #region Events

    protected virtual void OnStatusAddedEvent(EStatus status) => UpdateStatusTimer(status);

    protected virtual void OnStatusRemovedEvent(EStatus status)
    {
        if (timers.ContainsKey(status))
            timers.Remove(status);
    }

    #endregion Events

    public bool AddStatus(EStatus status)
    {
        if (statuses.Contains(status) || isNotAffectedBy.Contains(status))
        {
            return false;
        }
        statuses.Add(status);
        OnStatusAddedEvent(status);
        OnStatusAdded?.Invoke(status);
        return true;
    }

    public void RemoveStatus(EStatus status)
    {
        if (statuses.Contains(status))
        {
            statuses.Remove(status);
            OnStatusRemoved?.Invoke(status);
            OnStatusRemovedEvent(status);
        }
    }

    private void UpdateStatusTimer(EStatus status)
    {
        // Check if this status use a timer
        SStatusInfo statusInfo = info.statusInfos.FirstOrDefault((x) => x.Status == status && x.UseTimer);

        if (!statusInfo.UseTimer)
            return;

        if (!timers.ContainsKey(status))
        {
            // Could not find a timer for this Status, create a timer
            timers.Add(status, statusInfo.Duration);
        }
        else
        {
            // There is already a timer for this Status, update this timer
            timers[status] = statusInfo.Duration;
        }
    }
}