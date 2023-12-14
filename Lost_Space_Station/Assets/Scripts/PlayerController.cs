//Author : Yuko Yamano


using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.UI;

public enum WeaponTypes{
    WeaponNormal, WeaponBetter, WeaponGood, WeaponBest, WeaponGOAT
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
    public float bulletSize = 1.0f;

    public float fallSpeed = 4.0f;

    public WeaponTypes currentWeaponType = WeaponTypes.WeaponNormal;
    public GameObject WeaponNormal;
    public GameObject WeaponBetter;
    public GameObject WeaponGood;
    public GameObject WeaponBest;
    public GameObject WeaponGOAT;

    public ParticleSystem muzzleFlash;
    private Slider hpBarSliderForPlayer;

    // This is for keys---------------
    public bool hasKey = false;

    AudioManager am;
    AudioSource audioSource;

    
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
        am = GameObject.Find("AudioController").GetComponent<AudioManager>();
        audioSource = GameObject.Find("AudioController").GetComponent<AudioSource>();

        //WeaponNormal = GameObject.FindGameObjectWithTag("WeaponNormal");
        //WeaponBetter = GameObject.FindGameObjectWithTag("WeaponBetter");
        //WeaponGood = GameObject.FindGameObjectWithTag("WeaponGood");
        //WeaponBest = GameObject.FindGameObjectWithTag("WeaponBest");
        //WeaponGOAT = GameObject.FindGameObjectWithTag("WeaponGOAT");

        //ActivateWeapon(WeaponTypes.WeaponNormal);
        Debug.Log("current weapon type is : " + currentWeaponType);
        currentWeaponType = GameManager.instance.playerCurrentWeapon;
        
        ActivateWeapon(currentWeaponType);

