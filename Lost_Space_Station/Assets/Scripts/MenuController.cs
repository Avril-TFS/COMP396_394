//Author : Yuko Yamano


using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;
using System.Linq;

public class MenuController : MonoBehaviour
{
    AudioSource m_AudioSource;
    //private bool isMusicOn = true;

    public Toggle toggleMusic;    //Music On/Off button 
    public Slider sliderVolume;    //Music volume slider
    [Tooltip("Please select Easy Toggle which is inside the RightButtomPanle2")]
    public ToggleGroup toggleGroup; //Difficulty (easy,medium, hard) toggle Group(attach in the Easy toggle game object)

    // Start is called before the first frame update
    void Start()
    {
        DontDestroyOnLoad(this);
       
        m_AudioSource = GetComponent<AudioSource>();
        
        //LOAD_PREFERENCES is not used at this point. 
        LOAD_PREFERENCES();
    }
    private void Update()
    {
       
        if (Input.GetKeyDown(KeyCode.Escape))
        {
#if UNITY_EDITOR
            UnityEditor.EditorApplication.isPlaying = false;
#else
        Application.Quit();
#endif
        }

        
    }

  


    public void MusicToggleValueChanged()
    {
       
        if(m_AudioSource != null)
        {
            if(toggleMusic.isOn)
            {
                m_AudioSource.Play();
            }
            else
            {
                m_AudioSource.Pause();
            }
        }


    }

    public void MusicVolumeChanged(float newSlideValue)
    {
        if (m_AudioSource != null)
        {
            if (sliderVolume != null)
            {
                //sliderVolume.value = m_AudioSource.volume;
                m_AudioSource.volume = newSlideValue;
            }
            else
            {
                Debug.LogError("Slider component is null. Make sure it is assigned.");
            }
        }
        else
        {
            Debug.LogError("AudioSource component is null. Make sure it is initialized.");
        }
    }

    public string DifficultyLevelSlect()
    {
        //Get the label in activated toggles
        string selectedLabel = toggleGroup.ActiveToggles().First().GetComponentsInChildren<Text>().First(t => t.name == "Label").text;
        //Debug.Log("Difficulty level selected: " + selectedLabel);
        return selectedLabel;

        
    }


    public void LOAD_SCENE(string sceneName)
    {
        SceneManager.LoadScene(sceneName);
    }

    public void LOAD_SCENE(int sceneIndex)
    {
        SceneManager.LoadScene(sceneIndex);
    }

    public void ExitButtonClicked()
    {
#if UNITY_EDITOR
#else
    Application.Quit();
#endif
    }


    public void LOAD_PREFERENCES()
    {
        if (PlayerPrefs.HasKey("Music"))
        {
            string sMusic = PlayerPrefs.GetString("Music");
            toggleMusic.isOn = (sMusic == "True" ? true : false);
            //print("Fomr LOAD_PREFERENCES sMusic: " + sMusic);
            //print("From LOAD_PREFERENCES:" + toggleMusic.isOn);
            if (toggleMusic.isOn)
            {
                m_AudioSource.Play();
            }
        }

        if (PlayerPrefs.HasKey("Volume"))
        {
            sliderVolume.value = PlayerPrefs.GetFloat("Volume");

        }
        if (PlayerPrefs.HasKey("Difficulty"))
        {
            Debug.Log("The previous difficulty selected was: " + PlayerPrefs.GetString("Difficulty"));
        }

    }

    public void SAVE_PREFERENCES()
    {
        PlayerPrefs.SetString("Music", toggleMusic.isOn.ToString());
        PlayerPrefs.SetFloat("Volume", sliderVolume.value);
        //difficulty 
        PlayerPrefs.SetString("Difficulty", DifficultyLevelSlect().ToString());
        print("Music is saved as: " + toggleMusic.isOn.ToString());
        print("Volume is saved as: " + sliderVolume.value);
        print("Difficulty" + DifficultyLevelSlect().ToString());
    }



}
