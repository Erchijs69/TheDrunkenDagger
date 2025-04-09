using UnityEngine;
using Unity.AI.Navigation.Samples;
using System.Collections;
using System.Collections.Generic;

public class Door : MonoBehaviour, IInteractable
{
    public Animator mAnimator;
    private bool isClosed = true;
    public ElfMovement elfMovement;
    public float doorAnimationDelay = 0.5f;
    private bool hasTriggeredElf = false;

    // Add a reference to the QuestTrigger script
    public QuestTrigger questTrigger;  // Reference to the QuestTrigger

    void Start()
    {
        mAnimator = GetComponent<Animator>();

        if (elfMovement == null)
        {
            Debug.Log("No ElfMovement assigned to the door.");
        }
        else
        {
            elfMovement.StopMovement();
        }
    }

    public void Interact()
    {
        // Ensure this is only for door interactions
        if (!CompareTag("Door")) return;

        if (isClosed)
        {
            mAnimator.SetTrigger("Open");

            if (!hasTriggeredElf && elfMovement != null)
            {
                StartCoroutine(EnableElfMovementAfterDelay());
                hasTriggeredElf = true;
            }

            // Trigger the next quest after opening the door
            if (questTrigger != null)
            {
                questTrigger.TriggerNextQuest();
            }
        }
        else
        {
            mAnimator.SetTrigger("Close");
        }

        isClosed = !isClosed;
    }

    private IEnumerator EnableElfMovementAfterDelay()
    {
        yield return new WaitForSeconds(doorAnimationDelay);
        if (elfMovement != null)
        {
            elfMovement.StartMovement();
        }
    }
}

