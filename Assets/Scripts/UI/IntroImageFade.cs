using System.Collections;
using UnityEngine;

public class IntroImageFade : MonoBehaviour
{
    public CanvasGroup canvasGroup;
    public float fadeDuration = 1f;
    public float showDuration = 1f;

    public MouseLook mouseLook; 

    public void PlayIntroFade()
    {
        gameObject.SetActive(true); 
        StartCoroutine(FadeRoutine());
    }

    IEnumerator FadeRoutine()
    {
        canvasGroup.alpha = 0f;

        
        float t = 0;
        while (t < fadeDuration)
        {
            t += Time.deltaTime;
            canvasGroup.alpha = Mathf.Lerp(0, 1, t / fadeDuration);
            yield return null;
        }

        
        yield return new WaitForSeconds(showDuration);

        
        t = 0;
        while (t < fadeDuration)
        {
            t += Time.deltaTime;
            canvasGroup.alpha = Mathf.Lerp(1, 0, t / fadeDuration);
            yield return null;
        }

        gameObject.SetActive(false);

        
        mouseLook.EnableMouseLook();
    }
}




