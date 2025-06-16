using UnityEngine;

public class LookAtPlayer : MonoBehaviour
{
    private Transform playerCamera;

    void Start()
    {
        playerCamera = Camera.main.transform; 
    }

    void Update()
    {
        if (playerCamera != null)
        {
            
            transform.rotation = Quaternion.LookRotation(transform.position - playerCamera.position);

            
            transform.Rotate(Vector3.up * Time.deltaTime * 50f, Space.World);
        }
    }
}

