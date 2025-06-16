using System.Collections;
using UnityEngine;

public class PlayerStealthKill : MonoBehaviour
{
    public GameObject daggerObject;
    public PlayerMovement playerMovement;
    public MouseLook mouseLook;

    [Header("Fast Stealth Settings")]
    public bool fastStealthMode = false;
    public float fastKillAnimSpeed = 2f;

    [Header("Stealth UI")]
    public CanvasGroup stealthTextUI; 

    public void ExecuteStealthKill(BaseEnemy enemy)
    {
        StartCoroutine(StealthKillRoutine(enemy));
    }

    private IEnumerator StealthKillRoutine(BaseEnemy enemy)
    {
        if (playerMovement != null) playerMovement.canMove = false;
        if (mouseLook != null) mouseLook.FreezeLook(true);

        if (daggerObject != null)
        {
            daggerObject.SetActive(true);
            Animator anim = daggerObject.GetComponent<Animator>();
            if (anim != null)
            {
                float originalSpeed = anim.speed;
                anim.speed = fastStealthMode ? fastKillAnimSpeed : 1f;
                anim.Play("StealthKill");

                float animLength = anim.GetCurrentAnimatorStateInfo(0).length / anim.speed;
                yield return new WaitForSeconds(animLength);

                anim.speed = originalSpeed;
            }
            else
            {
                yield return new WaitForSeconds(1f);
            }

            daggerObject.SetActive(false);
        }
        else
        {
            Debug.LogWarning("Dagger not assigned.");
            yield return new WaitForSeconds(1f);
        }

        Destroy(enemy.gameObject);

        ShowStealthUI(); 

        if (playerMovement != null) playerMovement.canMove = true;
        if (mouseLook != null) mouseLook.FreezeLook(false);
    }

    private void ShowStealthUI()
    {
        if (stealthTextUI == null) return;

        stealthTextUI.gameObject.SetActive(true);
        stealthTextUI.alpha = 1f;
        StartCoroutine(FadeOutStealthUI());
    }

    private IEnumerator FadeOutStealthUI()
    {
        yield return new WaitForSeconds(2f); 

        float fadeDuration = 1f;
        float t = 0f;

        while (t < fadeDuration)
        {
            t += Time.deltaTime;
            stealthTextUI.alpha = Mathf.Lerp(1f, 0f, t / fadeDuration);
            yield return null;
        }

        stealthTextUI.gameObject.SetActive(false);
    }
}




