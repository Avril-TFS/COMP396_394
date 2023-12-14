using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Medkit : MonoBehaviour
{
    public float rotationSpeed = 10.0f;
    public int healing = 50;
    Scoring scoring;
    AudioManager am;


    // Start is called before the first frame update
    void Start()
    {
        scoring = GameObject.Find("UI").GetComponent<Scoring>();
        am = GameObject.Find("AudioController").GetComponent<AudioManager>();
    }

    // Update is called once per frame
    void Update()
    {
        transform.Rotate(Vector3.up, rotationSpeed * Time.deltaTime);
    }

    void OnTriggerEnter(Collider col)
    {
        if (col.gameObject.tag == "Player")
        {
            //Should play picking up sound later
            am.PLAY_SOUND_ONCE(7);
            scoring.sendMessageToUI("Health increased!");
            Destroy(gameObject);

            PlayerController playerController = col.GetComponent<PlayerController>();
            if (playerController != null)
            {
                playerController.Heal(healing);
            }
        }
    }
}