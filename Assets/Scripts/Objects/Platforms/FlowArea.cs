using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class FlowArea : Platform
{
    enum ForceType
    {
        AdditionalForce,
        BaseForce
    }
    [SerializeField] private ForceType forceType = ForceType.AdditionalForce;
    [SerializeField] private float force = 10.0f;

    Vector2 forceDirection;

    protected override void Awake()
    {
        base.Awake();
        forceDirection = Transform.right * force;
    }

    private void FixedUpdate()
    {
#if UNITY_EDITOR
        forceDirection = Transform.right * force;
#endif

        HandlePlayer();
        HandleOthersRigidibody2D();
    }

    private void HandlePlayer()
    {
        if (characterMovement == null)
            return;

        switch (forceType)
        {
            case ForceType.AdditionalForce:
                characterMovement.AddForce(forceDirection);
                break;

            case ForceType.BaseForce:
                characterMovement.AddBaseForce(forceDirection);
                break;

        }
    }

    private void HandleOthersRigidibody2D()
    {
        for (int i = 0; i < othersRigidbody2D.Count; i++)
        {
            othersRigidbody2D[i].AddForce(forceDirection, ForceMode2D.Force);
        }
    }
}
