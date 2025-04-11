using UnityEngine;
using TMPro;
using System.Collections;
using Unity.AI.Navigation.Samples;

public class DialogueManager : MonoBehaviour
{
    public TMP_Text dialogueText;
    public TMP_Text speakerNameText;
    public GameObject dialoguePanel;

    public DialogueLine[] dialogueLines; // Now uses custom class with speakerName
    private int currentLineIndex = 0;

    private ElfMovement elfMovement;
    private PlayerMovement playerMovement;
    private MouseLook mouseLook;

    void Start()
    {
        elfMovement = FindObjectOfType<ElfMovement>();
        playerMovement = FindObjectOfType<PlayerMovement>();
        mouseLook = FindObjectOfType<MouseLook>();
        dialoguePanel.SetActive(false);
    }

    void Update()
    {
        if (elfMovement != null && elfMovement.isDialogueActive)
        {
            if (Input.GetKeyDown(KeyCode.E) || Input.GetMouseButtonDown(0))
            {
                AdvanceDialogue();
            }
        }
    }

    public void StartDialogue()
    {
        if (elfMovement != null)
        {
            elfMovement.StopMovement();
        }

        dialoguePanel.SetActive(true);
        currentLineIndex = 0;
        ShowDialogueLine();

        if (playerMovement != null)
            playerMovement.enabled = false;

        if (mouseLook != null)
            mouseLook.enabled = false;

        Cursor.lockState = CursorLockMode.None;
        Cursor.visible = true;
    }

    public void ShowDialogueLine()
    {
        if (currentLineIndex < dialogueLines.Length)
        {
            speakerNameText.text = dialogueLines[currentLineIndex].speakerName;
            dialogueText.text = dialogueLines[currentLineIndex].lineText;
            currentLineIndex++;
        }
        else
        {
            EndDialogue();
        }
    }

    public void EndDialogue()
    {
        dialoguePanel.SetActive(false);

        if (elfMovement != null)
        {
            elfMovement.OnDialogueEnd();
        }

        if (playerMovement != null)
            playerMovement.enabled = true;

        if (mouseLook != null)
            mouseLook.enabled = true;

        Cursor.lockState = CursorLockMode.Locked;
        Cursor.visible = false;
    }

    public void AdvanceDialogue()
    {
        ShowDialogueLine();
    }
}







