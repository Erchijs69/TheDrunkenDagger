using UnityEngine;
using System.Collections;
using Unity.AI.Navigation.Samples;

public class BowAnimationController : MonoBehaviour
{
    private Animator bowAnimator;
    private Animator elfAnimator;
    private ElfMovement elfMovement;

    void Start()
    {
        bowAnimator = GetComponent<Animator>();
        elfMovement = GetComponentInParent<ElfMovement>();
        elfAnimator = elfMovement?.GetComponentInChildren<Animator>();

        if (bowAnimator == null)
            Debug.LogWarning("No Animator found on Bow!");

        if (elfMovement == null)
            Debug.LogWarning("No ElfMovement script found in parent!");

        if (elfAnimator == null)
            Debug.LogWarning("No Animator found in Elf!");
    }

    void Update()
{
    if (bowAnimator == null || elfAnimator == null)
        return;

    // Sync walking and bow set bools as before
    bool isWalking = elfAnimator.GetBool("isWalking");
    bowAnimator.SetBool("isWalking", isWalking);

    bool useBowSet = elfAnimator.GetBool("useBowSet");
    bowAnimator.SetBool("useBowSet", useBowSet);

    // Check if Elf is currently playing an idle animation by tag
    AnimatorStateInfo elfState = elfAnimator.GetCurrentAnimatorStateInfo(0);
    bool isIdlePlaying = elfState.IsTag("Idle");
    bowAnimator.SetBool("isIdlePlaying", isIdlePlaying);

    // Sync DrawBow animation if playing
    if (elfState.IsName("DrawBow"))
    {
        bowAnimator.Play("DrawBow", 0, elfState.normalizedTime);
    }

    // Sync Shoot animation if playing
    if (elfState.IsName("Shoot"))
    {
        bowAnimator.Play("Shoot", 0, elfState.normalizedTime);
    }
}

}




