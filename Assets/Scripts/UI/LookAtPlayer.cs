using UnityEngine;

public class LookAtPlayer : MonoBehaviour
{
    private Transform playerCamera;

    void Start()
    {
        playerCamera = Camera.main.transform; // Get the main camera
    }

    void Update()
    {
        if (playerCamera != null)
        {
            // Make the text face the player
            transform.rotation = Quaternion.LookRotation(transform.position - playerCamera.position);

            // Optional: You can add a little spin effect
            transform.Rotate(Vector3.up * Time.deltaTime * 50f, Space.World);
        }
    }
}

