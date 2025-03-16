using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class EnemyMultiPatrol : MonoBehaviour
{
    public Transform[] waypoints;  // Assign patrol points in the Inspector
    public Transform player;  // Assign the player GameObject
    public float patrolSpeed = 3.5f;  // Normal patrol speed
    public float chaseSpeed = 6.0f;  // Speed when chasing the player
    public float detectionRange = 10f;  // How far the enemy can see the player
    public float fieldOfView = 120f;  // Enemy's vision angle
    public float waitTime = 2.0f;  // Time to wait at waypoints
    public float lostPlayerTime = 3.0f;  // Time before forgetting player if lost

    private UnityEngine.AI.NavMeshAgent agent;
    private int currentWaypointIndex = 0;
    private int direction = 1;  // 1 = forward, -1 = backward
    private bool isWaiting = false;
    private bool isChasing = false;
    private float lastSeenTime;

    void Start()
    {
        agent = GetComponent<UnityEngine.AI.NavMeshAgent>();
        agent.speed = patrolSpeed;
        if (waypoints.Length > 0)
        {
            agent.SetDestination(waypoints[currentWaypointIndex].position);
        }
    }

    void Update()
    {
        if (isChasing)
        {
            ChasePlayer();
        }
        else
        {
            Patrol();
        }

        DetectPlayer();
    }

    void Patrol()
    {
        if (waypoints.Length == 0 || isWaiting) return;

        if (!agent.pathPending && agent.remainingDistance < 0.2f)
        {
            StartCoroutine(WaitAtWaypoint());
        }
    }

    IEnumerator WaitAtWaypoint()
    {
        isWaiting = true;
        yield return new WaitForSeconds(waitTime);
        GetNextWaypoint();
        isWaiting = false;
    }

    void GetNextWaypoint()
    {
        if (currentWaypointIndex == waypoints.Length - 1)
            direction = -1;
        else if (currentWaypointIndex == 0)
            direction = 1;

        currentWaypointIndex += direction;
        agent.SetDestination(waypoints[currentWaypointIndex].position);
    }

    void DetectPlayer()
    {
        if (player == null) return;

        Vector3 directionToPlayer = player.position - transform.position;
        float distanceToPlayer = directionToPlayer.magnitude;
        float angleToPlayer = Vector3.Angle(transform.forward, directionToPlayer);

        if (distanceToPlayer <= detectionRange && angleToPlayer <= fieldOfView / 2)
        {
            if (Physics.Linecast(transform.position, player.position, out RaycastHit hit))
            {
                if (hit.collider.gameObject == player.gameObject)
                {
                    isChasing = true;
                    lastSeenTime = Time.time;
                    agent.speed = chaseSpeed;
                    Debug.Log("Player detected, starting chase!");  // Debugging player detection
                }
            }
        }
        else if (isChasing && Time.time - lastSeenTime > lostPlayerTime)
        {
            isChasing = false;
            agent.speed = patrolSpeed;
            GetNextWaypoint();
        }
    }

    void ChasePlayer()
    {
        if (player != null)
        {
            agent.SetDestination(player.position);
        }
    }

    // Visualize the detection range and field of view in the editor
    void OnDrawGizmos()
    {
        // Visualize the detection range (circle)
        Gizmos.color = Color.red;
        Gizmos.DrawWireSphere(transform.position, detectionRange);

        // Visualize the field of view (cone)
        Gizmos.color = Color.green;
        Vector3 leftAngle = Quaternion.Euler(0, -fieldOfView / 2, 0) * transform.forward * detectionRange;
        Vector3 rightAngle = Quaternion.Euler(0, fieldOfView / 2, 0) * transform.forward * detectionRange;

        Gizmos.DrawLine(transform.position, transform.position + leftAngle);
        Gizmos.DrawLine(transform.position, transform.position + rightAngle);

        // Draw a line between the player and the enemy (for debugging)
        if (player != null)
        {
            Gizmos.color = Color.blue;
            Gizmos.DrawLine(transform.position, player.position);
        }
    }

    // Detect trigger collision with the player and restart the scene
    void OnTriggerEnter(Collider other)
    {
        Debug.Log("Trigger detected: " + other.gameObject.name);  // Debugging trigger detection

        if (isChasing)
        {
            if (other.gameObject.CompareTag("Player"))
            {
                Debug.Log("Trigger detected with player, restarting scene.");
                RestartScene();
            }
            else
            {
                Debug.Log("Trigger detected, but not with player.");
            }
        }
    }

    // Restart the scene
    void RestartScene()
    {
        // Get the current scene's name and reload it
        string currentScene = SceneManager.GetActiveScene().name;
        Debug.Log("Restarting scene: " + currentScene);  // Debugging scene reload
        SceneManager.LoadScene(currentScene);
    }
}
