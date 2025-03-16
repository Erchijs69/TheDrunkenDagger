using UnityEngine;
using UnityEngine.SceneManagement;

public class Spear : MonoBehaviour
{
    void OnCollisionEnter(Collision collision)
    {
        if (collision.gameObject.CompareTag("Player"))
        {
            Debug.Log("Spear hit the player! Restarting scene...");
            RestartScene();
        }
        else if (collision.gameObject.CompareTag("Enemy"))
        {
            Debug.Log("Spear hit the enemy! Destroying spear...");
            Destroy(gameObject);  // Remove the spear on impact with the enemy
        }
    }

    void RestartScene()
    {
        string currentScene = SceneManager.GetActiveScene().name;
        SceneManager.LoadScene(currentScene);
    }
}
