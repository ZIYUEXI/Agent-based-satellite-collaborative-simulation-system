using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement; // 确保包含这个命名空间

public class GameOver : MonoBehaviour
{
    // Start is called before the first frame update
    void Start()
    {

    }

    // Update is called once per frame
    void Update()
    {
        // Check if the ESC key was pressed
//        if (Input.GetKeyDown(KeyCode.Escape))
//        {
//            Quit the game
//#if UNITY_EDITOR
//            If running in the Unity Editor
//            UnityEditor.EditorApplication.isPlaying = false;
//#else
//             If running in a build version
//            Application.Quit();
//#endif
//        }

        // Check if the TAB key was pressed
        //if (Input.GetKeyDown(KeyCode.Tab))
        //{
        //    // Reload the current scene
        //    SceneManager.LoadScene(SceneManager.GetActiveScene().name);
        //}
    }
}
