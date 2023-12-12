using System;
using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;
using static UnityEngine.EventSystems.EventTrigger;

public class ProwlerController : MonoBehaviour
{
    public Transform[] waypoints;
    public int currentWaypoint = 0;

    public Transform player;
    public float chaseDistance = 15.0f;
    public float speed = 3f;
    public float FOV = 90f;
    public float rotationSpeed = 1000f;
    Scoring scoring;
    //For HP bar - Slider is a child of Robot
    private Slider hpBarSlider;

    //for level3 attacking/damage
    [Header("Level 3 variables")]
    Scene currentScene;
    PlayerController playerController;
    public int damage = 20;
    private float currentHealth;
    public float health = 1000;
    bool enraged = false;
    public GameObject key;
    public GameObject explosion;

    [Header("Throw Object variables")]
    public GameObject throwObject;
    public float throwDistance = 10.0f;
    public int totalThrows = 100;
    public float throwCooldown = 0.2f;
    bool canThrow;
    public float throwForce = 70;
    public Transform attackPoint;


    private enum ProwlerState
    {
        Patrol, Chase, Attack, ThrowObject
    }
    private ProwlerState state;

    // Start is called before the first frame update
    void Start()
    {
        state = ProwlerState.Patrol;
        scoring = GameObject.Find("UI").GetComponent<Scoring>();
        // For HP bar for Robot---- - TODO: implement the HP bar for boss fight. For now, this robot can't be beaten.
        Transform hpBarCanvas = transform.Find("HPBarCanvas");
        if (hpBarCanvas != null)
        {
            // Find the HPBarSlider within the HPBarCanvas
            hpBarSlider = hpBarCanvas.GetComponentInChildren<Slider>();
            hpBarCanvas.LookAt(player.transform);

            if (hpBarSlider == null)
            {
                Debug.LogError("HPBarSlider not found in HPBarCanvas.");
            }
        }
        else
        {
            Debug.LogError("HPBarCanvas not found.");
        }

        currentScene = SceneManager.GetActiveScene();
        playerController = GameObject.Find("Player").GetComponent<PlayerController>();
        currentHealth = health;
        canThrow = true; 
    }

    // Update is called once per frame
    void Update()
    {
        switch (state)
        {
            case ProwlerState.Patrol:
                Patrol();
                break;

            case ProwlerState.Chase:
                Chase();
                break;

            case ProwlerState.ThrowObject:
                ThrowObject(); 
                break;
        }

    }

    private void ThrowObject()
    {
        canThrow = false;
        if (Vector3.Distance(transform.position, player.position) > throwDistance || totalThrows <=0)
        {
            state = ProwlerState.Chase;
            return;
        }
        canThrow = false;
        GameObject projectile = Instantiate(throwObject, attackPoint.transform.position, attackPoint.transform.rotation);
        Rigidbody projectileRb = projectile.GetComponent<Rigidbody>();
        Vector3 forcefulness = projectileRb.transform.forward * throwForce;

        projectileRb.AddForce(forcefulness);
        totalThrows--;

        Destroy(projectile, 1);
        state = ProwlerState.Chase;

        Invoke(nameof(ResetThrow), throwCooldown);
    }

    private void ResetThrow()
    {
        canThrow = true;
    }

    void Patrol()
    {
        if (Vector3.Distance(transform.position, player.position) < chaseDistance && CanSee())
        {
            state = ProwlerState.Chase;
            return;
        }

        UnityEngine.AI.NavMeshHit hit;
        if (UnityEngine.AI.NavMesh.SamplePosition(waypoints[currentWaypoint].position, out hit, 2.0f, UnityEngine.AI.NavMesh.AllAreas))
        {
            Vector3 targetPosition = hit.position;
            UnityEngine.AI.NavMeshPath path = new UnityEngine.AI.NavMeshPath();


            if (UnityEngine.AI.NavMesh.CalculatePath(transform.position, targetPosition, UnityEngine.AI.NavMesh.AllAreas, path))
            {

                if (path.corners.Length > 1)
                {

                    Vector3 nextCorner = path.corners[1];
                    transform.position = Vector3.MoveTowards(transform.position, nextCorner, speed * Time.deltaTime);

                    Vector3 direction = (nextCorner - transform.position).normalized;
                    Quaternion targetRotation = Quaternion.LookRotation(new Vector3(direction.x, 0, direction.z));
                    transform.rotation = Quaternion.RotateTowards(transform.rotation, targetRotation, rotationSpeed * Time.deltaTime);

                    if (Vector3.Distance(transform.position, waypoints[currentWaypoint].position) < 0.2f)
                    {
                        currentWaypoint = (currentWaypoint + 1) % waypoints.Length;
                    }
                }
            }

            /*
            transform.position = Vector3.MoveTowards(transform.position, waypoints[currentWaypoint].position, speed * Time.deltaTime);
            transform.LookAt(waypoints[currentWaypoint].position);

            if (Vector3.Distance(transform.position, waypoints[currentWaypoint].position) < 0.2f)
            {
                currentWaypoint = (currentWaypoint + 1) % waypoints.Length;
            }
            */
        }
    }
    void Chase()
    {
        if (Vector3.Distance(transform.position, player.position) > chaseDistance && !CanSee())
        {
            state = ProwlerState.Patrol;
        }
        if (Vector3.Distance(transform.position, player.position) < throwDistance && CanSee() && canThrow && totalThrows > 0)
        {
            state = ProwlerState.ThrowObject;
            return;
        }

        UnityEngine.AI.NavMeshHit hit;
            if (UnityEngine.AI.NavMesh.SamplePosition(player.position, out hit, 2.0f, UnityEngine.AI.NavMesh.AllAreas))
            {
                Vector3 targetPosition = hit.position;
                UnityEngine.AI.NavMeshPath path = new UnityEngine.AI.NavMeshPath();

                if (UnityEngine.AI.NavMesh.CalculatePath(transform.position, targetPosition, UnityEngine.AI.NavMesh.AllAreas, path))
                {
                    if (path.corners.Length > 1)
                    {
                        Vector3 nextCorner = path.corners[1];
                        transform.position = Vector3.MoveTowards(transform.position, nextCorner, speed * Time.deltaTime);

                        Vector3 direction = (nextCorner - transform.position).normalized;
                        Quaternion targetRotation = Quaternion.LookRotation(new Vector3(direction.x, 0, direction.z));
                        transform.rotation = Quaternion.RotateTowards(transform.rotation, targetRotation, rotationSpeed * Time.deltaTime);

                    }
                }

                /*
            Vector3 targetPosition = player.position;
            targetPosition.y = transform.position.y;
            transform.position = Vector3.MoveTowards(transform.position, targetPosition, speed * Time.deltaTime);

            Vector3 direction = (player.position - transform.position).normalized;
            Quaternion targetRotation = Quaternion.LookRotation(new Vector3(direction.x, 0, direction.z));
            transform.rotation = Quaternion.RotateTowards(transform.rotation, targetRotation, rotationSpeed * Time.deltaTime);
                */
            }
    }

