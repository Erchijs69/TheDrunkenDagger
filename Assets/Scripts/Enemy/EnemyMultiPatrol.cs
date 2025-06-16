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

    private Animator animator;
    private bool isDetecting = false;


    protected override void Start()
{
    base.Start();
    agent = GetComponent<NavMeshAgent>();
    animator = GetComponentInChildren<Animator>();

    agent.speed = patrolSpeed;
    startingPosition = transform.position;
    startingRotation = transform.rotation;

    if (waypoints.Length > 0)
        agent.SetDestination(waypoints[currentWaypointIndex].position);

    animator.SetBool("IsRunning", false); // start with walk
    Debug.Log("Start: Set IsRunning = false");
}



    protected override void Update()
    {
        base.Update();

        if (isDetecting) return; // freeze while detection animation is playing

        if (isChasing)
            agent.SetDestination(player.position);
        else
            Patrol();
    }


    protected override void OnPlayerDetected()
    {
        if (!isChasing)
        {
            StartCoroutine(HandleDetectionAnimation());
        }

        lastSeenTime = Time.time;
    }


    protected override void OnPlayerLost()
{
    if (isChasing && Time.time - lastSeenTime > lostPlayerTime)
    {
        isChasing = false;
        agent.speed = patrolSpeed;
        animator.SetBool("IsRunning", false); // go back to walk
        Debug.Log("OnPlayerLost: Set IsRunning = false (returning to patrol)");
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

   private IEnumerator HandleDetectionAnimation()
{
    isDetecting = true;
    isChasing = false;
    agent.ResetPath(); // stop movement

    animator.SetTrigger("DetectedTrigger"); // play detected anim
    Debug.Log("HandleDetectionAnimation: Triggered 'DetectedTrigger'");

    animator.SetBool("IsRunning", false); // ensure running anim is off
    Debug.Log("HandleDetectionAnimation: Set IsRunning = false (during detection)");

    yield return new WaitForSeconds(1f); // match length of Detected animation

    isChasing = true;
    agent.speed = chaseSpeed;

    animator.SetBool("IsRunning", true); // start running anim
    Debug.Log("HandleDetectionAnimation: Set IsRunning = true (chasing)");

    isDetecting = false;
}




}

