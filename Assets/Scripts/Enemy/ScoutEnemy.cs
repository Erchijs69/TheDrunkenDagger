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
    
    // Allow rotation adjustment from Inspector
    public Vector3 spearRotation = new Vector3(0f, 0f, 90f);  // Default rotation (Z axis 90 degrees)

    private bool playerDetected = false;

    void Start()
    {
        StartCoroutine(ThrowSpearsAtIntervals());
    }

    void Update()
    {
        DetectPlayer();
    }

    void DetectPlayer()
    {
        if (player == null) return;

        Vector3 directionToPlayer = player.position - transform.position;
        float distanceToPlayer = directionToPlayer.magnitude;
        float angleToPlayer = Vector3.Angle(transform.forward, directionToPlayer);

        // If the player is within the detection range and in the field of view
        if (distanceToPlayer <= detectionRange && angleToPlayer <= fieldOfView / 2)
        {
            // If the player is detected, rotate to face the player
            playerDetected = true;
            RotateTowardsPlayer();
        }
        else
        {
            playerDetected = false;
        }
    }

    void RotateTowardsPlayer()
    {
        Vector3 directionToPlayer = player.position - transform.position;
        directionToPlayer.y = 0;  // Keep the rotation on the y-axis
        Quaternion rotation = Quaternion.LookRotation(directionToPlayer);
        transform.rotation = Quaternion.Slerp(transform.rotation, rotation, Time.deltaTime * 2f);
    }

    // Throw spear at regular intervals
    IEnumerator ThrowSpearsAtIntervals()
    {
        while (true)
        {
            if (playerDetected)
            {
                ThrowSpear();
            }
            yield return new WaitForSeconds(throwInterval);  // Wait for the specified interval before throwing the next spear
        }
    }

    // Throw the spear
    void ThrowSpear()
    {
        // Define the offset from the spawn point, you can adjust this value
        Vector3 spawnOffset = spearSpawnPoint.forward * 1.0f; // Adjust the multiplier to change the spawn distance in front of the spawn point

        // Calculate the spawn position by adding the offset to the spawn point's position
        Vector3 spawnPosition = spearSpawnPoint.position + spawnOffset;

        // Define the rotation of the spear, now editable via the Inspector
        Quaternion rotation = Quaternion.Euler(spearRotation);  // Use the inspector-defined rotation

        // Instantiate the spear at the new spawn position with the correct rotation
        GameObject spear = Instantiate(spearPrefab, spawnPosition, rotation);

        // Get the rigidbody and apply force
        Rigidbody spearRb = spear.GetComponent<Rigidbody>();
        if (spearRb != null)
        {
            // Apply force to throw the spear in the direction of the spawn point's forward
            spearRb.AddForce(spearSpawnPoint.forward * throwForce, ForceMode.VelocityChange);
        }

        // Make the spear rotate to face the player while in the air
        StartCoroutine(MakeSpearFacePlayer(spear));
    }

    // Coroutine to make the spear's Z-axis always face the player while it's flying
    IEnumerator MakeSpearFacePlayer(GameObject spear)
    {
        while (spear != null)
        {
            // Get the direction to the player
            Vector3 directionToPlayer = player.position - spear.transform.position;

            // Calculate the rotation that aligns the spear's Z-axis to the player
            Quaternion rotationToPlayer = Quaternion.LookRotation(directionToPlayer);

            // Smoothly rotate the spear to face the player
            spear.transform.rotation = Quaternion.Slerp(spear.transform.rotation, rotationToPlayer, Time.deltaTime * 10f);

            // Wait until the next frame
            yield return null;
        }
    }

    // Detect collision with player and restart scene
    void OnCollisionEnter(Collision collision)
    {
        if (collision.gameObject.CompareTag("Player"))
        {
            Debug.Log("Spear hit player, restarting scene.");
            RestartScene();
        }
    }

    // Restart the scene
    void RestartScene()
    {
        string currentScene = SceneManager.GetActiveScene().name;
        SceneManager.LoadScene(currentScene);
    }

    // Visualize the detection range and field of view in the editor
    void OnDrawGizmos()
    {
        if (player == null) return;

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
}
