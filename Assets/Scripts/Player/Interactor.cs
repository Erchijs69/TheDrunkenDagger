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

    // Direct reference to PlayerMovement
    private PlayerMovement playerMovement;

    void Start()
    {
        // Assuming PlayerMovement is on the same GameObject
        playerMovement = GetComponent<PlayerMovement>();
    }

    void Update()
    {
        bool canTakedown = false;
        Ray ray = new Ray(InteractorSource.position + InteractorSource.forward * 0.5f, InteractorSource.forward);
        RaycastHit hit;
        Potion hitPotion = null;

        // Only allow interactions when not stealthed
        if (playerMovement != null && playerMovement.IsStealthed)
        {
            stealthPromptUI.SetActive(false); // Optionally hide stealth prompt
            return; // Exit early to prevent interactions in stealth mode
        }

        // Raycast to detect interactable objects
        if (Physics.Raycast(ray, out hit, InteractRange, interactableLayers))
        {
            currentHitObject = hit.collider.gameObject;
            IInteractable interactable = currentHitObject.GetComponent<IInteractable>();

            // Stealth detection (this part remains unchanged)
            if (interactable is BaseEnemy enemy)
            {
                Vector3 toPlayer = enemy.player.position - enemy.transform.position;
                float angle = Vector3.Angle(enemy.transform.forward, toPlayer);
                float distance = Vector3.Distance(enemy.transform.position, enemy.player.position);

                playerMovement = enemy.player.GetComponent<PlayerMovement>();

                // Check if in stealth mode
                if (playerMovement != null && playerMovement.IsStealthed)
                {
                    canTakedown = true;
                }
            }

            if (Input.GetKeyDown(KeyCode.E) && interactable != null)
            {
                interactable.Interact();
                Debug.Log($"Interacted with {currentHitObject.name}");
            }

            // Check if it's a potion
            hitPotion = currentHitObject.GetComponent<Potion>();
        }

        // Potion distance handling
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

        // Show stealth prompt
        if (stealthPromptUI != null)
            stealthPromptUI.SetActive(canTakedown);

        // Item pickup (only allow if not stealthed)
        if (InventoryManager.Instance.ItemCount >= InventoryManager.Instance.maxInventorySlots || (playerMovement != null && playerMovement.IsStealthed))
            return;

        if (InventoryManager.Instance.itemPickup != null && InventoryManager.Instance.itemPickup.IsHoldingItem())
            return;

        if (!InventoryManager.Instance.IsInventoryOpen)
        {
            if (Input.GetKeyDown(KeyCode.Q))
            {
                Ray itemRay = new Ray(InteractorSource.position + InteractorSource.forward * 0.5f, InteractorSource.forward);
                if (Physics.Raycast(itemRay, out RaycastHit itemHit, InteractRange, itemLayer))
                {
                    GameObject itemObject = itemHit.collider.gameObject;
                    InventoryManager.Instance.AddItem(itemObject);
                    itemObject.SetActive(false); // Disable instead of destroy
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
