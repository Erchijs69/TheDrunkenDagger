using System.Collections;
using UnityEngine;
using UnityEngine.UI;

public class JournalUI : MonoBehaviour
{
    public GameObject journalObject;             // The full journal UI GameObject
    public Animator animator;                    // Animator for open animation
    public GameObject questTextContainer;        // Object holding quest text (Title + Description)
    public float textDelay = 0.5f;               // Delay before showing quest text

    private bool isOpen = false;

    public PlayerMovement playerMovement;

    void Start()
    {
        journalObject.SetActive(false);          // Hide journal on start
        questTextContainer.SetActive(false);     // Hide text on start
        playerMovement = FindObjectOfType<PlayerMovement>(); // Automatically find the PlayerMovement component

    }

    void Update()
    {
        if (Input.GetKeyDown(KeyCode.J) && !playerMovement.IsStealthed)
    {
        if (!isOpen)
        {
            StartCoroutine(OpenJournal());
        }
        else
        {
            CloseJournal();
        }
    }
    }

    IEnumerator OpenJournal()
    {
        isOpen = true;
        journalObject.SetActive(true);
        animator.Play("Open");                   // Play opening animation
        yield return new WaitForSeconds(textDelay);
        questTextContainer.SetActive(true);      // Show quest text after delay
    }

    void CloseJournal()
    {
        isOpen = false;
        journalObject.SetActive(false);
        questTextContainer.SetActive(false);     // Reset text state too
    }
}
