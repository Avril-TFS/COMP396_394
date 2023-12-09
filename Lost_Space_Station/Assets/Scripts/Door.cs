using System;
using System.Collections;
using System.Collections.Generic;
using System.Drawing;
using UnityEngine;
using UnityEngine.SceneManagement;

public class Door : MonoBehaviour
{
    public PlayerController playerController;
    public float openDoorDistance = 5;
    public int goodTimeRecord = 30; //the amount of time spent when the player reaches the goal within this second 
    public int bonusRewardPoint = 500;  //Bonus reward point
    Scoring scoring;

    PhotonPlayerController photonPlayerControllerScript;

    // Start is called before the first frame update
    void Start()
    {
        string currentSceneName = SceneManager.GetActiveScene().name;
        scoring = GameObject.Find("UI").GetComponent<Scoring>();
        //if (currentSceneName == "PhontonLevelOne")
        //{
        //    photonPlayerControllerScript = GameObject.Find("Player").GetComponent<PhotonPlayerController>();
        //}
    }
    void OnCollisionEnter(Collision col)
    {
        string currentSceneName = SceneManager.GetActiveScene().name;
        if (currentSceneName == "PhontonLevelOne")
        {
            if (col.gameObject.tag == "Player" && col.gameObject.GetComponent<PhotonPlayerController>().CheckHasKey())
            {
                OpenDoor();
                Debug.Log("Clear");
            }

            if (col.gameObject.tag == "Player" && !col.gameObject.GetComponent<PhotonPlayerController>().CheckHasKey())
            {
                scoring.sendMessageToUI("You need a key to open this door");

            }
        }


        if (currentSceneName != "PhontonLevelOne")
        {
            if (col.gameObject.tag == "Player" && playerController.CheckHasKey())
            {
                OpenDoor();
                Debug.Log("Clear");
            }

            if (col.gameObject.tag == "Player" && !playerController.CheckHasKey())
            {
                scoring.sendMessageToUI("You need a key to open this door");

            }
        }
           
    }

    void OpenDoor()
    {
        string currentSceneName = SceneManager.GetActiveScene().name;
        transform.Translate(Vector3.down * openDoorDistance);
        GameObject crossHairImage = GameObject.Find("crossHairImage");

        if (currentSceneName != "PhontonLevelOne")
        {
            crossHairImage.SetActive(false);
        }


          
        //Bonus added according to clear time
        if (scoring.timer < goodTimeRecord)
        {
            scoring.score += bonusRewardPoint;
        }

        //Show the Game Clear screen
        scoring.GameClear();
    }
}
