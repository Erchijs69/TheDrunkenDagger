using UnityEngine;
using UnityEngine.SceneManagement;
using System.Collections;
using Unity.AI.Navigation.Samples;
using UnityEngine.UI;

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

    private OutlineEffect currentOutlinedItem;

    private PlayerBuffs playerBuffs;

    public ElfMovement elfShooter;


    public float shootRange = 200f;

    public float shootCooldown = 6f;
    private bool canShoot = true;


    



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


        if (Input.GetKeyDown(KeyCode.F) && canShoot)
        {
            Ray shootRay = new Ray(InteractorSource.position + InteractorSource.forward * 0.5f, InteractorSource.forward);

            if (Physics.Raycast(shootRay, out RaycastHit shootHit, shootRange, interactableLayers))
            {
                BaseEnemy targetEnemy = shootHit.collider.GetComponent<BaseEnemy>();

                if (targetEnemy != null)
                {
                    float distanceToEnemy = Vector3.Distance(transform.position, targetEnemy.transform.position);

                    if (distanceToEnemy <= shootRange)
                    {
                        Debug.Log("Enemy detected by interactor raycast.");

                        elfShooter.LookAtTarget(targetEnemy.transform);

                        canShoot = false;  // disable shooting until cooldown expires
                        StartCoroutine(DelayedShoot(targetEnemy, 2f)); // shoot after delay
                        StartCoroutine(ShootCooldownRoutine());
                    }
                    else
                    {
                        Debug.Log($"Enemy is too far to shoot (distance: {distanceToEnemy}, max range: {shootRange}).");
                    }
                }
                else
                {
                    Debug.Log("Raycast hit something, but it's not an enemy.");
                }
            }
            else
            {
                Debug.Log("No valid target hit within arrow shoot range.");
            }
        }
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

        
        if (stealthPromptUI != null)
            stealthPromptUI.SetActive(canTakedown);

       
        if (isStealthed)
            return;

        if (InventoryManager.Instance.itemPickup != null && InventoryManager.Instance.itemPickup.IsHoldingItem())
            return;

        
        if (!InventoryManager.Instance.IsInventoryOpen)
        {
            Ray itemRay = new Ray(InteractorSource.position + InteractorSource.forward * 0.5f, InteractorSource.forward);
            RaycastHit itemHit;
            bool hitItem = Physics.Raycast(itemRay, out itemHit, InteractRange, itemLayer);

            if (hitItem)
            {
                var outlineEffect = itemHit.collider.GetComponent<OutlineEffect>();

               
                if (outlineEffect != null && itemHit.collider.gameObject.activeInHierarchy)
                {
                    if (currentOutlinedItem != outlineEffect)
                    {
                        if (currentOutlinedItem != null)
                            currentOutlinedItem.DisableOutline();

                        currentOutlinedItem = outlineEffect;
                        currentOutlinedItem.EnableOutline();
                    }
                }
                else
                {
                    if (currentOutlinedItem != null)
                    {
                        currentOutlinedItem.DisableOutline();
                        currentOutlinedItem = null;
                    }
                }
            }
            else
            {
                if (currentOutlinedItem != null)
                {
                    currentOutlinedItem.DisableOutline();
                    currentOutlinedItem = null;
                }
            }

            // PICKUP ITEM ON Q PRESS
            if (Input.GetKeyDown(KeyCode.Q))
            {
                if (InventoryManager.Instance.ItemCount >= InventoryManager.Instance.maxInventorySlots)
                {
                    InventoryManager.Instance.ShowInventoryFullMessage();
                    return;
                }

                if (hitItem)
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
            Gizmos.color = Color.red;
            Gizmos.DrawWireSphere(transform.position, shootRange);

        }
    }

    private IEnumerator DelayedShoot(BaseEnemy target, float delaySeconds)
    {
        yield return new WaitForSeconds(delaySeconds);

        if (elfShooter != null && target != null)
        {
            elfShooter.ShootAtTarget(target.transform);
            Debug.Log($"ElfShooter shot arrow at {target.name} after {delaySeconds} seconds delay.");
        }
    }

private IEnumerator ShootCooldownRoutine()
{
    yield return new WaitForSeconds(shootCooldown);
    canShoot = true; 
}

}


