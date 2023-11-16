using UnityEngine;

namespace DashYeah.Object
{
    public class Object : MonoBehaviour
    {
        public Transform Transform { get; private set; }

        protected virtual void Awake()
        {
            Transform = transform;
        }

        protected virtual void OnEnable()
        {

        }

        protected virtual void Start()
        {

        }

        protected virtual void Update()
        {

        }

        protected virtual void OnDisable()
        {

        }
    }
}