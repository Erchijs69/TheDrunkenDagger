using System.Collections;
using UnityEngine;
using UnityEngine.SceneManagement;

public class ScoutEnemy : MonoBehaviour
{
    public Transform player;  // Player object
    public float detectionRange = 10f;  // Range at which the scout can see the player
    public float fieldOfView = 120f;  // Field of view angle
    public GameObject spearPrefab;  // The spear prefab
    public Transform spearSpawnPoint;  // Where the spear will be thrown from
    public float throwInterval = 2.0f;  // Time between throws
    public float throwForce = 10f;  // Force at which the spear is thrown
    public float spearThrowAngle = 20f;  // Adjustable throw angle in inspector
    public float predictionTime = 0.5f; // Time ahead to predict player movement
    public float spearLifetime = 10f; // Time before the spear is destroyed

    private bool playerDetected = false;
    private Vector3 lastPlayerPosition;
    private Vector3 playerVelocity;

    void Start()
    {
        StartCoroutine(ThrowSpearsAtIntervals());
    }

    void Update()
    {
        DetectPlayer();
        TrackPlayerMovement();
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
                    playerDetected = true;
                    RotateTowardsPlayer();
                }
                else
                {
                    playerDetected = false;
                }
            }
        }
        else
        {
            playerDetected = false;
        }
    }

    void RotateTowardsPlayer()
    {
        Vector3 directionToPlayer = player.position - transform.position;
        directionToPlayer.y = 0;
        Quaternion rotation = Quaternion.LookRotation(directionToPlayer);
        transform.rotation = Quaternion.Slerp(transform.rotation, rotation, Time.deltaTime * 2f);
    }

    void TrackPlayerMovement()
    {
        Vector3 newPosition = player.position;
        playerVelocity = (newPosition - lastPlayerPosition) / Time.deltaTime;
        lastPlayerPosition = newPosition;
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
        Vector3 predictedPosition = player.position + (playerVelocity * predictionTime);
        Vector3 spawnOffset = spearSpawnPoint.forward * 1.0f;
        Vector3 spawnPosition = spearSpawnPoint.position + spawnOffset;

        Quaternion rotation = Quaternion.LookRotation(predictedPosition - spearSpawnPoint.position);
        rotation *= Quaternion.Euler(spearThrowAngle, 0f, 0f);

        GameObject spear = Instantiate(spearPrefab, spawnPosition, rotation);

        Rigidbody spearRb = spear.GetComponent<Rigidbody>();
        if (spearRb != null)
        {
            spearRb.AddForce((predictedPosition - spearSpawnPoint.position).normalized * throwForce, ForceMode.VelocityChange);
        }

        // Start the spear lifetime coroutine to destroy it after the specified time
        StartCoroutine(DestroySpearAfterTime(spear, spearLifetime));
    }

    IEnumerator DestroySpearAfterTime(GameObject spear, float lifetime)
    {
        // Wait for the lifetime duration before destroying the spear
        yield return new WaitForSeconds(lifetime);
        Destroy(spear);
    }

    void OnCollisionEnter(Collision collision)
    {
        if (collision.gameObject.CompareTag("Player"))
        {
            Debug.Log("Spear hit player, restarting scene.");
            RestartScene();
        }
    }

    void RestartScene()
    {
        string currentScene = SceneManager.GetActiveScene().name;
        SceneManager.LoadScene(currentScene);
    }

    void OnDrawGizmos()
    {
        if (player == null) return;

        Gizmos.color = Color.red;
        Gizmos.DrawWireSphere(transform.position, detectionRange);

        Gizmos.color = Color.green;
        Vector3 leftAngle = Quaternion.Euler(0, -fieldOfView / 2, 0) * transform.forward * detectionRange;
        Vector3 rightAngle = Quaternion.Euler(0, fieldOfView / 2, 0) * transform.forward * detectionRange;

        Gizmos.DrawLine(transform.position, transform.position + leftAngle);
        Gizmos.DrawLine(transform.position, transform.position + rightAngle);

        if (player != null)
        {
            Gizmos.color = Color.blue;
            Gizmos.DrawLine(transform.position, player.position);
        }
    }
}
