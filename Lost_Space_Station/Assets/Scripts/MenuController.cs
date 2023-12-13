//Author : Yuko Yamano


using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;
using System.Linq;

public class MenuController : MonoBehaviour
{
    [Header("Sounds")]
    AudioSource m_AudioSource;
    [Tooltip("Element0->SelectSound, Element1->LoadingSoundLevel")]
    public AudioClip[] sfxClips;
    public Toggle toggleMusic;    //Music On/Off button 
    public Slider sliderVolume;    //Music volume slider
    [Tooltip("Please select Easy Toggle which is inside the RightButtomPanle2")]
    public ToggleGroup toggleGroup; //Difficulty (easy,medium, hard) toggle Group(attach in the Easy toggle game object)
    [SerializeField] private Slider slider;
    [Tooltip("For time it takes for the slider to fill to go to level1 ")]
    public float loadingScenedurationSeconds = 2f;
    [Tooltip("For music volume change to this targetVolumne at the end of 'loading' ")]
    float targetVolume;
    [Tooltip(" The amount of sound drop while 'loading' ")]
    public float soundDropAmountWhileLoading = 0.8f;

    [Header("Levels")]
    public Button level1Button, level2Button, level3Button;
    int levelPassed;


    // Start is called before the first frame update
    void Start()
    {
        //DontDestroyOnLoad(this);

        LevelUnlock();

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

    public void LevelUnlock()
    {
        Debug.Log("Level Passed = " + levelPassed);
        //Unlock 
        levelPassed = PlayerPrefs.GetInt("LevelPassed");
        level2Button.interactable = false;
        level3Button.interactable = false;

        switch (levelPassed)
        {
            case 1:
                level2Button.interactable = true;
                break;
            case 2:
                level2Button.interactable = true;
                level3Button.interactable = true;
                break;
            case 3:
                level1Button.interactable = true;
                level2Button.interactable = true;
                level3Button.interactable = true;
                break;
            default:
                break;
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

    

    public void LOAD_MAINGAME()
    {
        //This method is used only for "PLAY" button only. This is to call the "loading" effect in the game.
        slider.gameObject.SetActive(true);
        if (m_AudioSource != null && sfxClips != null)
        {
            m_AudioSource.PlayOneShot(sfxClips[1]);
        }
        StartCoroutine(LoadMainGame(1f, loadingScenedurationSeconds));
    }

    IEnumerator LoadMainGame(float targetValue, float time)
    {
        float elapsedTime = 0f;
        float startValue = slider.value;
        

        float startVolume = m_AudioSource.volume;
        if (m_AudioSource.volume>= soundDropAmountWhileLoading)
        {
             targetVolume = m_AudioSource.volume - soundDropAmountWhileLoading;
        }
        else
        {
             targetVolume = 0.0f;
        }
        

        while (elapsedTime < time)
        {
            // Interpolate slider value
            slider.value = Mathf.Lerp(startValue, targetValue, elapsedTime / time);
            // Interpolate music pitch
            m_AudioSource.volume = Mathf.Lerp(startVolume, targetVolume, elapsedTime / time);
            elapsedTime += Time.deltaTime;

            yield return null;
        }

        // Ensure the slider value reaches exactly the target value
        slider.value = targetValue;
        m_AudioSource.pitch = targetVolume;

        SceneManager.LoadScene("LevelOne");
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

    public void PLAY_SOUND_ONCE(int element)
    {
        if (m_AudioSource != null && sfxClips != null)
        {
            m_AudioSource.PlayOneShot(sfxClips[element]);
        }
    }

}
