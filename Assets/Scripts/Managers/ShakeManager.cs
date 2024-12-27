using Unity.Cinemachine;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Rendering;

public class ShakeManager : Singleton<ShakeManager>
{
    /*
    #region Variables

    [SerializeField] private CinemachineImpulseSource weakImpulseSource;
    [SerializeField] private CinemachineImpulseSource mediumImpulseSource;
    [SerializeField] private CinemachineImpulseSource strongImpulseSource;
    
    private CinemachineImpulseSource impulseSource;

    private CinemachineImpulseDefinition originalWeakImpulseDefinition = new CinemachineImpulseDefinition();
    private CinemachineImpulseDefinition originalMediumImpulseDefinition = new CinemachineImpulseDefinition();
    private CinemachineImpulseDefinition originalStrongImpulseDefinition = new CinemachineImpulseDefinition();

    private EShake currentShakeType;
    private float currentShakeDuration;
    private float currentShakeTimeToEnd;

    #endregion Variables
    
    #region Engine
    
    protected override void Awake()
    {
        base.Awake();

        originalWeakImpulseDefinition.CopyValues(weakImpulseSource.m_ImpulseDefinition);
        originalMediumImpulseDefinition.CopyValues(mediumImpulseSource.m_ImpulseDefinition);
        originalStrongImpulseDefinition.CopyValues(strongImpulseSource.m_ImpulseDefinition);
    }

    #endregion Engine

    #region Utils

    private float GetShakeDuration(EShake shake)
    {
        switch (shake)
        {
            default:
            case EShake.Weak:
                return originalWeakImpulseDefinition.m_ImpulseDuration;

            case EShake.Medium:
                return originalMediumImpulseDefinition.m_ImpulseDuration;

            case EShake.Strong:
                return originalStrongImpulseDefinition.m_ImpulseDuration;
        }
    }

    private void ResetShakeValues()
    {
        weakImpulseSource.m_ImpulseDefinition.m_TimeEnvelope.m_SustainTime = originalWeakImpulseDefinition.m_TimeEnvelope.m_SustainTime;
        mediumImpulseSource.m_ImpulseDefinition.m_TimeEnvelope.m_SustainTime = originalMediumImpulseDefinition.m_TimeEnvelope.m_SustainTime;
        strongImpulseSource.m_ImpulseDefinition.m_TimeEnvelope.m_SustainTime = originalStrongImpulseDefinition.m_TimeEnvelope.m_SustainTime;

        // weakImpulseSource.m_ImpulseDefinition.CopyValues(originalWeakImpulseDefinition);
        // mediumImpulseSource.m_ImpulseDefinition.CopyValues(originalMediumImpulseDefinition);
        // strongImpulseSource.m_ImpulseDefinition.CopyValues(originalStrongImpulseDefinition);
    }

    #endregion Core

    public void Shake(EShake shake, float duration = 0.0f)
    {
        // Check if it is the same shake
        // Check if the current shake finished
        // If it is a different type of shake, override the current shake beign played
        if (currentShakeType == shake &&
            currentShakeDuration == duration &&
            currentShakeTimeToEnd > Time.time)
        {
            return;
        }

        // If there was no time specified, get the inspector default time
        if (duration <= 0.0f)
            duration = GetShakeDuration(shake);

        // Cache the new values
        currentShakeType = shake;
        currentShakeDuration = duration;
        currentShakeTimeToEnd = Time.time + duration;

        impulseSource = weakImpulseSource;

        switch (shake)
        {
            case EShake.Medium:
                impulseSource = mediumImpulseSource;
                break;

            case EShake.Strong:
                impulseSource = strongImpulseSource;
                break;
        }

        // Shake the camera
        impulseSource.m_ImpulseDefinition.m_ImpulseDuration = duration;
        impulseSource.m_ImpulseDefinition.m_TimeEnvelope.m_SustainTime = duration;
        impulseSource.GenerateImpulse();

        // TODO: Shake the controller

        // Reset shake values after the shake has ended
        Invoke(nameof(ResetShakeValues), duration + 0.1f);
    }
    */
}
