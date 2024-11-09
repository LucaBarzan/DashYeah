using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;
using UnityEngine.UI;
using UnityEngine.SceneManagement;

public class UI_Button_Level : UI_Button
{
    #region Variables

    public string LevelName = "";
    [SerializeField] private TextMeshProUGUI textMesh;

    #endregion // Variables

    public void LoadLevel()
    {
        LevelManager.Instance.LoadScene(LevelName);
    }

    public void LoadCurrentLevel()
    {
        LevelManager.Instance.LoadScene(SceneManager.GetActiveScene().buildIndex);
    }

    public void SetName(string text)
    {
        textMesh.text = text;
    }
}
