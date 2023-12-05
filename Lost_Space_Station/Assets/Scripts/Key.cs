using System;
using System.Collections;
using System.Collections.Generic;
using System.Xml.Linq;
using UnityEngine;

public class Key : MonoBehaviour
{
    public float rotationSpeed = 10.0f;

    public bool isPickedUp = false;
    Scoring scoring;
    AudioManager am;


    private void Start()
    {
        scoring = GameObject.Find("UI").GetComponent<Scoring>();
        am = GameObject.Find("AudioController").GetComponent<AudioManager>();
    }
    void Update()
    {
        transform.Rotate(Vector3.up, rotationSpeed * Time.deltaTime);
    }

    void OnTriggerEnter(Collider col)
    {
        if(col.gameObject.tag == "Player" && !isPickedUp)
        {
            isPickedUp = true;
            am.PLAY_SOUND_ONCE(0);
            Destroy(gameObject);
            //Should play picking up sound later
            scoring.sendMessageToUI("Key picked up! ");
            scoring.KeypickUpUI();
            col.GetComponent<PlayerController>().HasKey();
        }
    }

 
}
