using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class UI_Button : MonoBehaviour
{
    #region Variables

    public Button Button { get; private set; }
    public RectTransform RectTransform { get; private set; }

    #endregion Variables

    #region Engine

    protected virtual void Awake()
    {
        Button = GetComponent<Button>();
        RectTransform = GetComponent<RectTransform>();
    }

    #endregion Engine
}
