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

    private PlayerMovement playerMovement;

    private StealthScreenEffect stealthEffect;

    private bool isShrunk = false;

    public PlayerStealthKill playerStealthKill;

    private void Awake()
{
    playerMovement = GetComponent<PlayerMovement>();
    stealthEffect = FindObjectOfType<StealthScreenEffect>();
    if (playerMovement == null)
        Debug.LogError("PlayerMovement script not found on Player!");
}

    public void ApplyBuff(string buffName)
    {
        switch (buffName)
        {
            case "Speed Boost":
                StartCoroutine(ApplySpeedBoost());
                break;

            case "Jump Height":
                StartCoroutine(ApplyJumpBoost());
                break;

            case "Swim Speed":
                ApplySwimSpeedBoost(swimBoostMultiplier, swimBoostDuration);
                break;

            case "Wraith Form":
                StartCoroutine(ApplyStealthCoroutine(stealthDuration));
                break;

            case "Tiny Tina's Curse":
                ApplyPermanentShrink();
                break;    
                
            case "Master Assassin":
                StartCoroutine(ApplyMasterAssassinBuff());
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

    public void ApplySwimSpeedBoost(float swimSpeed, float duration)
    {
        void Apply()
        {
            playerMovement.SetSwimSpeed(swimSpeed);
            Debug.Log("Swim speed boost applied!");
        }

        void Reset()
        {
            playerMovement.SetSwimSpeed(5f); // Reset to your desired "base" swim speed
            Debug.Log("Swim speed boost reset!");
        }

        ApplyBuffWithTimer(Apply, Reset, duration);
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

    // Enable fast stealth mode (affects crouch speed and kill animation speed)
    playerMovement.EnableFastStealthMode(true);

    // Adjust the kill animation speed in PlayerStealthKill
    if (playerStealthKill != null)
    {
        playerStealthKill.fastStealthMode = true;
        playerStealthKill.fastKillAnimSpeed = 2f; // Set this to whatever value you need
    }

    // Wait for the duration of the buff
    yield return new WaitForSeconds(stealthBuffDuration);

    // Disable fast stealth mode
    playerMovement.EnableFastStealthMode(false);

    // Reset the kill animation speed
    if (playerStealthKill != null)
    {
        playerStealthKill.fastStealthMode = false;
        playerStealthKill.fastKillAnimSpeed = 1f; // Reset to default speed
    }

    Debug.Log("Master Assassin buff ended!");
}


}


