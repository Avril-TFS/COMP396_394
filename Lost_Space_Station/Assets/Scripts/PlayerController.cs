//Author : Yuko Yamano


using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerController : MonoBehaviour
{
    public float moveSpeed = 5.0f;
    public Transform playerBody; // Reference to the player model 
    private Rigidbody rb;

    // This is for keys---------------
    public bool hasKey = false;

    public void HasKey()
    {
        hasKey = true;
    }
    public bool CheckHasKey()
    {
        return hasKey;
    }//-------------------------------

    public int health = 100;

    Scoring scoring;
    public int attackStrength = 20; 

    private void Start()
    {
        rb = GetComponent<Rigidbody>();
        playerBody = transform.Find("PlayerBody");
        scoring = GameObject.Find("UI").GetComponent<Scoring>();

    }

    private void Update()
    {
        // Player movement
        float horizontalInput = Input.GetAxis("Horizontal");
        float verticalInput = Input.GetAxis("Vertical");


        // Calculate the movement direction relative to the player's local space
        Vector3 movement = playerBody.TransformDirection(new Vector3(horizontalInput, 0, verticalInput)) * moveSpeed;
        rb.velocity = movement;

        if (health <= 0)
        {
            Die();
        }
    }

    private void Die()
    {
        scoring.GameOver();
    }

    public void Damage(int amount)
    {
        health -= amount;
    }
}
