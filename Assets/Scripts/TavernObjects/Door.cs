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
        if (!CompareTag("Door")) return;

        // Only open if it’s still closed
        if (isClosed)
        {
            mAnimator.SetTrigger("Open");

            if (!hasTriggeredElf && elfMovement != null)
            {
                StartCoroutine(EnableElfMovementAfterDelay());
                hasTriggeredElf = true;
            }

            if (questTrigger != null)
            {
                questTrigger.TriggerNextQuest();
            }

            isClosed = false;  // Update state so it doesn't reopen again
        }
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


