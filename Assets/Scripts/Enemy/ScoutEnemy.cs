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

    protected override void Start()
    {
        base.Start();
        StartCoroutine(ThrowSpearsAtIntervals());
    }

    protected override void Update()
    {
        base.Update();
        TrackPlayerMovement();
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
                ThrowSpear();
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


} 

