using UnityEngine;
using UnityEngine.SceneManagement;

public class GameResetInteractable : MonoBehaviour, IInteractable
{
    public void Interact()
    {
        Debug.Log("Resetting game...");
        SceneManager.LoadScene(SceneManager.GetActiveScene().buildIndex);
    }
}

