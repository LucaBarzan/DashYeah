using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Health : MonoBehaviour
{
    #region Events

    public System.Action<float> OnHealed;
    public System.Action<bool, float> OnInvulnerable;
    public System.Action<SDamageInfo> OnTakeDamage;
    public System.Action<SDamageInfo> OnHealthEmptyInfo;
    public UnityEngine.Events.UnityEvent OnHealthEmpty;
    [SerializeField] private UnityEngine.Events.UnityEvent OnTakeDamages;

    #endregion // Events

    #region Variables

    /* Public */

    // Getters
    public float CurrentHealth => health;
    public float MaxHealth => maxHealth;
    public float HealthPercentage => health / maxHealth;
    public bool Invulnerable => !canTakeDamage;

    public LayerMask IgnoreDamageFromLayers;
    public GameObject[] IgnoreDamageFromObjects;
    public float InvulnerabilityTime = 0f;

    /* Private */

    [SerializeField] private float maxHealth;

    [SerializeField, ReadOnly]
    protected float health;

    private float invulnerabilityTimer;
    private bool canTakeDamage = true;

    #endregion // Variables

    #region Engine

    protected virtual void Awake()
    {
        SetHealth(maxHealth);
    }

    private void Update()
    {
        if (invulnerabilityTimer > 0.0f)
        {
            invulnerabilityTimer -= Time.deltaTime;

            if (invulnerabilityTimer < 0.0f)
                SetCanTakeDamage(true);
        }
    }

    #endregion // Engine

    #region Core

    public virtual void TakeDamage(SDamageInfo damageInfo)
    {
        // Ignore attack if can not take damage

        if (!canTakeDamage)
            return;

        if (damageInfo.AttackerObject != null)
        {
            // Ignore attack if is ignoring the attacker layer
            if (IgnoreDamageFromLayers.value != 0 && damageInfo.AttackerObject.layer.IsInLayerMask(IgnoreDamageFromLayers))
                return;

            // Ignore attack if is ignoring attacker object
            foreach (var gameObject in IgnoreDamageFromObjects)
            {
                if (gameObject == damageInfo.AttackerObject)
                    return;
            }
        }

        // Prevents from applying negative damage
        damageInfo.Damage = Mathf.Max(0, damageInfo.Damage);

        // Apply damage
        SetHealth(health - damageInfo.Damage, damageInfo);

        if (health <= 0.0f)
            return;

        if (InvulnerabilityTime > 0.0f)
        {
            SetCanTakeDamage(false);
            invulnerabilityTimer = InvulnerabilityTime;
            OnInvulnerable?.Invoke(!canTakeDamage, InvulnerabilityTime);
        }

        OnTakeDamage?.Invoke(damageInfo);
        OnTakeDamages?.Invoke();
    }

    public void SetCanTakeDamage(bool canTakeDamage)
    {
        if (invulnerabilityTimer > 0.0f)
        {
            if (canTakeDamage)
                return;

            if (!canTakeDamage)
                invulnerabilityTimer = 0.0f;
        }

        this.canTakeDamage = canTakeDamage;
    }

    public void Heal(float healAmount) => SetHealth(health + healAmount);

    public void SetHealth(float newHealth) => SetHealth(newHealth, default);

    public virtual void SetHealth(float newHealth, SDamageInfo damageInfo)
    {
        if (newHealth < 0.0f)
            newHealth = 0.0f;

        bool increased = newHealth >= health;

        // Prevents Health from being bigger than max health
        health = Mathf.Min(newHealth, maxHealth);

        if (health <= 0.0f)
        {
            OnHealthEmpty?.Invoke();
            OnHealthEmptyInfo?.Invoke(damageInfo);
        }
        else if (increased)
            OnHealed?.Invoke(newHealth - health);
    }
    #endregion // Core
}