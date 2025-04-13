using UnityEngine;
using UnityEngine.SceneManagement;

public class Cart : MonoBehaviour, IInteractable
{
    [SerializeField] private string sceneToLoad = "Scene2";

    public void Interact()
    {
        Debug.Log("Interacting with the cart. Loading scene...");
        SceneManager.LoadScene(sceneToLoad);
    }
}

