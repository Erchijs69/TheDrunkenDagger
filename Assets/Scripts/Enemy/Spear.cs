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
    }

    void RestartScene()
    {
        string currentScene = SceneManager.GetActiveScene().name;
        SceneManager.LoadScene(currentScene);
    }
}
