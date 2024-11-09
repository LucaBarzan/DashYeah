using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;

[RequireComponent(typeof(ParticleSystem))]
public class Particle_Listener : MonoBehaviour
{
    #region Variables
    public UnityEvent OnStarted;
    public UnityEvent OnFinished;

    private ParticleSystem particle;
    private bool raisedFinishedEvent;
    private bool playedOnce;
    #endregion // Variables
    
    #region Engine
    
    void Awake()
    {
        particle = GetComponent<ParticleSystem>();
    }

    private void OnEnable()
    {
        raisedFinishedEvent = false;
        playedOnce = false;
        OnStarted?.Invoke();
    }

    private void Update()
    {
        if(particle.isPlaying)
            playedOnce = true;

        if (!raisedFinishedEvent && playedOnce && !particle.IsAlive(true))
        {
            raisedFinishedEvent = true;
            OnFinished?.Invoke();
        }
    }

    #endregion // Engine

    public void DestroyGameObject()
    {
        Destroy(gameObject);
    }
}
