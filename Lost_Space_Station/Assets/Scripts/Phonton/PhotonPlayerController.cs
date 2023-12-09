//Author : Yuko Yamano


using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.UI;
using Photon.Pun;

public class PhotonPlayerController : MonoBehaviour
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
    public WeaponTypes currentWeaponType = WeaponTypes.WeaponNormal;
    public GameObject WeaponNormal;
    public GameObject WeaponBetter;
    public GameObject WeaponGood;
    public GameObject WeaponBest;
    public GameObject WeaponGOAT;
    public ParticleSystem muzzleFlash;
    private Slider hpBarSliderForPlayer;
    public int health = 100;
    Scoring scoring;
    public int attackStrength = 20;
    private PhontonMenu phontonMenuScript;


    PhotonView view;
    // This is for keys---------------
    public bool hasKey = false;

    AudioManager am;

    public void HasKey()
    {
        hasKey = true;
    }
    public bool CheckHasKey()
    {
        return hasKey;
    }//-------------------------------

    




    private void Start()
    {
        rb = GetComponent<Rigidbody>();
        playerBody = transform.Find("PlayerBody");
        scoring = GameObject.Find("UI").GetComponent<Scoring>();
        playerBodyObject = GameObject.Find("PlayerBody");
        am = GameObject.Find("AudioController").GetComponent<AudioManager>();
        phontonMenuScript = GameObject.Find("MenuController").GetComponent<PhontonMenu>();
        //WeaponNormal = GameObject.FindGameObjectWithTag("WeaponNormal");
        //WeaponBetter = GameObject.FindGameObjectWithTag("WeaponBetter");
        //WeaponGood = GameObject.FindGameObjectWithTag("WeaponGood");
        //WeaponBest = GameObject.FindGameObjectWithTag("WeaponBest");
        //WeaponGOAT = GameObject.FindGameObjectWithTag("WeaponGOAT");

        ActivateWeapon(WeaponTypes.WeaponNormal);


        //No constrain on the coursor movement
        Cursor.lockState = CursorLockMode.None;

        view = GetComponent<PhotonView>();

    }

    private void Update()
    {
        if (view.IsMine)
        {
            HealthBarForPlayer();
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

            //Shooting debugging purpose only
            if (Input.GetMouseButtonDown(0))
            {
                //ShootBullet();
                Debug.Log("ShootBullet will be called");
            }
            if (Input.GetKeyDown(KeyCode.Escape))
            {
#if UNITY_EDITOR
                //ityEditor.EditorApplication.isPlaying = false;
                phontonMenuScript.pausePanel.SetActive(true);
                Time.timeScale = 0f;
#else
                phontonMenuScript.pausePanel.SetActive(true);
                Time.timeScale = 0f;
        
#endif
            }
        }
       
    }

    private void Die()
    {
        scoring.GameOver();
    }

    public void Damage(int amount)
    {
        health -= amount;
        scoring.sendMessageToUI("Attacked by enemy");

        // Update HP bar value  
        if (hpBarSliderForPlayer != null)
        {
            // Ensure that the health value stays within the range [0, 100]
            health = Mathf.Clamp(health, 0, 100);

            // Calculate the normalized value for the slider based on the health
            float normalizedHealth = (float)health / 100f;

            // Update the slider value
            hpBarSliderForPlayer.value = normalizedHealth;
        }
    }

    //HP bar for the Player
    void HealthBarForPlayer()
    {
        GameObject Canvas = GameObject.Find("Canvas");
        if (Canvas != null)
        {
            hpBarSliderForPlayer = Canvas.GetComponentInChildren<Slider>();
            //Debug.Log("hpBarSliderForPlayer assigned: " + (hpBarSliderForPlayer != null));
        }
    }



    //void ShootBullet()
    //{


    //    // Check if enough time has passed since the last shot
    //    if (Time.time - lastShotTime >= shootingInterval)
    //    {
    //        // Find the camera component on the player
    //        Camera playerCamera = playerBodyObject.GetComponentInChildren<Camera>();
    //        if (playerCamera != null)
    //        {
    //            // Get the center of the screen in viewport coordinates
    //            Vector3 screenCenter = new Vector3(0.5f, 0.5f, playerCamera.nearClipPlane);

    //            // Convert the screen center to a ray from the camera
    //            Ray ray = playerCamera.ViewportPointToRay(screenCenter);

    //            // Instantiate the bullet at the center of the screen
    //            GameObject bullet = Instantiate(bulletPrefab, ray.origin, Quaternion.identity);

    //            Rigidbody bulletRb = bullet.GetComponent<Rigidbody>();

    //            if (bulletRb != null)
    //            {
    //                // Adjust properties based on the current weapon type
    //                switch (currentWeaponType)
    //                {
    //                    case WeaponTypes.WeaponNormal:
    //                        bulletSpeed = 50f;
    //                        shootingInterval = 0.8f;
    //                        am.PLAY_SOUND_ONCE(1);
    //                        // Other properties for Normal weapon...
    //                        break;

    //                    case WeaponTypes.WeaponBetter:
    //                        bulletSpeed = 100f;
    //                        shootingInterval = 0.6f;
    //                        am.PLAY_SOUND_ONCE(2);
    //                        // Other properties for Better weapon...
    //                        break;

    //                    case WeaponTypes.WeaponGood:
    //                        bulletSpeed = 150f;
    //                        shootingInterval = 0.4f;
    //                        am.PLAY_SOUND_ONCE(3);
    //                        // Other properties for Good weapon...
    //                        break;

    //                    case WeaponTypes.WeaponBest:
    //                        bulletSpeed = 20f;
    //                        shootingInterval = 0.2f;
    //                        //Download better sound for this if we implement this type of weapon
    //                        am.PLAY_SOUND_ONCE(1);
    //                        // Other properties for Best weapon...
    //                        break;

    //                    case WeaponTypes.WeaponGOAT:
    //                        bulletSpeed = 250f;
    //                        shootingInterval = 0.1f;
    //                        //Download better sound for this if we implement this type of weapon
    //                        am.PLAY_SOUND_ONCE(1);
    //                        // Other properties for GOAT weapon...
    //                        break;

    //                    // Add more cases for additional weapon types...

    //                    default:
    //                        break;
    //                }

    //                Vector3 bulletVelocity = ray.direction * bulletSpeed;
    //                bulletRb.velocity = bulletVelocity;
    //                muzzleFlash.Play();


    //                // Set the rotation of the bullet to match its velocity
    //                bullet.transform.rotation = Quaternion.LookRotation(bulletVelocity);

    //                // Destroy the bullet after the specified lifetime
    //                Destroy(bullet, bulletLifetime);
    //            }
    //            else
    //            {
    //                Debug.LogError("Bullet prefab is missing Rigidbody component!");
    //            }
    //        }
    //        else
    //        {
    //            Debug.LogError("Player camera not found!");
    //        }

    //        // Update the last shot time
    //        lastShotTime = Time.time;
    //    }

    //}

    void OnTriggerEnter(Collider col)
    {

        switch (col.gameObject.tag)
        {
            case "WeaponBetter":
                scoring.sendMessageToUI("Better weapon picked up! ");
                currentWeaponType = WeaponTypes.WeaponBetter;
                ActivateWeapon(WeaponTypes.WeaponBetter);
                Destroy(col.gameObject);
                break;

            case "WeaponGood":
                scoring.sendMessageToUI("Good weapon picked up! ");
                currentWeaponType = WeaponTypes.WeaponGood;
                ActivateWeapon(WeaponTypes.WeaponGood);
                Destroy(col.gameObject);
                break;

            case "WeaponBest":
                scoring.sendMessageToUI("Best weapon picked up! ");
                currentWeaponType = WeaponTypes.WeaponBest;
                ActivateWeapon(WeaponTypes.WeaponBest);
                Destroy(col.gameObject);
                break;

            case "WeaponGOAT":
                scoring.sendMessageToUI("GOAT weapon picked up! ");
                currentWeaponType = WeaponTypes.WeaponGOAT;
                ActivateWeapon(WeaponTypes.WeaponGOAT);
                Destroy(col.gameObject);
                break;

            default:
                break;
        }
        //Debug.Log("CurrentWeaponType : " + currentWeaponType);

        //if (col.gameObject.tag == "WeaponBetter")
        //{

        //    Destroy(col.gameObject);
        //TODO: play picking up weapon sound
        //    scoring.sendMessageToUI("New weapon picked up! ");
        //    currentWeaponType = WeaponTypes.better;
        //    Debug.Log("CurrentWeaponType : " + currentWeaponType);
        //}
    }

    void ActivateWeapon(WeaponTypes weaponType)
    {
        currentWeaponType = weaponType;

        GameObject[] weaponObjects ={
            WeaponNormal,
            WeaponGood,
            WeaponBetter,
            WeaponBest,
            WeaponGOAT
        };



        foreach (GameObject weaponObject in weaponObjects)
        {
            if (weaponObject.tag != currentWeaponType.ToString())
            {
                weaponObject.SetActive(false);
            }
            else
            {
                weaponObject.SetActive(true);
            }

            //Debug.Log(weaponObject.ToString()) ;
        }

        // Activate the selected weapon object
        GameObject selectedWeaponObject = GameObject.FindGameObjectWithTag(weaponType.ToString());
        //Debug.Log("selectedWeaponObject is: "+selectedWeaponObject.ToString());  //rifle

        //switch (selectedWeaponObject.ToString())
        //{
        //    case "rifle":
        //        WeaponBetter.SetActive(true);
        //        break;
        //    //case "game object's name for weapontGood":
        //    //    WeaponGood.SetActive(true);
        //    //    break;
        //    //case "game object's name for weapontBest":
        //    //    WeaponBest.SetActive(true);
        //    //    break;
        //    //case "game object's name for weapontGOAT":
        //    //    WeaponGOAT.SetActive(true);
        //    //    break;

        //    default:
        //        break;
        //}

    }
    }


