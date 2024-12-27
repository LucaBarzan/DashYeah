using System;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Pool;

// A pooler base class to share the parentTransform for every variant classes
public abstract class Pooler
{
    public bool Initialized { get; protected set; }
    protected static Transform poolersParentTransform;
    private const string POOLER_ROOT_NAME = "Poolers";
    protected static void Setup()
    {
        if (poolersParentTransform == null)
            poolersParentTransform = new GameObject(POOLER_ROOT_NAME).transform;
    }
}

/// <summary>
/// A simple base class to simplify object pooling in Unity 2021.
/// Derive from this class, call InitPool and you can Get and Release to your hearts content.
/// </summary>
/// <typeparam name="T">A Pool Object you'd like to perform pooling on.</typeparam>
public class Pooler<T> : Pooler where T : PoolObject
{
    #region Variables

    private T prefab;
    public List<T> Objects { get; private set; }

    private ObjectPool<T> pool;

    private ObjectPool<T> Pool
    {
        get
        {
            if (pool == null)
                throw new InvalidOperationException("You need to call InitPool before using it.");

            return pool;
        }
        set => pool = value;
    }

    private Transform poolerRootTransform;

    #endregion Variables

    #region Core
    public void InitPool(GameObject gameObjectSpawner, T prefab, int initial = 10, int max = 20, bool collectionChecks = false)
    {
        Setup();

        poolerRootTransform = new GameObject($"{gameObjectSpawner.name}_{prefab.name}").transform;
        poolerRootTransform.SetParent(poolersParentTransform);

        Objects = new List<T>();
        this.prefab = prefab;
        Pool = new ObjectPool<T>(
            CreateSetup,
            GetSetup,
            ReleaseSetup,
            DestroySetup,
            collectionChecks,
            initial,
            max);

        Initialized = true;
    }

    private void OnKillObject(PoolObject poolObject)
    {
        pool.Release(poolObject as T);
    }

    #endregion Core

    #region Overrides
    protected virtual T CreateSetup()
    {
        var poolObject = GameObject.Instantiate(prefab);
        poolObject.Setup(OnKillObject);
        poolObject.transform.SetParent(poolerRootTransform);

        return poolObject;
    }
    protected virtual void GetSetup(T obj)
    {
        if (!Objects.Contains(obj))
            Objects.Add(obj);
    }

    protected virtual void ReleaseSetup(T obj)
    {
        if (Objects.Contains(obj))
            Objects.Remove(obj);
    }

    protected virtual void DestroySetup(T obj) => GameObject.Destroy(obj);
    #endregion Overrides

    #region Getters
    // Note: If you want to create a specialized "Get" method that better suits your needs,
    // feel free to create a child class that inherits from the Pooler<T> class
    public T Get()
    {
        return Get(Vector3.zero);
    }

    public T Get(Vector3 position, Transform parent = null)
    {
        return Get(position, Quaternion.identity, parent);
    }

    public T Get(Vector3 position, Vector3 direction, Transform parent = null)
    {
        Quaternion rotation = Quaternion.LookRotation(Vector3.forward, direction);
        return Get(position, rotation, parent);
    }

    public T Get(Vector3 position, Quaternion rotation, Transform parent = null)
    {
        T instance = pool.Get();

        if (parent != null)
        {
            instance.transform.SetParent(parent, true);
        }

        instance.transform.position = position;
        instance.transform.rotation = rotation;
        instance.gameObject.Enable();

        return instance;
    }

    public void Release(T obj) => Pool.Release(obj);
    #endregion Getters
}
