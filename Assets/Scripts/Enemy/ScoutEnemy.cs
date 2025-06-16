using System.Collections;
using UnityEngine;
using UnityEngine.SceneManagement;

public class ScoutEnemy : BaseEnemy
{
    [Header("Spear Throwing")]
    public GameObject spearPrefab;
    public Transform spearSpawnPoint;
    public float throwInterval = 2.0f;
    public float throwForce = 10f;
    public float spearThrowAngle = 20f;
    public float predictionTime = 0.5f;
    public float spearLifetime = 10f;

    private Vector3 lastPlayerPosition;
    private Vector3 playerVelocity;

    private Animator animator;

    private float idleCheckTimer = 0f;
    private float idleCheckInterval = 3f; // how often to check
    private float idleVariantChance = 0.2f; // 20% chance

    protected override void Start()
    {
        base.Start();
        animator = GetComponentInChildren<Animator>(); // if animator is on child
        StartCoroutine(ThrowSpearsAtIntervals());
    }

    protected override void Update()
{
    base.Update();

    // Only check for idle variant if not chasing or throwing
    if (!playerDetected)
    {
        idleCheckTimer += Time.deltaTime;
        if (idleCheckTimer >= idleCheckInterval)
        {
            idleCheckTimer = 0f;

            if (Random.value < idleVariantChance)
            {
                animator.SetTrigger("PlayIdleVariant");
            }
        }
    }
}

    protected override void OnPlayerDetected()
    {
        RotateTowardsPlayer();
    }

    protected override void OnPlayerLost() { }

    void RotateTowardsPlayer()
    {
        Vector3 direction = player.position - transform.position;
        direction.y = 0;
        transform.rotation = Quaternion.Slerp(transform.rotation, Quaternion.LookRotation(direction), Time.deltaTime * 2f);
    }

    void TrackPlayerMovement()
    {
        Vector3 newPos = player.position;
        playerVelocity = (newPos - lastPlayerPosition) / Time.deltaTime;
        lastPlayerPosition = newPos;
    }

    IEnumerator ThrowSpearsAtIntervals()
    {
        while (true)
        {
            if (playerDetected)
            {
                RotateTowardsPlayer();
                animator.SetTrigger("ThrowTrigger"); // trigger throw animation
            }
            yield return new WaitForSeconds(throwInterval);
        }
    }

    void ThrowSpear()
    {
        Vector3 predictedPos = player.position + playerVelocity * predictionTime;
        Vector3 spawnPos = spearSpawnPoint.position + spearSpawnPoint.forward;

        Quaternion rot = Quaternion.LookRotation(predictedPos - spearSpawnPoint.position);
        rot *= Quaternion.Euler(spearThrowAngle, 0f, 0f);

        GameObject spear = Instantiate(spearPrefab, spawnPos, rot);
        if (spear.TryGetComponent<Rigidbody>(out var rb))
        {
            rb.AddForce((predictedPos - spearSpawnPoint.position).normalized * throwForce, ForceMode.VelocityChange);
        }

        Destroy(spear, spearLifetime);
    }
    
    public void OnThrowMoment()
{
    Vector3 predictedPos = player.position + playerVelocity * predictionTime;
    Vector3 spawnPos = spearSpawnPoint.position + spearSpawnPoint.forward;

    Quaternion rot = Quaternion.LookRotation(predictedPos - spearSpawnPoint.position);
    rot *= Quaternion.Euler(spearThrowAngle, 0f, 0f);

    GameObject spear = Instantiate(spearPrefab, spawnPos, rot);
    if (spear.TryGetComponent<Rigidbody>(out var rb))
    {
        rb.AddForce((predictedPos - spearSpawnPoint.position).normalized * throwForce, ForceMode.VelocityChange);
    }

    Destroy(spear, spearLifetime);
}


} 

