using System.Collections;
using UnityEngine;

public class PlayerStealthKill : MonoBehaviour
{
    public GameObject daggerObject; // Assign in inspector
    public PlayerMovement playerMovement;
    public MouseLook mouseLook;

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
                anim.Play("StealthKill");
                yield return new WaitForSeconds(anim.GetCurrentAnimatorStateInfo(0).length);
            }
            daggerObject.SetActive(false);
        }
        else
        {
            Debug.LogWarning("Dagger not assigned.");
            yield return new WaitForSeconds(1f);
        }

        Destroy(enemy.gameObject); // Kill the enemy

        if (playerMovement != null) playerMovement.canMove = true;
        if (mouseLook != null) mouseLook.FreezeLook(false);
    }
}

