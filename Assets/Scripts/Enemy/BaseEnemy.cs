using UnityEngine;
using UnityEngine.SceneManagement;
using System.Collections;

public abstract class BaseEnemy : MonoBehaviour, IInteractable
{
    public Transform player;
    public GameObject daggerObject;
    public float detectionRange = 10f;
    public float fieldOfView = 120f;

    [Header("Detection Gizmos")]
    public Color detectionRangeColor = Color.green;
    public Color fieldOfViewColor = Color.yellow;

    public string waterLayerName = "Water";

    protected bool playerDetected = false;
    protected float lastSeenTime;

    protected PlayerMovement playerMovement;

    protected virtual void Start()
    {
        if (player != null)
        {
            playerMovement = player.GetComponent<PlayerMovement>();
        }
    }

    protected virtual void Update()
    {
        DetectPlayer();
    }

    protected virtual void DetectPlayer()
{
    if (player == null || playerMovement == null) return;

    // Ignore player if stealthed OR small
    if (playerMovement.IsStealthed || playerMovement.IsSmall)
    {
        playerDetected = false;
        OnPlayerLost();
        return;
    }

    Vector3 dirToPlayer = player.position - transform.position;
    float distance = dirToPlayer.magnitude;
    float angle = Vector3.Angle(transform.forward, dirToPlayer);

    int waterLayer = LayerMask.NameToLayer(waterLayerName);
    int ignoreMask = ~(1 << waterLayer);

    if (distance <= detectionRange && angle <= fieldOfView / 2)
    {
        if (Physics.Linecast(transform.position, player.position, out RaycastHit hit, ignoreMask))
        {
            if (hit.collider.gameObject == player.gameObject)
            {
                playerDetected = true;
                OnPlayerDetected();
                return;
            }
        }
    }

    playerDetected = false;
    OnPlayerLost();
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

        enabled = false;

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

        Destroy(gameObject);
        if (playerMovement != null) playerMovement.canMove = true;
        if (mouseLook != null) mouseLook.FreezeLook(false);
    }

    public void Interact()
    {
        if (player == null || playerMovement == null) return;

        Vector3 toPlayer = player.position - transform.position;
        float angle = Vector3.Angle(transform.forward, toPlayer);
        float distance = toPlayer.magnitude;

        if (angle > 100f && distance < 2f && playerMovement.IsCrouching)
        {
            StealthTakedown();
        }
        else
        {
            Debug.Log("Takedown failed: Too visible, too far, or not crouching.");
        }
    }

    protected virtual void OnDrawGizmos()
    {
        Gizmos.color = detectionRangeColor;
        Gizmos.DrawWireSphere(transform.position, detectionRange);

        Gizmos.color = fieldOfViewColor;
        Vector3 leftAngle = Quaternion.Euler(0, -fieldOfView / 2, 0) * transform.forward * detectionRange;
        Vector3 rightAngle = Quaternion.Euler(0, fieldOfView / 2, 0) * transform.forward * detectionRange;
        Gizmos.DrawLine(transform.position, transform.position + leftAngle);
        Gizmos.DrawLine(transform.position, transform.position + rightAngle);

        if (player != null)
        {
            Gizmos.color = Color.red;
            Gizmos.DrawLine(transform.position, player.position);
        }
    }
}






