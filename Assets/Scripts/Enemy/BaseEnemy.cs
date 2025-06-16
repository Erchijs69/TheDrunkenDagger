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

    [Header("Hearing Settings")]
    public float hearingRadius = 20f;
    public Color hearingRadiusColor = Color.cyan;

    public GameObject hearingVisual;

    private PlayerBuffs playerBuffs;
    public string waterLayerName = "Water";

    protected bool playerDetected = false;

    public bool PlayerDetected => playerDetected;

    protected float lastSeenTime;

    protected PlayerMovement playerMovement;

    protected bool isDead = false;
    public bool IsDead => isDead;


    protected virtual void Start()
    {
        if (player != null)
        {
            playerMovement = player.GetComponent<PlayerMovement>();
        }

        playerBuffs = FindObjectOfType<PlayerBuffs>();

        if (hearingVisual != null)
            hearingVisual.SetActive(false);
    }

    protected virtual void Update()
    {
        DetectPlayer();

        if (playerBuffs != null && playerBuffs.isMasterAssassinActive)
        {
            if (hearingVisual != null && !hearingVisual.activeSelf)
                hearingVisual.SetActive(true);
        }
        else
        {
            if (hearingVisual != null && hearingVisual.activeSelf)
                hearingVisual.SetActive(false);
        }
    }

    protected virtual void DetectPlayer()
    {
        if (player == null || playerMovement == null) return;


        if (playerBuffs != null && playerBuffs.isTinyTinasCurseActive)
        {
            playerDetected = false;
            return;
        }

        if (playerMovement.IsStealthed)
        {
            playerDetected = false;
            return;
        }

        Vector3 dirToPlayer = player.position - transform.position;
        float distance = dirToPlayer.magnitude;
        float angle = Vector3.Angle(transform.forward, dirToPlayer);

        int waterLayer = LayerMask.NameToLayer(waterLayerName);
        int ignoreMask = ~(1 << waterLayer);

        bool inSightRange = distance <= detectionRange && angle <= fieldOfView / 2;
        bool heard = false;

        if (inSightRange)
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

        // Updated Hearing Logic
        if (!playerMovement.IsCrouching && distance <= hearingRadius)
        {
            // Only allow hearing detection if the player is moving or hasn't just stood up
            if (!playerMovement.JustStoodUp || playerMovement.IsMoving)
            {
                heard = true;
            }
        }

        if (heard)
        {
            playerDetected = true;
            OnPlayerDetected();
            return;
        }

        playerDetected = false;
        OnPlayerLost();
    }


    public bool CanAssassinate()
    {
        return !playerDetected && !(playerBuffs != null && playerBuffs.isTinyTinasCurseActive);
    }

    protected virtual void OnPlayerDetected() { }
    protected virtual void OnPlayerLost() { }

    protected void RestartScene()
    {
        SceneManager.LoadScene(SceneManager.GetActiveScene().name);
    }

    public virtual void StealthTakedown()
    {
        if (playerDetected)
        {
            Debug.Log("Cannot perform takedown: enemy is alert.");
            return;
        }

        if (playerBuffs != null && playerBuffs.isTinyTinasCurseActive)
        {
            Debug.Log("Cannot perform takedown: Tiny Tina's Curse is active.");
            return;
        }

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
        isDead = true;
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

        if (angle > 100f && distance < 2f && playerMovement.IsCrouching && !playerDetected)
        {
            StealthTakedown();
        }
        else
        {
            Debug.Log("Takedown failed: Detected, too visible, too far, or not crouching.");
        }
    }


    protected virtual void OnDrawGizmos()
    {
        Gizmos.color = detectionRangeColor;
        Gizmos.DrawWireSphere(transform.position, detectionRange);

        // Only show the hearing gizmo if the player has Master Assassin buff
        if (player != null && HasMasterAssassinBuff())
        {
            Gizmos.color = hearingRadiusColor;
            DrawFlatCircle(transform.position, hearingRadius);
        }

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

    private void DrawFlatCircle(Vector3 position, float radius, int segments = 40)
    {
        Vector3 prevPoint = position + Vector3.right * radius;
        for (int i = 1; i <= segments; i++)
        {
            float angle = i * Mathf.PI * 2 / segments;
            Vector3 newPoint = position + new Vector3(Mathf.Cos(angle), 0, Mathf.Sin(angle)) * radius;
            Gizmos.DrawLine(prevPoint, newPoint);
            prevPoint = newPoint;
        }
    }

    private bool HasMasterAssassinBuff()
    {
        if (player == null) return false;

        PlayerBuffs buffs = player.GetComponent<PlayerBuffs>();
        if (buffs == null) return false;

        return buffs.isMasterAssassinActive;
    }
    
    protected void CheckPlayerDeath(Collider other)
{
    if (other.CompareTag("Player"))
    {
        PlayerMovement playerMovement = other.GetComponent<PlayerMovement>();
        if (playerMovement != null && !playerMovement.IsStealthed && !playerMovement.IsSmall)
        {
            GameManager.Managerinstance?.PlayerDied();
        }
    }
}


}






