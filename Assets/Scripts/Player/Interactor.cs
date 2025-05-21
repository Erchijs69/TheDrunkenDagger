using UnityEngine;
using UnityEngine.SceneManagement;
using System.Collections;

interface IInteractable
{
    void Interact();
}

public class Interactor : MonoBehaviour
{
    public Transform InteractorSource;
    public float InteractRange = 3f;
    public LayerMask interactableLayers;
    public LayerMask itemLayer;
    public GameObject stealthPromptUI;

    private GameObject currentHitObject;
    private Potion currentPotion;

    private PlayerMovement playerMovement;

    public LayerMask cartLayer;

    private PlayerBuffs playerBuffs;

    


    [Header("Stealth Settings")]
    public float stealthTakedownDistance = 2f;


    void Start()
    {
        playerMovement = GetComponent<PlayerMovement>();
        playerBuffs = GetComponent<PlayerBuffs>();
    }

    void Update()
    {
        bool canTakedown = false;
        bool isPlayerTiny = playerBuffs != null && playerBuffs.isTinyTinasCurseActive;
        bool isPlayerDetected = false;

        Ray ray = new Ray(InteractorSource.position + InteractorSource.forward * 0.5f, InteractorSource.forward);
        RaycastHit hit;
        Potion hitPotion = null;

        bool isStealthed = playerMovement != null && playerMovement.IsStealthed;

        // Early exit if stealthed or tiny, hide prompt
        if (isStealthed || isPlayerTiny)
        {
            if (stealthPromptUI != null)
                stealthPromptUI.SetActive(false);
            return;
        }

        if (Physics.Raycast(ray, out hit, InteractRange, interactableLayers))
        {
            currentHitObject = hit.collider.gameObject;
            IInteractable interactable = currentHitObject.GetComponent<IInteractable>();

            if (interactable is BaseEnemy enemy)
            {
                Vector3 toPlayer = enemy.player.position - enemy.transform.position;
                float angle = Vector3.Angle(enemy.transform.forward, toPlayer);
                float distance = Vector3.Distance(enemy.transform.position, enemy.player.position);

                playerMovement = enemy.player.GetComponent<PlayerMovement>();
                isStealthed = playerMovement != null && playerMovement.IsStealthed;

                 isPlayerDetected = enemy.PlayerDetected;

                if (!isStealthed && !isPlayerDetected && !isPlayerTiny && distance <= stealthTakedownDistance)
                {
                    canTakedown = true;
                }
            }

            if (Input.GetKeyDown(KeyCode.E) && interactable != null)
            {
                interactable.Interact();
                Debug.Log($"Interacted with {currentHitObject.name}");
            }

            hitPotion = currentHitObject.GetComponent<Potion>();
        }

        if (hitPotion != null && Vector3.Distance(transform.position, hitPotion.transform.position) <= InteractRange)
        {
            if (currentPotion != hitPotion)
            {
                if (currentPotion != null)
                    currentPotion.HideEffectName();

                currentPotion = hitPotion;
                currentPotion.ShowEffectName();
            }
        }
        else if (currentPotion != null)
        {
            currentPotion.HideEffectName();
            currentPotion = null;
        }

        // Show stealth prompt only if takedown is allowed, player is NOT detected or tiny or stealthed
        if (stealthPromptUI != null)
            stealthPromptUI.SetActive(canTakedown);

        // Early return if stealthed
        if (isStealthed)
            return;

        if (InventoryManager.Instance.itemPickup != null && InventoryManager.Instance.itemPickup.IsHoldingItem())
            return;

        if (!InventoryManager.Instance.IsInventoryOpen)
        {
            if (Input.GetKeyDown(KeyCode.Q))
            {
                if (InventoryManager.Instance.ItemCount >= InventoryManager.Instance.maxInventorySlots)
                {
                    InventoryManager.Instance.ShowInventoryFullMessage();
                    return;
                }

                Ray itemRay = new Ray(InteractorSource.position + InteractorSource.forward * 0.5f, InteractorSource.forward);
                if (Physics.Raycast(itemRay, out RaycastHit itemHit, InteractRange, itemLayer))
                {
                    GameObject itemObject = itemHit.collider.gameObject;
                    InventoryManager.Instance.AddItem(itemObject);
                    itemObject.SetActive(false);
                    Debug.Log($"Picked up {itemObject.name} and added to inventory.");
                }
            }
        }
    


    }

    void OnDrawGizmos()
    {
        if (InteractorSource != null)
        {
            Gizmos.color = Color.green;
            Gizmos.DrawWireSphere(InteractorSource.position, InteractRange);
        }
    }
}

