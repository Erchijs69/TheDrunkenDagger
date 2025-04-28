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
    

   void Update()
{
    bool canTakedown = false;
    Ray ray = new Ray(InteractorSource.position + InteractorSource.forward * 0.5f, InteractorSource.forward);
    RaycastHit hit;
    Potion hitPotion = null;

    // Raycast to detect interactable objects
    if (Physics.Raycast(ray, out hit, InteractRange, interactableLayers))
    {
        currentHitObject = hit.collider.gameObject;
        IInteractable interactable = currentHitObject.GetComponent<IInteractable>();

        // Handle enemy stealth takedown logic (if applicable)
        if (interactable is BaseEnemy enemy)
        {
            Vector3 toPlayer = enemy.player.position - enemy.transform.position;
            float angle = Vector3.Angle(enemy.transform.forward, toPlayer);
            float distance = Vector3.Distance(enemy.transform.position, enemy.player.position);

            PlayerMovement playerMovement = enemy.player.GetComponent<PlayerMovement>();
            if (angle > 100f && distance < 2f && playerMovement != null && playerMovement.IsCrouching)
            {
                canTakedown = true;
            }
        }

        // Handle interaction if the player presses 'E'
        if (Input.GetKeyDown(KeyCode.E) && interactable != null)
        {
            interactable.Interact();
            Debug.Log($"Interacted with {currentHitObject.name}");
        }

        // Check if the hit object is a potion
        hitPotion = currentHitObject.GetComponent<Potion>();
    }

    // Handle potion hover effect
    if (hitPotion != currentPotion)
    {
        // Hide the previous potion effect name
        if (currentPotion != null)
            currentPotion.HideEffectName();

        // Update the current potion
        currentPotion = hitPotion;

        // Show the new potion's effect name if we are hovering over a potion
        if (currentPotion != null)
            currentPotion.ShowEffectName();
    }

    // Update stealth prompt UI visibility based on conditions
    if (stealthPromptUI != null)
        stealthPromptUI.SetActive(canTakedown);

    // Handle item pickup logic if the inventory is not full
    if (InventoryManager.Instance.ItemCount >= InventoryManager.Instance.maxInventorySlots)
        return;

    // Don't allow pickup if already holding an item
    if (InventoryManager.Instance.itemPickup != null && InventoryManager.Instance.itemPickup.IsHoldingItem())
        return;

    if (!InventoryManager.Instance.IsInventoryOpen)
    {
        if (Input.GetKeyDown(KeyCode.Q))
        {
            Ray itemRay = new Ray(InteractorSource.position + InteractorSource.forward * 0.5f, InteractorSource.forward);
            RaycastHit itemHit;

            if (Physics.Raycast(itemRay, out itemHit, InteractRange, itemLayer))
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









