using UnityEngine;
using TMPro;
using System.Collections;
using Unity.AI.Navigation.Samples;

[System.Serializable]
public class DialogueManager : MonoBehaviour
{
    public TMP_Text dialogueText;
    public TMP_Text speakerNameText;
    public GameObject dialoguePanel;

    public DialogueLine[] dialogueLines; 
    private int currentLineIndex = 0;

    private ElfMovement elfMovement;
    private PlayerMovement playerMovement;
    private MouseLook mouseLook;

    public bool triggersQuestOnEnd = false;
    public bool requiresQuestCompletion = false;
    public QuestSO requiredQuest;

   
    private bool isAtLastLineRepeated = false;

    public bool dialogueWasCancelled { get; private set; } = false;


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
                if (isAtLastLineRepeated)
                {
                    
                }
                else
                {
                    AdvanceDialogue();
                }
            }

            
            if (Input.GetKeyDown(KeyCode.Escape))
{
    dialogueWasCancelled = true;
    EndDialogue();
}

        }
    }

    public void StartDialogue()
{
    if (elfMovement != null)
    {
        elfMovement.StopMovement();
    }

    dialogueWasCancelled = false; 
    dialoguePanel.SetActive(true);
    currentLineIndex = 0;
    isAtLastLineRepeated = false;
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
            if (requiresQuestCompletion && requiredQuest != null && !requiredQuest.isComplete)
            {
                
                currentLineIndex = dialogueLines.Length - 1;
                speakerNameText.text = dialogueLines[currentLineIndex].speakerName;
                dialogueText.text = dialogueLines[currentLineIndex].lineText;

               
                isAtLastLineRepeated = true;

                return;
            }
            EndDialogue();
        }
    }

    void EndDialogue()
    {
        dialoguePanel.SetActive(false);

        if (triggersQuestOnEnd)
        {
            QuestManager.Instance.CompleteQuest(); 
        }

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

        
        isAtLastLineRepeated = false;
    }

    public void AdvanceDialogue()
    {
        ShowDialogueLine();
    }
}








