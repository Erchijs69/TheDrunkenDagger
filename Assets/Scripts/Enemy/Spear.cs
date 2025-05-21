using UnityEngine;
using UnityEngine.SceneManagement;

public class Spear : MonoBehaviour
{
    private void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("Player"))
        {
            PlayerMovement playerMovement = other.GetComponent<PlayerMovement>();
            if (playerMovement != null && !playerMovement.IsStealthed && !playerMovement.IsSmall)
            {
                GameManager.Managerinstance?.PlayerDied();
            }
        }

        Destroy(gameObject); // optional: destroy on hit
    }
}

