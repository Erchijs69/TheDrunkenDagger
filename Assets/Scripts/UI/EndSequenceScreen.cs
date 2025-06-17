using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.EventSystems;
using TMPro;
using UnityEngine.SceneManagement;

public class EndSequenceScreen : MonoBehaviour, IInteractable
{
    [Header("UI Elements")]
    public Image fadeImage; // Your black Panel with Image component

    public GameObject hearts;
    public CanvasGroup textPhalinGroup; // CanvasGroup on the Phalin TMP text
    public CanvasGroup textCaspianGroup; // CanvasGroup on the Caspian TMP text

    [Header("Timing")]
    public float fadeDuration = 1f;
    public float textDisplayDuration = 2.5f;

    public void Interact()
    {
        Debug.Log("Starting end sequence...");
        hearts.SetActive(false);
        StartCoroutine(EndSequence());
    }

    private IEnumerator EndSequence()
    {
        // Fade to black
        yield return StartCoroutine(FadeImage(true));

        // Show first TMP text (Phalin)
        yield return StartCoroutine(ShowText(textPhalinGroup));

        // Show second TMP text (Caspian)
        yield return StartCoroutine(ShowText(textCaspianGroup));

        // ✅ Add a short pause before reload
        yield return new WaitForSeconds(1.5f);

        // Reload scene
        SceneManager.LoadScene(SceneManager.GetActiveScene().buildIndex);
    }

    private IEnumerator FadeImage(bool fadeToBlack)
    {
        float startAlpha = fadeToBlack ? 0 : 1;
        float endAlpha = fadeToBlack ? 1 : 0;
        float elapsed = 0f;

        Color color = fadeImage.color;
        while (elapsed < fadeDuration)
        {
            elapsed += Time.deltaTime;
            float alpha = Mathf.Lerp(startAlpha, endAlpha, elapsed / fadeDuration);
            fadeImage.color = new Color(color.r, color.g, color.b, alpha);
            yield return null;
        }

        fadeImage.color = new Color(color.r, color.g, color.b, endAlpha);
    }

    private IEnumerator ShowText(CanvasGroup textGroup)
    {
        yield return StartCoroutine(FadeCanvasGroup(textGroup, 0, 1, 1f));
        yield return new WaitForSeconds(textDisplayDuration);
        yield return StartCoroutine(FadeCanvasGroup(textGroup, 1, 0, 1f));
    }

    private IEnumerator FadeCanvasGroup(CanvasGroup group, float start, float end, float duration)
    {
        float elapsed = 0f;
        while (elapsed < duration)
        {
            elapsed += Time.deltaTime;
            group.alpha = Mathf.Lerp(start, end, elapsed / duration);
            yield return null;
        }
        group.alpha = end;
    }
}
