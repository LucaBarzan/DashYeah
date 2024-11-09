using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Spin : Object
{
    [System.Flags]
    enum Orientation
    {
        RotateX = 1 << 0,
        RotateY = 1 << 1,
        RotateZ = 1 << 2
    }

    [SerializeField] float rotateSpeed = 10.0f;
    [SerializeField] Orientation orientation = Orientation.RotateY;

    private Vector3 axis;

    protected override void Awake()
    {
        base.Awake();
        SetupAxisOrientation();
    }

    private void Update()
    {
#if UNITY_EDITOR
        SetupAxisOrientation();
#endif
        Transform.Rotate(rotateSpeed * Time.deltaTime * axis);
    }

    private void SetupAxisOrientation()
    {
        int x = orientation.HasFlag(Orientation.RotateX) ? 1 : 0;
        int y = orientation.HasFlag(Orientation.RotateY) ? 1 : 0;
        int z = orientation.HasFlag(Orientation.RotateZ) ? 1 : 0;
        axis = new Vector3(x, y, z);
    }
}