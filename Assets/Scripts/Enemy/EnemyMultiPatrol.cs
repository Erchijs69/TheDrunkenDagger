using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.AI;

public class EnemyMultiPatrol : BaseEnemy
{
    public Transform[] waypoints;
    public float patrolSpeed = 3.5f;
    public float chaseSpeed = 6.0f;
    public float waitTime = 2.0f;
    public float lostPlayerTime = 3.0f;

    private NavMeshAgent agent;
    private int currentWaypointIndex = 0;
    private int direction = 1;
    private bool isWaiting = false;
    private bool isChasing = false;

    private Vector3 startingPosition;
    private Quaternion startingRotation;

    protected override void Start()
    {
        base.Start();
        agent = GetComponent<NavMeshAgent>();
        agent.speed = patrolSpeed;
        startingPosition = transform.position;
        startingRotation = transform.rotation;
        if (waypoints.Length > 0)
            agent.SetDestination(waypoints[currentWaypointIndex].position);
    }

    protected override void Update()
    {
        base.Update();

        if (isChasing)
            agent.SetDestination(player.position);
        else
            Patrol();
    }

    protected override void OnPlayerDetected()
    {
        isChasing = true;
        agent.speed = chaseSpeed;
        lastSeenTime = Time.time;
    }

    protected override void OnPlayerLost()
    {
        if (isChasing && Time.time - lastSeenTime > lostPlayerTime)
        {
            isChasing = false;
            agent.speed = patrolSpeed;
            GoToNextWaypoint();
        }
    }

    void Patrol()
    {
        if (waypoints.Length == 0 || isWaiting || agent.pathPending || agent.remainingDistance >= 0.2f) return;

        StartCoroutine(WaitAtWaypoint());
    }

    IEnumerator WaitAtWaypoint()
    {
        isWaiting = true;
        yield return new WaitForSeconds(waitTime);
        GoToNextWaypoint();
        isWaiting = false;
    }

    void GoToNextWaypoint()
    {
        if (currentWaypointIndex == waypoints.Length - 1) direction = -1;
        else if (currentWaypointIndex == 0) direction = 1;

        currentWaypointIndex += direction;
        agent.SetDestination(waypoints[currentWaypointIndex].position);
    }

    void OnTriggerEnter(Collider other)
    {
        if (isChasing)
        {
            CheckPlayerDeath(other);
        }
    }

public void ResetEnemy()
{
    StopAllCoroutines();
    isChasing = false;
    isWaiting = false;
    currentWaypointIndex = 0;
    direction = 1;

    transform.position = startingPosition;
    transform.rotation = startingRotation;

    agent.ResetPath();
    agent.speed = patrolSpeed;

    if (waypoints.Length > 0)
        agent.SetDestination(waypoints[currentWaypointIndex].position);
}
}

