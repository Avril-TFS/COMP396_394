using System;
using System.Collections;
using System.Collections.Generic;
using System.Drawing;
using UnityEngine;

public class Door : MonoBehaviour
{
    public PlayerController playerController;
   // public float openDoorDistance = 5;
    public int goodTimeRecord = 30; //the amount of time spent when the player reaches the goal within this second 
    public int bonusRewardPoint = 500;  //Bonus reward point
    Scoring scoring;

    // Start is called before the first frame update
    void Start()
    {
        scoring = GameObject.Find("UI").GetComponent<Scoring>();
    }
    void OnCollisionEnter(Collision col)
    {
        
        if(col.gameObject.tag == "Player" && playerController.CheckHasKey())
        {
            OpenDoor();
            Debug.Log("Clear");
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
