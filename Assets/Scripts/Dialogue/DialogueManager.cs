using UnityEngine;
using TMPro;
using System.Collections;
using Unity.AI.Navigation.Samples;

public class DialogueManager : MonoBehaviour
{
    public TMP_Text dialogueText;
    public GameObject dialoguePanel;
    public string[] dialogueLines; // Dialogue lines with color tags
    private int currentLineIndex = 0;

    private ElfMovement elfMovement;

    void Start()
    {
        elfMovement = FindObjectOfType<ElfMovement>();
        dialoguePanel.SetActive(false);
    }

    void Update()
    {
        if (elfMovement != null && elfMovement.isDialogueActive)
        {
            if (Input.GetKeyDown(KeyCode.E))
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
    }

    public void ShowDialogueLine()
    {
        if (currentLineIndex < dialogueLines.Length)
        {
            dialogueText.text = dialogueLines[currentLineIndex];
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
    }

    public void AdvanceDialogue()
    {
        ShowDialogueLine();
    }
}


