using UnityEngine;

public class UI_Tab : MonoBehaviour
{
    #region Variables

    [SerializeField] private UI_Fade fade;

    #endregion // Variables

    #region Engine

    void Awake()
    {
        fade.GameObject = gameObject;
    }

    void Start()
    {

    }

    private void Update()
    {
        if (fade != null)
        {
            fade.Update();
        }
    }

    #endregion // Engine

    public void Show() => fade.Show();

    public void Hide(bool fade = true) => this.fade.Hide(fade);
}