using System.Collections;
using UnityEngine;

public class PlayerBuffs : MonoBehaviour
{
    [Header("Speed Boost Settings")]
    public float speedBoostMultiplier = 2f;
    public float speedBoostDuration = 5f;

    [Header("Jump Boost Settings")]
    public float jumpBoostMultiplier = 2f;
    public float jumpBoostDuration = 5f;

    [Header("Swim Boost Settings")]
    public float swimBoostMultiplier = 2f;
    public float swimBoostDuration = 5f;

    [Header("Wraith Settings")]
    public float stealthDuration = 30f;

    [Header("Stealth Buff Timer")]
    public float stealthBuffDuration = 30f;


    [HideInInspector]
    public bool isTinyTinasCurseActive = false;

    private PlayerMovement playerMovement;

    private StealthScreenEffect stealthEffect;

    private bool isShrunk = false;

    public PlayerStealthKill playerStealthKill;

    [HideInInspector]
    public bool isMasterAssassinActive = false;

    public BuffUIManager buffUIManager;

    private Coroutine speedBoostCoroutine;
    private Coroutine jumpBoostCoroutine;
    private Coroutine swimBoostCoroutine;
    private Coroutine stealthCoroutine;
    private Coroutine masterAssassinCoroutine;



    private void Awake()
{
    playerMovement = GetComponent<PlayerMovement>();
    stealthEffect = FindObjectOfType<StealthScreenEffect>();
    if (playerMovement == null)
        Debug.LogError("PlayerMovement script not found on Player!");
}

    public void ApplyBuff(string buffName)
{
    float duration = 10f;  // Default duration, but will be overridden below
    
    switch (buffName)
    {
        case "Speed Boost":
            duration = speedBoostDuration;
            if (speedBoostCoroutine != null) StopCoroutine(speedBoostCoroutine);
            speedBoostCoroutine = StartCoroutine(ApplySpeedBoost());
            buffUIManager.ShowBuff("Speed Boost", duration);
            break;

        case "Jump Height":
            duration = jumpBoostDuration;
            if (jumpBoostCoroutine != null) StopCoroutine(jumpBoostCoroutine);
            jumpBoostCoroutine = StartCoroutine(ApplyJumpBoost());
            buffUIManager.ShowBuff("Jump Height", duration);
            break;

        case "Swim Speed":
            duration = swimBoostDuration;
            if (swimBoostCoroutine != null) StopCoroutine(swimBoostCoroutine);
            swimBoostCoroutine = StartCoroutine(SwimSpeedCoroutine(swimBoostMultiplier, duration));
            buffUIManager.ShowBuff("Swim Speed", duration);
            break;

        case "Wraith Form":
            duration = stealthDuration;
            if (stealthCoroutine != null) StopCoroutine(stealthCoroutine);
            stealthCoroutine = StartCoroutine(ApplyStealthCoroutine(duration));
            buffUIManager.ShowBuff("Wraith Form", duration);
            break;

        case "Tiny Tina's Curse":
            duration = 9999999f; // Or set to your desired long duration
            ApplyPermanentShrink();
            isTinyTinasCurseActive = true;
            buffUIManager.ShowBuff("Tiny Tina's Curse", duration);
            break;


        case "Master Assassin":
            duration = stealthBuffDuration;
            if (masterAssassinCoroutine != null) StopCoroutine(masterAssassinCoroutine);
            masterAssassinCoroutine = StartCoroutine(ApplyMasterAssassinBuff());
            buffUIManager.ShowBuff("Master Assassin", duration);
            break;

            default:
            Debug.LogWarning($"Buff '{buffName}' not recognized.");
            break;
    }
}


    private IEnumerator ApplySpeedBoost()
    {
        if (playerMovement == null) yield break;

        Debug.Log("Speed Boost applied!");
        playerMovement.SetSpeedMultiplier(speedBoostMultiplier);
        yield return new WaitForSeconds(speedBoostDuration);
        playerMovement.SetSpeedMultiplier(1f);
        Debug.Log("Speed Boost reset!");
    }

    private IEnumerator ApplyJumpBoost()
    {
        if (playerMovement == null) yield break;

        Debug.Log("Jump Height Boost applied!");
        playerMovement.SetJumpMultiplier(jumpBoostMultiplier);
        yield return new WaitForSeconds(jumpBoostDuration);
        playerMovement.SetJumpMultiplier(1f);
        Debug.Log("Jump Height Boost reset!");
    }

    private IEnumerator SwimSpeedCoroutine(float swimSpeed, float duration)
{
    playerMovement.SetSwimSpeed(swimSpeed);
    Debug.Log("Swim speed boost applied!");
    yield return new WaitForSeconds(duration);
    playerMovement.SetSwimSpeed(5f); // Replace with your default speed
    Debug.Log("Swim speed boost reset!");
}


    private void ApplyBuffWithTimer(System.Action applyEffect, System.Action removeEffect, float duration)
    {
        applyEffect.Invoke();
        StartCoroutine(RemoveBuffAfterDelay(removeEffect, duration));
    }

    private IEnumerator RemoveBuffAfterDelay(System.Action removeEffect, float delay)
    {
        yield return new WaitForSeconds(delay);
        removeEffect.Invoke();
    }

    private IEnumerator ApplyStealthCoroutine(float duration)
{
    playerMovement.SetStealth(true);  
    Debug.Log("Wraith Form activated.");

    // Show the blue overlay
    StealthScreenEffect.Instance?.ShowOverlay();

    yield return new WaitForSeconds(duration);

    playerMovement.SetStealth(false);
    Debug.Log("Wraith Form ended.");

    // Hide the blue overlay
    StealthScreenEffect.Instance?.HideOverlay();
}

public void ApplyPermanentShrink()
{
    if (isShrunk) return;

    Transform playerTransform = GetComponent<Transform>();
    if (playerTransform != null)
    {
        playerTransform.localScale *= 0.2f;
        isShrunk = true;
        Debug.Log("smoll ┗|｀O′|┛");
    }
}

public IEnumerator ApplyMasterAssassinBuff()
{
    Debug.Log("Master Assassin buff applied!");
    isMasterAssassinActive = true;

    // Enable fast stealth mode (affects crouch speed and kill animation speed)
    playerMovement.EnableFastStealthMode(true);

    if (playerStealthKill != null)
    {
        playerStealthKill.fastStealthMode = true;
        playerStealthKill.fastKillAnimSpeed = 2f;
    }

    yield return new WaitForSeconds(stealthBuffDuration);

    // Reset everything
    isMasterAssassinActive = false;
    playerMovement.EnableFastStealthMode(false);

    if (playerStealthKill != null)
    {
        playerStealthKill.fastStealthMode = false;
        playerStealthKill.fastKillAnimSpeed = 1f;
    }

    Debug.Log("Master Assassin buff ended!");
}

}


