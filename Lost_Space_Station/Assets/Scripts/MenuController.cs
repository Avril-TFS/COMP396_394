//Author : Yuko Yamano


using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;

public class MenuController : MonoBehaviour
{
    AudioSource m_AudioSource;
    private bool isMusicOn = true;
    
    // Start is called before the first frame update
    void Start()
    {
        m_AudioSource = GetComponent<AudioSource>();
    }
    public void MusicToggleValueChanged()
    {
        isMusicOn = !isMusicOn;
        if(m_AudioSource != null)
        {
            if (isMusicOn)
            {
                m_AudioSource.Play();   //Both code works. 
                //AudioListener.volume = 0; // Mute all audio
            }
            else
            {
                m_AudioSource.Pause();    //Both code works. 
                //AudioListener.volume = 0.5f; // change the volume
            }
        }
    }


    public void PlayButtonClicked()
    {
        SceneManager.LoadScene("LevelOne");
    }

    public void ExitButtonClicked()
    {
        // Exit the application
        Application.Quit();
    }
}