    private bool CanSee()
    {
        Vector3 directionToPlayer = player.position - transform.position;
        float angle = Vector3.Angle(transform.forward, directionToPlayer);

        return angle < FOV * 0.5f;
    }

    void OnTriggerEnter(Collider col)
    {

        if (currentScene.name == "LevelThree")
        {
            if (col.gameObject.tag == "Player")
            {
                Attack();
            }

            if (col.gameObject.tag == "Bullet")
            {
                TakeDamage(playerController.attackStrength);

                // Destroy the bullet on impact
                Destroy(col.gameObject);
            }
        }
        else
        {
            if (col.gameObject.tag == "Player")
            {
                SceneManager.LoadScene("GameOverScene");
            }

            if (col.gameObject.tag == "Bullet")
            {
                scoring.sendMessageToUI("This robot is unbeatable at this level.");
            }
        }

    }

    void Attack()
    {
        //Debug.Log("Attacking, damage: " + damage);

        if (enraged)
        {
            //attack damage increse, speed increase, throw objects?
            scoring.sendMessageToUI("The robot is enranged. Damage and speed increased!");
            playerController.Damage(damage * 2);
            speed = 6f;
            //Debug.Log("PlayerControll Damage called: " + damage);
        }
        else
        {
            playerController.Damage(damage);
            //Debug.Log("PlayerControll Damage called: " + damage);

        }
    }

    /*void TakeDamage(int damage)
    {
        currentHealth -= damage;
        scoring.score += damage;
        scoring.sendMessageToUI("Hit enemy");

        // Update HP bar value  
        if (hpBarSlider != null)
        {
            hpBarSlider.value = currentHealth / health;
        }

        if (currentHealth <= health * .5)
        {
            enraged = true;

        }
        // Check if the enemy's health has reached 0
        if (currentHealth <= 0)
        {
            Die();
        }

    }*/

    private void TakeDamage(int playerDamage)
    {
        currentHealth -= playerDamage;
        scoring.score += playerDamage;
        scoring.sendMessageToUI("Hit enemy");
        state = ProwlerState.Chase;

        // Update HP bar value  
        if (hpBarSlider != null)
        {
            hpBarSlider.value = currentHealth / health;
        }
        if (currentHealth <= health * .5)
        {
            enraged = true;

        }
        // Check if the enemy's health has reached 0
        if (currentHealth <= 0)
        {
            Die();
        }
    }

    int GetPlayerDamageFromBullet(GameObject bullet)
    {
        int playerDamage;

        WeaponTypes playerWeaponType = playerController.currentWeaponType;

        switch (playerWeaponType)
        {
            case WeaponTypes.WeaponNormal:
                playerDamage = 20;
                break;

            case WeaponTypes.WeaponBetter:
                playerDamage = 20;
                break;

            case WeaponTypes.WeaponGood:
                playerDamage = 75;
                break;

            case WeaponTypes.WeaponBest:
                playerDamage = 50;
                break;

            case WeaponTypes.WeaponGOAT:
                playerDamage = 100;
                break;

            default:
                playerDamage = 20;
                break;
        }

        return playerDamage;
    }

    void Die()
    {
        Debug.Log("Die called.");
        //scoring.AddScore(killPoints);
        GameObject newKey = Instantiate(key, transform.position, Quaternion.identity);
        newKey.transform.localScale = new Vector3(0.2f, 0.2f, 0.2f);
        GameObject newExplosion = Instantiate(explosion, transform.position, Quaternion.identity);
        newExplosion.transform.localScale = new Vector3(1, 1, 1);
        Destroy(this.gameObject);
    }
}