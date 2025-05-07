using System.Collections;
using UnityEngine;

public class PlayerStealthKill : MonoBehaviour
{
    public GameObject daggerObject;
    public PlayerMovement playerMovement;
    public MouseLook mouseLook;

    [Header("Fast Stealth Settings")]
    public bool fastStealthMode = false;
    public float fastKillAnimSpeed = 2f; // animation speed multiplier when enabled


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

            // Wait for the adjusted animation length
            float animLength = anim.GetCurrentAnimatorStateInfo(0).length / anim.speed;
            yield return new WaitForSeconds(animLength);

            anim.speed = originalSpeed;
        }

        daggerObject.SetActive(false);
    }
    else
    {
        Debug.LogWarning("Dagger not assigned.");
        yield return new WaitForSeconds(1f);
    }

    Destroy(enemy.gameObject);

    if (playerMovement != null) playerMovement.canMove = true;
    if (mouseLook != null) mouseLook.FreezeLook(false);
}

}


