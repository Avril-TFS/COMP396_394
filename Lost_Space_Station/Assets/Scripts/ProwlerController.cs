using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class ProwlerController : MonoBehaviour
{
    public Transform[] waypoints;
    public int currentWaypoint = 0;

    public Transform player;
    public float chaseDistance = 10.0f;
    public float speed = 3f;
    public float FOV = 80f;
    public float rotationSpeed = 1000f;

    private enum ProwlerState
    {
        Patrol, Chase
    }
    private ProwlerState state;

    // Start is called before the first frame update
    void Start()
    {
        state = ProwlerState.Patrol;
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
        }
    }

    void Patrol()
    {
        if(Vector3.Distance(transform.position, player.position) < chaseDistance && CanSee())
        {
            state = ProwlerState.Chase;
            return;
        }

        transform.position = Vector3.MoveTowards(transform.position, waypoints[currentWaypoint].position, speed * Time.deltaTime);
        transform.LookAt(waypoints[currentWaypoint].position);

        if(Vector3.Distance(transform.position, waypoints[currentWaypoint].position) < 0.2f)
        {
            currentWaypoint = (currentWaypoint  + 1) % waypoints.Length;
        }
    }

    void Chase()
    {
        if (Vector3.Distance(transform.position, player.position) > chaseDistance && !CanSee())
        {
            state = ProwlerState.Patrol;
        }

        Vector3 targetPosition = player.position;
        targetPosition.y = transform.position.y;
        transform.position = Vector3.MoveTowards(transform.position, targetPosition, speed * Time.deltaTime);

        Vector3 direction = (player.position - transform.position).normalized;
        Quaternion targetRotation = Quaternion.LookRotation(new Vector3(direction.x, 0, direction.z));
        transform.rotation = Quaternion.RotateTowards(transform.rotation, targetRotation, rotationSpeed * Time.deltaTime);
    }


    bool CanSee()
    {
        Vector3 directionToPlayer = player.position - transform.position;
        float angle = Vector3.Angle(transform.forward, directionToPlayer);

        return angle < FOV * 0.5f;
    }

    private void OnTriggerEnter(Collider col)
    {
        if(col.gameObject.tag == "Player")
        {
            SceneManager.LoadScene("GameOverScene");
        }
    }
}
