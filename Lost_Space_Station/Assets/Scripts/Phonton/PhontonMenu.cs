using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class PhontonMenu : MonoBehaviour
{
    [Tooltip(" Attach PausePanel in UI ")]
    public GameObject pausePanel; 

    // Start is called before the first frame update
    void Start()
    {
        pausePanel.SetActive(false);
    }

    // Update is called once per frame
    void Update()
    {
        
    }


    public void Resume()
    {
        pausePanel.SetActive(false);
        Time.timeScale = 1f;

    }

    public void LOAD_SCENE(string sceneName)
    {
        SceneManager.LoadScene(sceneName);
    }


    public void QUIT_GAME()
    {
        Application.Quit();
    }
}
