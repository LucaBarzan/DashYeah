using DG.Tweening;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.InputSystem;

[RequireComponent(typeof(Collider))]
public class Interactable : MonoBehaviour
{
    #region Variables

    /* Public Variables */
    public UnityEvent OnInteracted;
    public System.Action<Interactable, Player> OnInteracted_InteractInfo;

    public Transform Transform => myTransform;
    public Collider2D Collider2D { get; private set; }

    /* Protected inspector variables */
    [SerializeField] protected InputActionReference interactInput;
    [SerializeField] protected float delayBetweenInteractions = 0.0f;

    [Header("Optional")]
    [SerializeField] private bool checkForObstacles;
    [SerializeField] private Transform promptIcon_Transform;
    [SerializeField] private float promptIcon_AnimTime = 0.2f;
    [SerializeField] private Ease promptIcon_AnimEase = Ease.InOutSine;

    /* Private variables */
    private Transform myTransform;
    private Collider myCollider;
    private SpriteRenderer promptIconSpriteRenderer;
    private Vector3 promptInitialScale;
    private bool lastInteractionAvailable;
    private Tween anim_PromptInputSprite;
    private Tween anim_PromptInputTransform;
   
    /* Protected variables */
    protected Player player;
    protected bool interactionAvailable;
    protected float delayTime = 0.0f;

    public Player playerRef => player;

    // Input
    private InputAction interactInputAction;
    private Input input;

    #endregion Variables

    #region Constants & Read Onlys

    private readonly List<Collider2D> colliders2D = new List<Collider2D>();

    #endregion Constants

    #region Engine

    protected virtual void Awake()
    {
        myTransform = transform;
        myCollider = GetComponent<Collider>();
        Collider2D = GetComponent<Collider2D>();
        input = new Input();
        interactInputAction = input.FindAction(interactInput.action.name);

        if (promptIcon_Transform != null)
        {
            promptIconSpriteRenderer = promptIcon_Transform.GetComponent<SpriteRenderer>();
            promptInitialScale = promptIcon_Transform.localScale;
        }
    }

    private void Update()
    {
        HandleInteractionAvailability();
        HandlePromptInputVisual();
        lastInteractionAvailable = interactionAvailable;
    }

    private void OnEnable()
    {
        input.Enable();
        myCollider.OnCollisionStay.AddListener(OnCollision2DStay);
        myCollider.OnCollisionExit.AddListener(OnCollision2DExit);
        interactInputAction.performed += OnInputInteract_performed;
        HidePromptInput(false);
    }

    private void OnDisable()
    {
        input.Disable();
        myCollider.OnCollisionStay.RemoveListener(OnCollision2DStay);
        myCollider.OnCollisionExit.RemoveListener(OnCollision2DExit);
        interactInputAction.performed -= OnInputInteract_performed;
    }

    #endregion Engine

    #region Events

    protected virtual void OnInputInteract_performed(InputAction.CallbackContext obj)
    {
        if (interactionAvailable)
        {
            delayTime = Time.time + delayBetweenInteractions;
            OnInteracted?.Invoke();
            OnInteracted_InteractInfo?.Invoke(this, player);
        }
    }

    private void OnCollision2DStay(Collider2D other)
    {
        if (!colliders2D.Contains(other))
        {
            colliders2D.Add(other);

            if (player == null)
                player = other.GetComponent<Player>();
        }
    }

    private void OnCollision2DExit(Collider2D other)
    {
        if (colliders2D.Contains(other))
        {
            colliders2D.Remove(other);

            Player exitPlayer = other.GetComponent<Player>();
            if (player != null && exitPlayer == player)
                player = null;
        }
    }

    #endregion Events

    #region Core

    public void HandleInteractionAvailability()
    {
        if (player == null || Time.time < delayTime)
        {
            interactionAvailable = false;
            return;
        }

        if (!checkForObstacles)
        {
            interactionAvailable = true;
            return;
        }

        Vector2 direction = player.transform.position - Transform.position;
        interactionAvailable = !Physics2D.Raycast(Transform.position, direction.normalized, direction.magnitude, GameManager.Instance.GlobalSettings.ObstacleLayers);
    }

    private void HandlePromptInputVisual()
    {
        if (promptIcon_Transform == null)
            return;

        if (lastInteractionAvailable != interactionAvailable)
        {
            if (interactionAvailable)
                ShowPromptInput();
            else
                HidePromptInput();
        }
    }

    private void ShowPromptInput()
    {
        if (promptIcon_Transform == null)
            return;

        if (anim_PromptInputSprite != null)
            anim_PromptInputSprite.Kill();

        if (anim_PromptInputTransform != null)
            anim_PromptInputTransform.Kill();

        anim_PromptInputSprite = promptIconSpriteRenderer.DOFade(1.0f, promptIcon_AnimTime).SetEase(promptIcon_AnimEase);
        anim_PromptInputTransform = promptIcon_Transform.DOScale(promptInitialScale, promptIcon_AnimTime).SetEase(promptIcon_AnimEase);
    }

    private void HidePromptInput(bool playAnim = true)
    {
        if (promptIcon_Transform == null)
            return;

        if (anim_PromptInputSprite != null)
            anim_PromptInputSprite.Kill();

        if (anim_PromptInputTransform != null)
            anim_PromptInputTransform.Kill();

        if (!playAnim)
        {
            Color temp = promptIconSpriteRenderer.color;
            temp.a = 0.0f;
            promptIconSpriteRenderer.color = temp;

            promptIcon_Transform.localScale = Vector3.zero;
            return;
        }

        anim_PromptInputSprite = promptIconSpriteRenderer.DOFade(0.0f, promptIcon_AnimTime).SetEase(promptIcon_AnimEase);
        anim_PromptInputTransform = promptIcon_Transform.DOScale(Vector3.zero, promptIcon_AnimTime).SetEase(promptIcon_AnimEase);
    }

    #endregion Core
}