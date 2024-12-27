using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class FallingBlock : MonoBehaviour
{
    [SerializeField] private Animator animator;
    [SerializeField] private new Rigidbody2D rigidbody2D;
    [SerializeField] private float shakeTime = 0.5f;

    private Rigidbody2D originalRigidbody2DCache;
    private int state = 0;

    #region Engine

    void Awake()
    {
        originalRigidbody2DCache = new GameObject("OriginalRigidbodyCache").AddComponent<Rigidbody2D>();
        originalRigidbody2DCache.transform.SetParent(transform);
        //originalRigidbody2DCache.CopyValues(rigidbody2D);
        originalRigidbody2DCache.gameObject.Disable();

        rigidbody2D.bodyType = RigidbodyType2D.Kinematic;
    }

    void Update()
    {
        // State 0 waiting to be triggered
        // State 2 falling
        switch (state)
        {
            case 1:
                shakeTime -= Time.deltaTime;
                if (shakeTime <= 0.0f)
                {
                    //rigidbody2D.CopyValues(originalRigidbody2DCache);
                    animator.enabled = false;
                    state++;
                }
                break;
        }
    }

    #endregion Engine

    #region Public Methods

    public void Fall()
    {
        state++;
        animator.enabled = true;
    }

    #endregion Public Methods

}
