using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;
using UnityEngine.UI;
using UnityEngine.SceneManagement;

public class EscMenu : MonoBehaviour
{
    public GameObject panel; // 在Inspector中指定你想要切换的面板
    public TMP_Text DataBaseInfo;
    public GameObject dataBase;

    public Button button_quit;
    public Button button_reset;

    public TMP_InputField InputNumberSate;

    public Toggle toggle_OpenDataLogging;
    private DataBase data;

    void Start()
    {
        data = dataBase.GetComponent<DataBase>(); 
        panel.SetActive(!panel.activeSelf);
        InputNumberSate.text = string.Empty;
        toggle_OpenDataLogging.isOn = false;

        button_quit.onClick.AddListener(QuitGame);
        button_reset.onClick.AddListener(ResetScene);
    }

    // Update is called once per frame
    void Update()
    {
        if (data.DataBaseAlive)
        {
            DataBaseInfo.text = "MongoDB server is alive";
        }
        else
        {
            DataBaseInfo.text = "MongoDB server is done";
        }
        if (Input.GetKeyDown(KeyCode.Escape)) // 监听ESC键
        {
            if (panel != null)
            {
                panel.SetActive(!panel.activeSelf); // 切换面板的活动状态
            }
        }
    }

    void QuitGame()
    {
#if UNITY_EDITOR
        // If running in the Unity Editor
        UnityEditor.EditorApplication.isPlaying = false;
#else
            // If running in a build version
            Application.Quit();
#endif
    }

    void ResetScene()
    {
        PlayerPrefs.SetString("NumberState", InputNumberSate.text);
        PlayerPrefs.SetInt("DataLogging", toggle_OpenDataLogging.isOn ? 1 : 0);
        SceneManager.LoadScene(SceneManager.GetActiveScene().name);
    }
}
