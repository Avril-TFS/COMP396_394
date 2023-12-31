using System;
using System.Collections;
using System.Collections.Generic;
//using Unity.VisualScripting;
using UnityEngine.XR;
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
    public float closeEnoughSenseCutoff = 10; //if (d(G,E) <=15m) - close enough to start chasing 
    public float strength = 5; //[0,100]
    public float health = 100;   //
    private float currentHealth;
    public float speed = 2; //2 m/s
    Scoring scoring;
    public int killPoints = 200;
    PlayerController playerController;
    float timer = 0;
    public bool isAttacking = false;
    public int damage = 10;
    public float attackRate = 4f; //attacks every 4 seconds
    //For HP bar - Slider is a child of small Enemy
    private Slider hpBarSlider;

    // Rotation speed for Enemy
    public float rotationSpeed = 60.0f;
    public GameObject QuestionMark;

    UnityEngine.AI.NavMeshHit hit;
    private UnityEngine.AI.NavMeshAgent navMeshAgent;

    AudioManager am;

    // Start is called before the first frame update
    void Start()
    {
        scoring = scoring = GameObject.Find("UI").GetComponent<Scoring>();
        playerController = GameObject.Find("Player").GetComponent<PlayerController>();
        //Error handling
        if (!enemy)
        {
            enemy = GameObject.FindGameObjectWithTag("Player");
        }

        //For enemy's HP
        currentHealth = health;

        HpBarLogic();

        //FSM logic**********************

        cosMonsterFOVover2InRad = Mathf.Cos(monsterFOV / 2f * Mathf.Deg2Rad); // in Rad
        stateMachine = new StateMachine();

        idle = stateMachine.CreateState("Idle"); //var lets the compiler detect the type
        //idle.onEnter = delegate { Debug.Log("idle.onEnter"); };
        //idle.onExit = delegate { Debug.Log("idle.onExit"); };
        idle.onFrame = IdleOnFrame;

        chase = stateMachine.CreateState("Chase");
        //chase.onEnter = delegate { Debug.Log("chase.onEnter"); };
        //chase.onExit = delegate { Debug.Log("chase.onExit"); };
        chase.onFrame = ChaseOnFrame;

        attack = stateMachine.CreateState("Attack");
        //attack.onEnter = delegate { Debug.Log("attack.onEnter"); };
        //attack.onExit = delegate { Debug.Log("attack.onExit"); };
        attack.onFrame = AttackOnFrame;

        QuestionMark.SetActive(false);
        //FSM Logie end************************

        navMeshAgent = GetComponent<UnityEngine.AI.NavMeshAgent>();

        am = GameObject.Find("AudioController").GetComponent<AudioManager>();

    }

    // Update is called once per frame
    void Update()
    {
        stateMachine.Update();

        //Debug.Log("current state is "+stateMachine.currentState);

        timer += Time.deltaTime;




        if (health <= 0)
        {
            Die();
        }

    }

    void RotateEnemy360()
    {
        // Rotate the enemy around its up vector (Y-axis) by rotationSpeed degrees per second
        transform.Rotate(Vector3.up, rotationSpeed * Time.deltaTime);
    }




    //FSM ***************************************************************************
    void IdleOnFrame()
    {

        Idle();

        //Idle -> chase (T1)
        if (SenseEnemy(this.transform.position, enemy.transform.position, this.transform.forward, cosMonsterFOVover2InRad, closeEnoughSenseCutoff))
        {
            //Debug.Log("Transitioning from Idle to Chase");
            stateMachine.ChangeState(chase);
        }

    }


    private void Idle()
    {
        QuestionMark.SetActive(true);
        if (stateMachine.currentState == idle)
        {
            RotateEnemy360();
        }
    }

    void ChaseOnFrame()
    {
        //Debug.Log("Chase.onFrame");
        Chase();
        QuestionMark.SetActive(false);

        // Look at the player
        transform.LookAt(enemy.transform);

        //Transition to chase -> attack (T2)
        if (WithinRange(this.transform.position, enemy.transform.position, closeEnoughAttackCutoff))
        {
            //Debug.Log("Transitioning from Chase to Attack");
            stateMachine.ChangeState(attack);
        }

        //Transition to chase ->idle (T1)
        if (!WithinRange(this.transform.position, enemy.transform.position, closeEnoughSenseCutoff))
        {
            //Debug.Log("Transitioning from Chase to Idle");
            stateMachine.ChangeState(idle);
        }


    }
    private void Chase()
    {
        Transform enemyTransform = enemy.transform;

        if (UnityEngine.AI.NavMesh.SamplePosition(enemyTransform.position, out hit, 2.0f, UnityEngine.AI.NavMesh.AllAreas))
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
        }


        /*
        Vector3 enemyHeading = (enemy.transform.position - this.transform.position);
        float enemyDistance = enemyHeading.magnitude;
        enemyHeading.Normalize();

        Vector3 movement = enemyHeading * speed * Time.deltaTime; //m/s but we want m/frame so multiply by s/frame
        Vector3.ClampMagnitude(movement, enemyDistance);
        this.transform.position += movement;
        */


    }

    void AttackOnFrame()
    {

        if (isAttacking == false)
        {
            // Look at the player when attacking
            transform.LookAt(enemy.transform);
            Attack();
        }



        //Attack-> chase(T3)  if the player is not within attack range
        if (isAttacking == true && !SenseEnemy(this.transform.position, enemy.transform.position, this.transform.forward, cosMonsterFOVover2InRad, closeEnoughSenseCutoff))
        {
            isAttacking = false;
            stateMachine.ChangeState(chase);
            //Debug.Log("No longer in attack range, start chasing again");

        }
        //Attack-> chase(T3)  if the player is not within sense range
        if (isAttacking == true && !SenseEnemy(this.transform.position, enemy.transform.position, this.transform.forward, cosMonsterFOVover2InRad, closeEnoughAttackCutoff))
        {
            isAttacking = false;
            stateMachine.ChangeState(chase);
            //Debug.Log("No longer in attack range, start chasing again");

        }



    }
    private void Attack()
    {
        isAttacking = true;



        //create timer so attack happens every x seconds 
        //if (timer > attackRate)
        //{
        //    playerController.Damage(damage);
        //    timer = 0;
        //}
        am.PLAY_SOUND_ONCE(8);
        playerController.Damage(damage);
        Debug.Log("Took " + damage +" damage From Monster" );

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

    //To display HPBar on top of the small enemy
    void HpBarLogic()
    {
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

    void OnCollisionEnter(Collision collision)
    {
        // Check if the collision is with a bullet
        if (collision.gameObject.CompareTag("Bullet"))
        {
            int playerDamage = GetPlayerDamageFromBullet(collision.gameObject);
            //Debug.Log("Bullet hit");
            TakeDamage(playerDamage);

            // Destroy the bullet on impact
            Destroy(collision.gameObject);
        }
    }

    private void TakeDamage(int playerDamage)
    {
        currentHealth -= playerDamage;
        scoring.score += playerDamage;
        scoring.sendMessageToUI("Hit enemy");
        stateMachine.ChangeState(chase);

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
}