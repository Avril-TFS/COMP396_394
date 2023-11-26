//Author : Yuko Yamano


using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerController : MonoBehaviour
{
    public float moveSpeed = 5.0f;
    public Transform playerBody; // Reference to the player model 
    private GameObject playerBodyObject;
    private Rigidbody rb;
    //for the bullet
    public Transform shootPoint;
    public GameObject bulletPrefab;
    public float shootingInterval = 0.5f;
    public float bulletSpeed = 10f; // Set your default bullet speed here
    public float bulletLifetime = 2f; // Time in seconds before the bullet is destroyed
    private float lastShotTime;
   


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
        playerBodyObject = GameObject.Find("PlayerBody");
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

        //Shooting
        if (Input.GetMouseButtonDown(0))
        {
            ShootBullet();
        }
        if (Input.GetKeyDown(KeyCode.Escape))
        {
#if UNITY_EDITOR
            UnityEditor.EditorApplication.isPlaying = false;
#else
        Application.Quit();
#endif
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

    //-----------Shoot script
    //void ShootBullet()
    //{
    //    // Check if enough time has passed since the last shot
    //    if (Time.time - lastShotTime >= shootingInterval)
    //    {
    //        // Find the camera component on the player
    //        Camera playerCamera = playerBodyObject.GetComponentInChildren<Camera>();
    //        if (playerCamera != null)
    //        {
    //            GameObject bullet = Instantiate(bulletPrefab, transform.position, Quaternion.identity);

    //            Rigidbody bulletRb = bullet.GetComponent<Rigidbody>();

    //            if (bulletRb != null)
    //            {
    //                Vector3 bulletVelocity = playerCamera.transform.forward * bulletSpeed;
    //                bulletRb.velocity = bulletVelocity;

    //                // Set the rotation of the bullet to match its velocity
    //                bullet.transform.rotation = Quaternion.LookRotation(bulletVelocity);

    //                // Destroy the bullet after the specified lifetime
    //                Destroy(bullet, bulletLifetime);


    //            }
    //            else
    //            {
    //                Debug.LogError("Bullet prefab is missing Rigidbody2D component!");
    //            }
    //        }

    //        // Update the last shot time
    //        lastShotTime = Time.time;
    //    }
    //}

    void ShootBullet()
    {
        // Check if enough time has passed since the last shot
        if (Time.time - lastShotTime >= shootingInterval)
        {
            // Find the camera component on the player
            Camera playerCamera = playerBodyObject.GetComponentInChildren<Camera>();
            if (playerCamera != null)
            {
                // Get the center of the screen in viewport coordinates
                Vector3 screenCenter = new Vector3(0.5f, 0.5f, playerCamera.nearClipPlane);

                // Convert the screen center to a ray from the camera
                Ray ray = playerCamera.ViewportPointToRay(screenCenter);

                // Instantiate the bullet at the center of the screen
                GameObject bullet = Instantiate(bulletPrefab, ray.origin, Quaternion.identity);

                Rigidbody bulletRb = bullet.GetComponent<Rigidbody>();

                if (bulletRb != null)
                {
                    Vector3 bulletVelocity = ray.direction * bulletSpeed;
                    bulletRb.velocity = bulletVelocity;

                    // Set the rotation of the bullet to match its velocity
                    bullet.transform.rotation = Quaternion.LookRotation(bulletVelocity);

                    // Destroy the bullet after the specified lifetime
                    Destroy(bullet, bulletLifetime);
                }
                else
                {
                    Debug.LogError("Bullet prefab is missing Rigidbody component!");
                }
            }
            else
            {
                Debug.LogError("Player camera not found!");
            }

            // Update the last shot time
            lastShotTime = Time.time;
        }
    }


}
