using UnityEngine;
using UnityEngine.UI;
using System.Collections;
using System;

public class ScreenFader : MonoBehaviour
{
    public static ScreenFader Instance;

    public Image blackScreenImage; // Assign the full-screen black Image here
    public float fadeDuration = 1f;

    private void Awake()
{
    if (Instance == null)
    {
        Instance = this;
        DontDestroyOnLoad(gameObject);
        SetAlpha(0);
        blackScreenImage.gameObject.SetActive(false); // <-- Disable it initially
    }
    else
    {
        Destroy(gameObject);
    }
}


    public void FadeAndTeleport(Transform player, Vector3 targetPosition, Action onTeleported = null)
    {
        StartCoroutine(FadeTeleportRoutine(player, targetPosition, onTeleported));
    }

    private IEnumerator FadeTeleportRoutine(Transform player, Vector3 targetPosition, Action onTeleported)
    {
        // Fade to black
        yield return StartCoroutine(Fade(0, 1));

        // Teleport player
        player.position = targetPosition;

        // Callback for actions after teleport (like changing skybox)
        onTeleported?.Invoke();

        // Small wait for safety
        yield return new WaitForSeconds(0.1f);

        // Fade back to transparent
        yield return StartCoroutine(Fade(1, 0));
    }

    private IEnumerator Fade(float startAlpha, float endAlpha)
{
    blackScreenImage.gameObject.SetActive(true); // Enable before fading

    float elapsed = 0f;
    while (elapsed < fadeDuration)
    {
        elapsed += Time.deltaTime;
        float alpha = Mathf.Lerp(startAlpha, endAlpha, elapsed / fadeDuration);
        SetAlpha(alpha);
        yield return null;
    }

    SetAlpha(endAlpha);

    // If faded to transparent, disable it
    if (endAlpha == 0)
    {
        blackScreenImage.gameObject.SetActive(false);
    }
}


    private void SetAlpha(float alpha)
    {
        Color c = blackScreenImage.color;
        c.a = alpha;
        blackScreenImage.color = c;
    }
}


