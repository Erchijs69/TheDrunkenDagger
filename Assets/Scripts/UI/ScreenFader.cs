using UnityEngine;
using UnityEngine.UI;
using System.Collections;
using System;

public class ScreenFader : MonoBehaviour
{
    public static ScreenFader Instance;

    public Image blackScreenImage; 
    public float fadeDuration = 0.2f;

    private void Awake()
    {
        if (Instance == null)
        {
            Instance = this;
            DontDestroyOnLoad(gameObject);
            SetAlpha(0);
            blackScreenImage.gameObject.SetActive(false); 
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
        
        yield return StartCoroutine(Fade(0, 1));

        
        player.position = targetPosition;

        
        onTeleported?.Invoke();

        
        yield return new WaitForSeconds(0.1f);

        
        yield return StartCoroutine(Fade(1, 0));
    }

    private IEnumerator Fade(float startAlpha, float endAlpha)
    {
        blackScreenImage.gameObject.SetActive(true); 

        float elapsed = 0f;
        while (elapsed < fadeDuration)
        {
            elapsed += Time.deltaTime;
            float alpha = Mathf.Lerp(startAlpha, endAlpha, elapsed / fadeDuration);
            SetAlpha(alpha);
            yield return null;
        }

        SetAlpha(endAlpha);

    
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