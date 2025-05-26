using System.Collections;
using UnityEngine;

public class IntroImageFade : MonoBehaviour
{
    public CanvasGroup canvasGroup;
    public float fadeDuration = 1f;
    public float showDuration = 1f;

    public MouseLook mouseLook; // Assign in inspector

    public void PlayIntroFade()
    {
        gameObject.SetActive(true); // Enable the image
        StartCoroutine(FadeRoutine());
    }

    IEnumerator FadeRoutine()
    {
        canvasGroup.alpha = 0f;

        // Fade in
        float t = 0;
        while (t < fadeDuration)
        {
            t += Time.deltaTime;
            canvasGroup.alpha = Mathf.Lerp(0, 1, t / fadeDuration);
            yield return null;
        }

        // Wait
        yield return new WaitForSeconds(showDuration);

        // Fade out
        t = 0;
        while (t < fadeDuration)
        {
            t += Time.deltaTime;
            canvasGroup.alpha = Mathf.Lerp(1, 0, t / fadeDuration);
            yield return null;
        }

        gameObject.SetActive(false);

        // 🔒 Lock cursor and enable mouse look here
        mouseLook.EnableMouseLook();
    }
}




