using UnityEngine;

public struct SDamageInfo
{
    public float Damage;
    public Vector2 Point;
    public Vector2 Direction;
    public GameObject AttackerObject;
    public EDamage Type;
    public bool Critical;

    public SDamageInfo(float damage, Vector2 point, Vector2 direction, GameObject attackerObject, EDamage type, bool critical)
    {
        Damage = damage;
        Point = point;
        Direction = direction;
        AttackerObject = attackerObject;
        Type = type;
        Critical = critical;
    }

    public SDamageInfo(float damage, Vector2 point, Vector2 direction, GameObject attackerObject, EDamage type)
        : this(damage, point, direction, attackerObject, type, false)
    {
        Damage = damage;
        Point = point;
        Direction = direction;
        AttackerObject = attackerObject;
        Type = type;
    }

    public SDamageInfo(float damage, Vector2 point, Vector2 direction, GameObject attackerObject)
        : this(damage, point, direction, attackerObject, EDamage.Default)
    {

    }
}

public struct SDamageableInfo
{
    public GameObject DamageableObject;
    public Health Damageable;

    public SDamageableInfo(GameObject damageableObject, Health damageable)
    {
        DamageableObject = damageableObject;
        Damageable = damageable;
    }
}

[System.Serializable]
public struct SCriticDamage
{
    public float Chance;
    public float DamageMultiplier;
}

[System.Serializable]
public struct SStatusInfo
{
    public EStatus Status;
    public bool UseTimer;
    public float Duration;
}