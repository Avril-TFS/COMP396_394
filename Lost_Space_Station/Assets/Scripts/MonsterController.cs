using System;
using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;

public class MonsterController : MonoBehaviour
{
    public StateMachine stateMachine;
    public StateMachine.State idle, chase, attack;

    public GameObject enemy;
    public float monsterFOV = 89; //degrees 
    private float cosMonsterFOVover2InRad;
    public float closeEnoughAttackCutoff = 2; //if distance of guard is less than or equal to 5 m close enou hto attack
    public float closeEnoughSenseCutoff = 15; //if (d(G,E) <=15m) - close enough to start chasing 

    public float strength = 5; //[0,100]
    public float health = 100;
    public float speed = 2; //2 m/s

    Scoring scoring;
    public int killPoints = 200; 

    PlayerController playerController;
    float timer = 0;
    public bool attacking = false;
    public int damage = 10;
    public float attackRate = 4f; //attacks every 4 seconds


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
        Debug.Log("Samell Enemy is Idling");

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
        scoring.AddScore(killPoints);
        Destroy(this.gameObject);
    }
}