        //Activate this line below for debugging if needed 
        //Cursor.lockState = CursorLockMode.None;
    }

    private void Update()
    {
        HealthBarForPlayer();
        // Player movement
        float horizontalInput = Input.GetAxis("Horizontal");
        float verticalInput = Input.GetAxis("Vertical");

        //Debug.Log("Current health: " + health);

        // Calculate the movement direction relative to the player's local space
        Vector3 movement = playerBody.TransformDirection(new Vector3(horizontalInput, 0, verticalInput)) * moveSpeed;
        rb.velocity = movement;

        if (health <= 0)
        {
            audioSource.Stop();
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

        if (!IsGrounded())
        {
            rb.velocity += Vector3.down * fallSpeed;
        }

    }

    bool IsGrounded()
    {
        return Physics.Raycast(transform.position, Vector3.down, 1.5f);
    }

    private void Die()
    {
       
        scoring.GameOver();
    }

    public void Damage(int amount)
    {
        health -= amount;

        if (amount > 0)
        {
            scoring.sendMessageToUI("Attacked by enemy");

        }
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

    public void Heal(int amount)
    {
        health += amount;

        health = Mathf.Clamp(health, 0, 100);

        if(hpBarSliderForPlayer != null)
        {
            float normalizedHealth = (float) health / 100f;
        
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

              //  float bulletSize = GetBulletSize(currentWeaponType);
                bullet.transform.localScale *= bulletSize;

                Rigidbody bulletRb = bullet.GetComponent<Rigidbody>();

                if (bulletRb != null)
                {
                    
                    // Adjust properties based on the current weapon type
                    switch (currentWeaponType)
                    {
// ---------- I added the damage values of the weapons to the monsters it was easier to do it this way due to how we have them coded to take damage
                        
                        case WeaponTypes.WeaponNormal:
                            bulletSpeed = 50f;
                            shootingInterval = 0.8f;
                            bulletSize = 1.0f;
                            am.PLAY_SOUND_ONCE(1);    //cann add these back if we add sound controller to the player, otherwise this causes issues to shotting
                            // Other properties for Normal weapon...
                            break;

                        case WeaponTypes.WeaponBetter:  // Rifle
                            bulletSpeed = 150f;
                            shootingInterval = 0.3f;
                            bulletSize = 1.0f;
                            am.PLAY_SOUND_ONCE(2);
                            // Other properties for Better weapon...
                            break;

                        case WeaponTypes.WeaponGood:    // Sniper
                            bulletSpeed = 2000f;
                            shootingInterval = 0.8f;
                            bulletSize = .5f;
                            am.PLAY_SOUND_ONCE(3);
                            // Other properties for Good weapon...
                            break;

                        case WeaponTypes.WeaponBest:    // Shotgun
                            bulletSpeed = 100f;
                            shootingInterval = 0.5f;
                            bulletSize = 5.0f;
                            //Download better sound for this if we implement this type of weapon
                            am.PLAY_SOUND_ONCE(1);
                            // Other properties for Best weapon...
                            break;

                        case WeaponTypes.WeaponGOAT:    // Stupid Op weapon i made lol
                            bulletSpeed = 250f;
                            shootingInterval = 0.1f;
                            bulletSize = 2.0f;
                            //Download better sound for this if we implement this type of weapon
                            am.PLAY_SOUND_ONCE(3);
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

                    muzzleFlash.Play();
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
       
        switch (col.gameObject.tag)
        {
            case "WeaponBetter":
                scoring.sendMessageToUI("Better weapon picked up! ");
                //currentWeaponType = WeaponTypes.WeaponBetter;
               // ActivateWeapon(WeaponTypes.WeaponBetter);
                SetPlayerWeapon(WeaponTypes.WeaponBetter);
                am.PLAY_SOUND_ONCE(5);
                Destroy(col.gameObject);
                break;

            case "WeaponGood":
                scoring.sendMessageToUI("Good weapon picked up! ");
             //   currentWeaponType = WeaponTypes.WeaponGood;
            //    ActivateWeapon(WeaponTypes.WeaponGood);
                SetPlayerWeapon(WeaponTypes.WeaponGood);
                am.PLAY_SOUND_ONCE(5);
                Destroy(col.gameObject);
                break;

            case "WeaponBest":
                scoring.sendMessageToUI("Best weapon picked up! ");
             //   currentWeaponType = WeaponTypes.WeaponBest;
             //   ActivateWeapon(WeaponTypes.WeaponBest);
                SetPlayerWeapon(WeaponTypes.WeaponBest);
                am.PLAY_SOUND_ONCE(5);
                Destroy(col.gameObject);
                break;

            case "WeaponGOAT":
                scoring.sendMessageToUI("GOAT weapon picked up! ");
              //  currentWeaponType = WeaponTypes.WeaponGOAT;
             //   ActivateWeapon(WeaponTypes.WeaponGOAT);
                SetPlayerWeapon(WeaponTypes.WeaponGOAT);
                am.PLAY_SOUND_ONCE(5);
                Destroy(col.gameObject);
                break;

            default:
                break;

        }
        //Debug.Log("CurrentWeaponType : " + currentWeaponType);

        //if (col.gameObject.tag == "WeaponBetter")
        //{

        //    Destroy(col.gameObject);
        //    //TODO: play picking up weapon sound 
        //    scoring.sendMessageToUI("New weapon picked up! ");
        //    currentWeaponType = WeaponTypes.better;
        //    Debug.Log("CurrentWeaponType : " + currentWeaponType);
        //}

        if (col.gameObject.tag == "EnemyThrowObject")
        {
            Damage(10); 
            Destroy(col.gameObject);

        }
    }

    void ActivateWeapon(WeaponTypes weaponType)
    {
        currentWeaponType = weaponType;
       // currentWeaponType = GameManager.instance.playerCurrentWeapon;

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
    }

    void SetPlayerWeapon(WeaponTypes newWeaponType)
    {
        GameManager.instance.playerCurrentWeapon = newWeaponType;
        ActivateWeapon(newWeaponType);
    }
   
}