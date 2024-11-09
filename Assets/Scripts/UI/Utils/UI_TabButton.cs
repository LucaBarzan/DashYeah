using UnityEngine;
using UnityEngine.UI;

public class UI_TabButton : MonoBehaviour
{
    #region Variables

    public bool Selected { get; private set; }

    [HideInInspector] public UI_TabMenu TabMenu;
    [SerializeField] private UI_Tab tab;
    [SerializeField] private TMPro.TMP_Text title;
    [SerializeField] private Image image;
    [SerializeField] private Color activatedColor = Color.white;
    [SerializeField] private Color deactivatedColor = Color.white;

    #endregion // Variables

    #region Engine

    UI_TabButton()
    {
        Selected = true;
    }


    #endregion // Engine

    public void SelectTab()
    {
        if (Selected)
            return;

        Selected = true;
        title.alpha = activatedColor.a;
        image.color = activatedColor;
        tab.Show();
        TabMenu.SelectTab(this);
    }

    public void DeselectTab()
    {
        if (!Selected)
            return;

        Selected = false;
        title.alpha = deactivatedColor.a;
        image.color = deactivatedColor;
        tab.Hide();
    }
}
