using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class GameOver : MonoBehaviour
{
    // Start is called before the first frame update
    void Start()
    {
        Cursor.visible = true;
        Cursor.lockState = CursorLockMode.None;
    }

    public void ContinueButton()
    {
        SceneManager.LoadScene("LevelOne");
    }
    public void MainMenuButton()
    {
        SceneManager.LoadScene("MainMenuScene");
    }
}
