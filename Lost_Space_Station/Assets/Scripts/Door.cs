using System;
using System.Collections;
using System.Collections.Generic;
using System.Drawing;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.SceneManagement;

public class Door : MonoBehaviour
{
    public PlayerController playerController;
   // public float openDoorDistance = 5;
    public int goodTimeRecord = 30; //the amount of time spent when the player reaches the goal within this second 
    public int bonusRewardPoint = 500;  //Bonus reward point
    Scoring scoring;
    AudioSource audioSource;
    AudioManager am;
    [Header("Level unlocking")]
    int levelPassed;
    
    // Start is called before the first frame update
    void Start()
    {
        scoring = GameObject.Find("UI").GetComponent<Scoring>();
        audioSource = GameObject.Find("AudioController").GetComponent<AudioSource>();
        am = GameObject.Find("AudioController").GetComponent<AudioManager>();
    }
    void OnCollisionEnter(Collision col)
    {
        
        if(col.gameObject.tag == "Player" && playerController.CheckHasKey())
        {
            OpenDoor();
            //Stop music
            
            audioSource.Stop();
            //set levelPassed in player prefabs so that menu controller can unlock the level
            int levelPassed = SceneManager.GetActiveScene().buildIndex;

            if (levelPassed < SceneManager.sceneCountInBuildSettings)
            {
                am.PLAY_SOUND_ONCE(6);
                PlayerPrefs.SetInt("LevelPassed",levelPassed);
            }
                //levelPassed = ;
                Debug.Log("Clear level " + levelPassed);

            //When level 3 is cleared
            //if (levelPassed == 3)
            //{
            //    SceneManager.LoadScene("Victory");
            //}

        }

        if (col.gameObject.tag == "Player" && !playerController.CheckHasKey())
        {
            scoring.sendMessageToUI("You need a key to open this door");
           
        }
    }

    void OpenDoor()
    {
        Destroy(GetComponent<Collider>());
        Destroy(GetComponent<Rigidbody>());
        Destroy(gameObject);

        //transform.Translate(Vector3.down * openDoorDistance);
        GameObject crossHairImage = GameObject.Find("crossHairImage");
        crossHairImage.SetActive(false);
        //Bonus added according to clear time
        if (scoring.timer < goodTimeRecord)
        {
            scoring.score += bonusRewardPoint;
        }

        //Show the Game Clear screen
        scoring.GameClear();
    }
}
