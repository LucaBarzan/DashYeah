using UnityEngine;
using UnityEngine.InputSystem;

public class UI_TabMenu : MonoBehaviour
{
    #region Variables

    [SerializeField] private bool useInput;
    [SerializeField] private UI_TabButton[] tabButtons;

    // private 
    private int currentTabIndex;
    private PlayerInputActions.UIActions input;

    #endregion // Variables

    #region Engine

    protected void OnEnable()
    {
        input.NextTab.performed += OnInput_NextTab_Performed;
        input.PreviousTab.performed += OnInput_PreviousTab_Performed;
        SelectTab(currentTabIndex);
    }

    protected void Awake()
    {
        input = InputManager.Instance.UIMap;

        for(int i = 0; i < tabButtons.Length; i++)
        {
            tabButtons[i].TabMenu = this;
        }
    }

    protected void OnDisable()
    {
        if (InputManager.Instance != null)
        {
            input.NextTab.performed -= OnInput_NextTab_Performed;
            input.PreviousTab.performed -= OnInput_PreviousTab_Performed;
        }
    }

    #endregion // Engine

    #region Events

    private void OnInput_NextTab_Performed(InputAction.CallbackContext context) => NextTab();
    private void OnInput_PreviousTab_Performed(InputAction.CallbackContext context) => PreviousTab();

    #endregion // Events

    #region Core

    public void SelectTab(int tabIndex)
    {
        for (int i = 0; i < tabButtons.Length; i++)
        {
            if (i != tabIndex)
                tabButtons[i].DeselectTab();
        }

        currentTabIndex = tabIndex;
        tabButtons[tabIndex].SelectTab();
    }

    public void SelectTab(UI_TabButton tabButton)
    {
        for (int i = 0; i < tabButtons.Length; i++)
        {
            if (tabButtons[i] != tabButton)
                tabButtons[i].DeselectTab();
            else
            {
                currentTabIndex = i;
                tabButtons[i].SelectTab();
            }
        }
    }

    #endregion // Core

    #region Navigation

    private void NextTab()
    {
        if (!useInput)
            return;

        int nextTab = currentTabIndex + 1;
        nextTab = Mathf.Min(nextTab, tabButtons.Length - 1);
        SelectTab(nextTab);
    }

    private void PreviousTab()
    {
        if (!useInput)
            return;

        int previousTab = currentTabIndex - 1;
        previousTab = Mathf.Max(previousTab, 0);
        SelectTab(previousTab);
    }

    #endregion
}