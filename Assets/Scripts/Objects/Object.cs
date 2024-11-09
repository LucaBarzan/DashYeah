using UnityEngine;

public abstract class Object : MonoBehaviour
{
    public Transform Transform { get; private set; }

    protected virtual void Awake() => Transform = transform;
}