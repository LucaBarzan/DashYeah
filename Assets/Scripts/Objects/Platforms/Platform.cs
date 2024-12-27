using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Platform : MonoBehaviour
{
    #region Variables

    public Transform Transform { get; private set; }
    public Collider2D Collider2D { get; private set; }

    [SerializeField] protected Collider myCollider;

    protected CharacterMovement characterMovement;

    #endregion Variables

    #region Constants & readonly

    protected readonly List<Rigidbody2D> othersRigidbody2D = new List<Rigidbody2D>();

    #endregion Constants & readonly

    #region Engine

    protected virtual void Awake()
    {
        Transform = transform;
        Collider2D = myCollider.GetComponent<Collider2D>();
    }

    protected virtual void Start()
    {

    }

    private void OnEnable()
    {
        SubscribeToColliderEvents();
    }

    private void OnDisable()
    {
        SubscribeToColliderEvents(false);
    }

    #endregion Engine

    #region Events

    #region Subscription

    private void SubscribeToColliderEvents(bool subscribe = true)
    {
        if (subscribe)
        {
            myCollider.OnCollisionEnter.AddListener(OnCollision2DEnter);
            myCollider.OnCollisionExit.AddListener(OnCollision2DExit);
        }
        else
        {
            myCollider.OnCollisionEnter.RemoveListener(OnCollision2DEnter);
            myCollider.OnCollisionExit.RemoveListener(OnCollision2DExit);
        }
    }

    #endregion Subscription

    protected virtual void OnCollision2DEnter(Collider2D other)
    {
        CharacterMovement characterMovement = other.GetComponent<CharacterMovement>();

        if (characterMovement != null)
        {
            this.characterMovement = characterMovement;
            return;
        }

        Rigidbody2D otherRigidbody2D = other.GetComponent<Rigidbody2D>();
        if (otherRigidbody2D != null && !othersRigidbody2D.Contains(otherRigidbody2D))
        {
            othersRigidbody2D.Add(otherRigidbody2D);
        }
    }

    protected virtual void OnCollision2DExit(Collider2D other)
    {
        CharacterMovement characterMovement = other.GetComponent<CharacterMovement>();

        if (characterMovement != null && characterMovement == this.characterMovement)
        {
            this.characterMovement = null;
            return;
        }

        Rigidbody2D otherRigidbody2D = other.GetComponent<Rigidbody2D>();

        if (othersRigidbody2D.Contains(otherRigidbody2D))
        {
            othersRigidbody2D.Remove(otherRigidbody2D);
        }
    }

    #endregion Events
}