using UnityEngine;

[CreateAssetMenu(fileName = "DamageStats", menuName = "Aumakua/Combat/DamageStats")]
public class SO_DealDamage : SO_DealEffect
{
    public EDamage Type;
    public float Value;
}
