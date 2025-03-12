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
    private bool hasTriggeredElf = false;  // Ensures the elf only starts moving once

    void Start()
    {
        mAnimator = GetComponent<Animator>();

        if (elfMovement == null)
        {
            Debug.Log("No ElfMovement assigned to the door.");
        }
        else
        {
            elfMovement.StopMovement();  // Ensure the Elf starts stationary
        }
    }

    public void Interact()
    {
        // Ensure this is only for door interactions
        if (!CompareTag("Door")) return;

        // Open door animation and trigger elf movement only the first time
        if (isClosed)
        {
            mAnimator.SetTrigger("Open");

            // ONLY trigger Elf movement the FIRST TIME the door opens
            if (!hasTriggeredElf && elfMovement != null)
            {
                StartCoroutine(EnableElfMovementAfterDelay());
                hasTriggeredElf = true;  // Mark that we already triggered the elf
            }
        }
        else
        {
            mAnimator.SetTrigger("Close");
        }

        isClosed = !isClosed;  // Toggle the door's open/closed state
    }

    private IEnumerator EnableElfMovementAfterDelay()
    {
        yield return new WaitForSeconds(doorAnimationDelay);
        if (elfMovement != null)
        {
            elfMovement.StartMovement();  // Start movement ONLY ONCE
        }
    }
}
