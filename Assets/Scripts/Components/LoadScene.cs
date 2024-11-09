using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[RequireComponent(typeof(Collider))]
public class LoadScene : MonoBehaviour
{
    #region Variables

    public Collider2D Collider2D { get; private set; }

    [SerializeField] string sceneName;

    private Collider myCollider;
    protected Player player {  get; private set; }

    #endregion // Variables
    
    #region Engine
    
    protected virtual void Awake()
    {
        if(sceneName == string.Empty)
        {
            Destroy(gameObject);
            return;
        }

        Collider2D = GetComponent<Collider2D>();
        myCollider = GetComponent<Collider>();
        myCollider.OnCollisionEnter.AddListener(OnCollision2DEnter);
    }
    
    #endregion // Engine

    #region Events

    private void OnCollision2DEnter(Collider2D collision)
    {
        player = collision.GetComponent<Player>();
        if(player != null)
        {
            Load();
        }
    }

    #endregion // Events

    public virtual void Load()
    {
        LevelManager.Instance.LoadScene(sceneName);
    }
}