using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class PickUpObject : MonoBehaviour
{
    #region Variables
    public System.Action OnObjectPickedUp;
    public System.Action OnObjectThrown;

    public bool PickedUp { get; private set; }
    public Transform Transform { get; private set; }

    [SerializeField] private Rigidbody2D myRigidbody2D;
    [SerializeField] private Collider2D myCollider2D;
    [SerializeField] private Interactable interactable;
    [SerializeField] private Transform pickUpPositionTransform;
    [SerializeField] private float speed = 15.0f;

    private int originalLayerValue;
    private Rigidbody2D originalRigidbody2DCache;
    private Collider2D otherCollider2D;
    // private MauiAbilities mauiAbilities;
    // private Maui maui;

    #endregion Variables

    #region Engine

    void Awake()
    {
        Transform = transform;
        originalLayerValue = gameObject.layer;
        GameObject originalRigidbody = new GameObject("OriginalRigidbodyCache");
        originalRigidbody.transform.SetParent(Transform, false);
        originalRigidbody2DCache = originalRigidbody.AddComponent<Rigidbody2D>();
        // originalRigidbody2DCache.CopyValues(in myRigidbody2D);
        originalRigidbody2DCache.gameObject.Disable();
    }

    void Start()
    {
        interactable.OnInteracted_InteractInfo += OnInteracted;
    }

    private void Update()
    {
        if (PickedUp)
        {
            // Vector2 targetPosition = mauiAbilities.PickUpPositionTransform.position - pickUpPositionTransform.localPosition;
            // Transform.MoveTowards(targetPosition, speed * Time.deltaTime);
        }
    }

    #endregion Engine

    #region Events

    public void OnInteracted(Interactable interactable, Player player)
    {
        /*
        Maui maui = player as Maui;

        // Pick up this object
        if (maui != null && maui.OnPickUpObject(this))
        {
            this.maui = maui;
            mauiAbilities = maui.PlayerAbilities as MauiAbilities;
            PickedUp = true;
            myRigidbody2D.isKinematic = true;
            gameObject.layer = Utils.GlobalSettings.IgnoreRaycastLayer;
            otherCollider2D = maui.CapsuleCollider2D;
            Physics2D.IgnoreCollision(myCollider2D, otherCollider2D, true);
            OnObjectPickedUp?.Invoke();
        }
        */
    }

    public void OnRelease()
    {
        ResetPickUpValues();
        OnObjectThrown?.Invoke();
    }

    public void OnThrow(Vector2 throwForce)
    {
        myRigidbody2D.AddForce(throwForce, ForceMode2D.Impulse);
        OnObjectThrown?.Invoke();
        ResetPickUpValues();
    }

    #endregion Events

    #region Core

    private void ResetPickUpValues()
    {
        StartCoroutine(Reset());
    }

    #endregion Utils

    private IEnumerator Reset()
    {
        PickedUp = false;
        //maui = null;          
        //myRigidbody2D.CopyValues(in originalRigidbody2DCache);
        gameObject.layer = originalLayerValue;
        yield return Utils.Wait(0.25f);
        Physics2D.IgnoreCollision(myCollider2D, otherCollider2D, false);
        yield return null;
    }
}
