using UnityEngine;
using UnityEngine.SceneManagement;
using System.Collections;

public abstract class BaseEnemy : MonoBehaviour, IInteractable
{
    public Transform player;
    public GameObject daggerObject; // Assign this in Inspector
    public float detectionRange = 10f;
    public float fieldOfView = 120f;

    protected bool playerDetected = false;
    protected float lastSeenTime;

    protected virtual void Update()
    {
        DetectPlayer();
    }

    protected virtual void DetectPlayer()
    {
        if (player == null) return;

        Vector3 dirToPlayer = player.position - transform.position;
        float distance = dirToPlayer.magnitude;
        float angle = Vector3.Angle(transform.forward, dirToPlayer);

        if (distance <= detectionRange && angle <= fieldOfView / 2)
        {
            if (Physics.Linecast(transform.position, player.position, out RaycastHit hit))
            {
                if (hit.collider.gameObject == player.gameObject)
                {
                    playerDetected = true;
                    OnPlayerDetected();
                }
            }
        }
        else
        {
            playerDetected = false;
            OnPlayerLost();
        }
    }

    protected virtual void OnPlayerDetected() { }
    protected virtual void OnPlayerLost() { }

    protected void RestartScene()
    {
        SceneManager.LoadScene(SceneManager.GetActiveScene().name);
    }

    public virtual void StealthTakedown()
{
    PlayerStealthKill killHandler = player.GetComponent<PlayerStealthKill>();
    if (killHandler != null)
    {
        killHandler.ExecuteStealthKill(this);
    }
}


    private IEnumerator PerformTakedown()
    {
        Debug.Log($"{gameObject.name} stealth takedown initiated.");

        enabled = false; // Disable enemy AI

        PlayerMovement playerMovement = player.GetComponent<PlayerMovement>();
        MouseLook mouseLook = player.GetComponentInChildren<MouseLook>();

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
            else
            {
                Debug.LogWarning("Assigned dagger has no Animator.");
                yield return new WaitForSeconds(1f);
            }

            daggerObject.SetActive(false);
        }
        else
        {
            Debug.LogWarning("Dagger object not assigned in Inspector.");
            yield return new WaitForSeconds(1f);
        }

        Destroy(gameObject); // Kill the enemy

        if (playerMovement != null) playerMovement.canMove = true;
        if (mouseLook != null) mouseLook.FreezeLook(false);
    }

    public void Interact()
{
    Vector3 toPlayer = player.position - transform.position;
    float angle = Vector3.Angle(transform.forward, toPlayer);
    float distance = toPlayer.magnitude;

    PlayerMovement playerMovement = player.GetComponent<PlayerMovement>();

    if (angle > 100f && distance < 2f && playerMovement != null && playerMovement.IsCrouching)
    {
        StealthTakedown();
    }
    else
    {
        Debug.Log("Takedown failed: Too visible, too far, or not crouching.");
    }
}

}

