using UnityEngine;
using UnityEngine.UI;
using System.Collections;

public class StealthScreenEffect : MonoBehaviour
{
    public static StealthScreenEffect Instance; // ✅ Add this line

    public Image stealthOverlayImage;
    public float fadeSpeed = 2f;

    private Coroutine fadeCoroutine;

    private void Awake()
    {
        // ✅ Ensure only one instance exists
        if (Instance == null)
        {
            Instance = this;
        }
        else
        {
            Destroy(gameObject); // Prevent duplicates
        }

        // Optional: Hide on start
        stealthOverlayImage.color = new Color(0, 0, 1, 0);
    }

    public void ShowOverlay()
    {
        if (fadeCoroutine != null) StopCoroutine(fadeCoroutine);
        fadeCoroutine = StartCoroutine(FadeToAlpha(0.3f)); // Semi-transparent blue
    }

    public void HideOverlay()
    {
        if (fadeCoroutine != null) StopCoroutine(fadeCoroutine);
        fadeCoroutine = StartCoroutine(FadeToAlpha(0f)); // Transparent
    }

    private IEnumerator FadeToAlpha(float targetAlpha)
    {
        Color currentColor = stealthOverlayImage.color;

        while (Mathf.Abs(currentColor.a - targetAlpha) > 0.01f)
        {
            currentColor.a = Mathf.MoveTowards(currentColor.a, targetAlpha, fadeSpeed * Time.deltaTime);
            stealthOverlayImage.color = currentColor;
            yield return null;
        }

        currentColor.a = targetAlpha;
        stealthOverlayImage.color = currentColor;
    }
}

