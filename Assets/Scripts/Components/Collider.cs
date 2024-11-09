using UnityEngine;
using UnityEngine.Events;

public class Collider : MonoBehaviour
{
    #region Variables

    public UnityEvent<Collider2D> OnCollisionEnter;
    public UnityEvent<Collider2D> OnCollisionStay;
    public UnityEvent<Collider2D> OnCollisionExit;

    public Collider2D Collider2D { get; private set; }
    public ECollisionCheck check = ECollisionCheck.LayerOrTag;
    public LayerMask LayerMaskCollissions;
    public string Tag;

    #endregion // Variables

    #region Constants

    private const string TAG_UNTAGGED = "Untagged";

    #endregion // Constants

    #region Engine

    private void Awake()
    {
        Collider2D = GetComponent<Collider2D>();
        ParticleSystem particle = GetComponent<ParticleSystem>();

        if (Collider2D == null && particle == null && !particle.collision.sendCollisionMessages)
        {
            Debug.LogError($"No collider or particleSystem collider found on {gameObject.name}!!");
            Destroy(this);
        }
    }

    private void OnCollisionEnter2D(Collision2D other)
    {
        Collider2D collider = other.collider;
        OnEnterCollider(in collider);
    }

    private void OnCollisionStay2D(Collision2D other)
    {
        Collider2D collider = other.collider;
        OnStayCollider(in collider);
    }

    private void OnCollisionExit2D(Collision2D other)
    {
        Collider2D collider = other.collider;
        OnExitCollider(in collider);
    }

    private void OnTriggerEnter2D(Collider2D other)
    {
        OnEnterCollider(in other);
    }

    private void OnTriggerStay2D(Collider2D other)
    {
        OnStayCollider(in other);
    }

    private void OnTriggerExit2D(Collider2D other)
    {
        OnExitCollider(in other);
    }

    private void OnParticleCollision(GameObject other)
    {
        Collider2D collider = other.GetComponent<Collider2D>();

        if(collider != null)
            OnEnterCollider(in collider);
    }

    #endregion // Engine

    #region Collisions

    private void OnEnterCollider(in Collider2D other)
    {
        if (!CheckIds(in other))
            return;

        OnCollisionEnter?.Invoke(other);
    }

    private void OnStayCollider(in Collider2D other)
    {
        if (!CheckIds(in other))
            return;
        
        OnCollisionStay?.Invoke(other);
    }

    private void OnExitCollider(in Collider2D other)
    {
        if (!CheckIds(in other))
            return;

        OnCollisionExit?.Invoke(other);
    }

    #endregion // Collisions

    #region Utils

    private bool CheckIds(in Collider2D other)
    {
        if (check == ECollisionCheck.None)
            return true;

        bool layer = true;
        bool tag = true;

        if (LayerMaskCollissions.value != 0)
        {
            layer = other.IsInLayerMask(LayerMaskCollissions);
        }

        if (Tag != null && Tag != string.Empty && Tag != TAG_UNTAGGED)
        {
            tag = other.CompareTag(Tag);
        }

        switch (check)
        {
            case ECollisionCheck.Layer:
                return layer;

            case ECollisionCheck.Tag:
                return tag;

            case ECollisionCheck.LayerAndTag:
                return layer && tag;

            default:
            case ECollisionCheck.LayerOrTag:
                return layer || tag;
        }
    }

    #endregion // Utils
}