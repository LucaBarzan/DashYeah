using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BouncingPlatform : Platform
{
    [SerializeField] private float bouncePower = 15f;
    private Vector2 bounceForce;

    protected override void Awake()
    {
        base.Awake();
        bounceForce = Transform.up * bouncePower;
    }

    protected override void OnCollision2DEnter(Collider2D other)
    {
        base.OnCollision2DEnter(other);
        HandleOnColliderEnter();
    }

    private void HandleOnColliderEnter()
    {
#if UNITY_EDITOR
        bounceForce = Transform.up * bouncePower;
#endif
        if (characterMovement != null)
        {
            characterMovement.ApplyImpulseForce(bounceForce);
        }

        for(int i = 0; i < othersRigidbody2D.Count; i++)
        {
            othersRigidbody2D[i].AddForce(bounceForce, ForceMode2D.Impulse);
        }
    }
}
