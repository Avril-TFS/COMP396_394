using System;
using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.UI;

public class MonsterController : MonoBehaviour   //This is for small enemy movement
{
    public StateMachine stateMachine;
    public StateMachine.State idle, chase, attack;

    public GameObject enemy;    //Enemy means player here
    public float monsterFOV = 89; //degrees 
    private float cosMonsterFOVover2InRad;
    public float closeEnoughAttackCutoff = 2; //if distance of guard is less than or equal to 5 m close enou hto attack
    public float closeEnoughSenseCutoff = 15; //if (d(G,E) <=15m) - close enough to start chasing 

    public float strength = 5; //[0,100]
    public float health = 100;   //
    private float currentHealth;  
    public float speed = 2; //2 m/s

    Scoring scoring;
    public int killPoints = 200; 

    PlayerController playerController;
    float timer = 0;
    public bool attacking = false;
    public int damage = 50;
    
    public float attackRate = 4f; //attacks every 4 seconds

    //For HP bar - Slider is a child of small Enemy
    private Slider hpBarSlider;



    // Start is called before the first frame update
    void Start()
    {
        scoring = scoring = GameObject.Find("UI").GetComponent<Scoring>();
        playerController = GameObject.Find("Player").GetComponent<PlayerController>();


        cosMonsterFOVover2InRad = Mathf.Cos(monsterFOV / 2f * Mathf.Deg2Rad); // in Rad
        stateMachine = new StateMachine();
        
        idle = stateMachine.CreateState("Idle"); //var lets the compiler detect the type
        idle.onFrame = IdleOnFrame;

        chase = stateMachine.CreateState("Chase");
        chase.onFrame = ChaseOnFrame;

        attack = stateMachine.CreateState("Attack");
        attack.onFrame = AttackOnFrame;

        //For enemy's HP
        currentHealth = health;

        enemy = GameObject.Find("Player");


        //For HP bar for enemy-----
        Transform hpBarCanvas = transform.Find("HPBarCanvas");
        if (hpBarCanvas != null)
        {
            // Find the HPBarSlider within the HPBarCanvas
            hpBarSlider = hpBarCanvas.GetComponentInChildren<Slider>();
            hpBarCanvas.LookAt(enemy.transform);





            if (hpBarSlider == null)
            {
                Debug.LogError("HPBarSlider not found in HPBarCanvas.");
            }
        }
        else
        {
            Debug.LogError("HPBarCanvas not found.");
        }
    }

    // Update is called once per frame
    void Update()
    {
        stateMachine.Update();
        timer += Time.deltaTime;

        
        if (health <= 0)
        {
            Die(); 
        }
    }

    void OnCollisionEnter(Collision collision)
    {
        // Check if the collision is with a bullet
        if (collision.gameObject.CompareTag("Bullet"))
        {
           
            TakeDamage(damage);

            // Destroy the bullet on impact
            Destroy(collision.gameObject);
        }
    }

    private void TakeDamage(int damage)
    {
        currentHealth -= damage;
        scoring.score += damage;
        scoring.sendMessageToUI("Hit enemy");


        // Update HP bar value  
        if (hpBarSlider != null)
        {
            hpBarSlider.value = currentHealth / health;
        }

        // Check if the enemy's health has reached 0
        if (currentHealth <= 0)
        {
            Die();
        }
    }

    void IdleOnFrame()
    {
        Idle();

        if (SenseEnemy(this.transform.position, enemy.transform.position, this.transform.forward, cosMonsterFOVover2InRad, closeEnoughSenseCutoff))
        {
            stateMachine.ChangeState(chase);
        }

    }


    private void Idle()
    {
        //do nothing for now 
        Debug.Log("Samell Enemy is in Idle state");

    }

    void ChaseOnFrame()
    {
        Chase();

        //Check transition conditions
        if (WithinRange(this.transform.position, enemy.transform.position, closeEnoughAttackCutoff))
        {
            stateMachine.ChangeState(attack);
        }


    }
    private void Chase()
    {

        Vector3 enemyHeading = (enemy.transform.position - this.transform.position);
        float enemyDistance = enemyHeading.magnitude;
        enemyHeading.Normalize();

        Vector3 movement = enemyHeading * speed * Time.deltaTime; //m/s but we want m/frame so multiply by s/frame
        Vector3.ClampMagnitude(movement, enemyDistance);
        this.transform.position += movement;

    }

    void AttackOnFrame()
    {
        if(attacking == false)
        {
            Attack(); 
        }

        if (!WithinRange(this.transform.position, enemy.transform.position, closeEnoughAttackCutoff))
        {
            stateMachine.ChangeState(idle);
        }

    }
    private void Attack()
    {
        Debug.Log("Small enemy is close and enemy is attacking");
        //create timer so attack happens every x seconds 
        if (timer > attackRate)
        {
            playerController.Damage(damage);
            timer = 0;
        }

    }


    public static bool SenseEnemy(Vector3 start, Vector3 enemyPos, Vector3 thisForward, float cutOff, float distance)
    {

        Vector3 enemyHeading = (enemyPos - start).normalized;
        float cosAngle = Vector3.Dot(enemyHeading, thisForward);


        //Case1: Enemy in front and close enough 
        if (cosAngle > cutOff && Vector3.Distance(start, enemyPos) <= distance)
        {
            return true;
        }
        else
        {
            return false;
        }

    }
    public static bool WithinRange(Vector3 start, Vector3 enemyPos, float distance)
    {
        if (Vector3.Distance(start, enemyPos) <= distance)
        {
            return true;
        }
        else
        {
            return false;
        }
    }

    public void Die()
    {
        //scoring.AddScore(killPoints);
        Destroy(this.gameObject);
    }
}
