using UnityEngine;

[CreateAssetMenu(fileName = "DamageStats", menuName = "Scriptable Objects/Combat/DamageStats")]
public class SO_DealDamage : SO_DealEffect
{
    public EDamage Type;
    public float Value;
}
