//Author : Yuko Yamano


using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;


public enum WeaponTypes{
    normal, better, good, best, GOAT
}
public class PlayerController : MonoBehaviour
{
    public float moveSpeed = 5.0f;
    public Transform playerBody; // Reference to the player model 
    private GameObject playerBodyObject;
    private Rigidbody rb;
    //for the bullet
    public Transform shootPoint;
    public GameObject bulletPrefab;
    //Bullet performance
    public float shootingInterval = 0.8f;
    public float bulletSpeed = 10f; // Set your default bullet speed here
    public float bulletLifetime = 2f; // Time in seconds before the bullet is destroyed
    private float lastShotTime;
   
    public WeaponTypes currentWeaponType = WeaponTypes.normal;
   
    

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

        //Activate this when debugging 
        //Cursor.lockState = CursorLockMode.None;
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
                    // Adjust properties based on the current weapon type
                    switch (currentWeaponType)
                    {
                        case WeaponTypes.normal:
                            bulletSpeed = 50f;
                            shootingInterval = 0.8f;
                            // Other properties for Normal weapon...
                            break;

                        case WeaponTypes.better:
                            bulletSpeed = 100f;
                            shootingInterval = 0.6f;
                            // Other properties for Better weapon...
                            break;

                        case WeaponTypes.good:
                            bulletSpeed =150f;
                            shootingInterval = 0.4f;
                            // Other properties for Good weapon...
                            break;

                        case WeaponTypes.best:
                            bulletSpeed = 20f;
                            shootingInterval = 0.2f;
                            // Other properties for Best weapon...
                            break;

                        case WeaponTypes.GOAT:
                            bulletSpeed = 250f;
                            shootingInterval = 0.1f;
                            // Other properties for GOAT weapon...
                            break;

                        // Add more cases for additional weapon types...

                        default:
                            break;
                    }

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

    void OnTriggerEnter(Collider col)
    {
        if (!col.gameObject == GameObject.FindGameObjectWithTag("Key1"))
        {
            Destroy(col.gameObject);
        }
        
        switch (col.gameObject.tag)
        {
            case "WeaponBetter":
                scoring.sendMessageToUI("Better weapon picked up! ");
                currentWeaponType = WeaponTypes.better;
                break;

            case "WeaponGood":
                scoring.sendMessageToUI("Good weapon picked up! ");
                currentWeaponType = WeaponTypes.good;
                break;

            case "WeaponBest":
                scoring.sendMessageToUI("Best weapon picked up! ");
                currentWeaponType = WeaponTypes.best;
                break;

            case "WeaponGOAT":
                scoring.sendMessageToUI("GOAT weapon picked up! ");
                currentWeaponType = WeaponTypes.GOAT;
                break;

            default:
                break;
        }
        Debug.Log("CurrentWeaponType : " + currentWeaponType);

        //if (col.gameObject.tag == "WeaponBetter")
        //{

        //    Destroy(col.gameObject);
        //    //TODO: play picking up weapon sound 
        //    scoring.sendMessageToUI("New weapon picked up! ");
        //    currentWeaponType = WeaponTypes.better;
        //    Debug.Log("CurrentWeaponType : " + currentWeaponType);
        //}
    }
}


