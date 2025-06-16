using System.Collections;
using UnityEngine;
using UnityEngine.UI;

public class JournalUI : MonoBehaviour
{
    public GameObject journalObject;            
    public Animator animator;                   
    public GameObject questTextContainer;       
    public float textDelay = 0.5f;              

    private bool isOpen = false;

    public PlayerMovement playerMovement;

    void Start()
    {
        journalObject.SetActive(false);          
        questTextContainer.SetActive(false);     
        playerMovement = FindObjectOfType<PlayerMovement>(); 

    }

    void Update()
{
    if (!IsDialogueActive() && !playerMovement.IsStealthed)  
    {
        if (Input.GetKeyDown(KeyCode.J))
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
}

private bool IsDialogueActive()
{
    
    return FindObjectOfType<DialogueManager>().dialoguePanel.activeSelf;
}


    IEnumerator OpenJournal()
    {
        isOpen = true;
        journalObject.SetActive(true);
        animator.Play("Open");                  
        yield return new WaitForSeconds(textDelay);
        questTextContainer.SetActive(true);    
    }

    void CloseJournal()
    {
        isOpen = false;
        journalObject.SetActive(false);
        questTextContainer.SetActive(false);     
    }
}
