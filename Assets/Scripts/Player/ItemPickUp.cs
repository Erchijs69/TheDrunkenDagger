using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ItemPickup : MonoBehaviour
{
    public Transform holdPosition;
    public LayerMask itemLayer;
    public LayerMask surfaceLayer;
    public float pickupRange = 3f;
    public float yOffset = 0.5f;
    public float maxAngle = 30f;
    public float placementHeightOffset = 0.1f;
    public float maxPlaceDistance = 5f;

    private GameObject heldItem;
    private Rigidbody heldItemRb;
    private Collider itemCollider;

    private GameObject ghostItem;
    private Material ghostMaterial;

    private Vector3 originalItemPosition;

    private Animator holdAnimator;
    private bool isDrinking = false;

    public PlayerMovement playerMovement;

    public PotionSystem potionSystem;

    public PlayerBuffs playerBuffs; // Drag Player in Inspector or find in Start()

    void Start()
    {
        ghostMaterial = new Material(Shader.Find("Standard"));
        ghostMaterial.color = new Color(1f, 1f, 1f, 0.3f);

        holdAnimator = holdPosition.GetComponent<Animator>();

        if (potionSystem == null)
        potionSystem = FindObjectOfType<PotionSystem>(); // Automatically find the PotionSystem
    }

    private void Update()
    {
        if (playerMovement != null && playerMovement.IsStealthed)
        {
            return; // Don't allow item pickup if stealth mode is active
        }

        if (isDrinking)
        {
            if (ghostItem != null)
                ghostItem.SetActive(false); // hide ghost while drinking
            return; // skip all input while drinking
        }

        if (Input.GetKeyDown(KeyCode.E))
        {
            if (heldItem == null)
                TryPickupItem();
            else
                TryPlaceItem();
        }

        if (Input.GetKeyDown(KeyCode.R) && heldItem != null)
        {
            StartCoroutine(DrinkPotion());
        }

        if (heldItem != null)
            UpdateGhostItemPosition();
    }

    void TryPickupItem()
    {
        if (heldItem != null) return; // Don't pick up if already holding

        Ray ray = Camera.main.ScreenPointToRay(new Vector3(Screen.width / 2, Screen.height / 2));
        RaycastHit hit;

        if (Physics.Raycast(ray, out hit, pickupRange, itemLayer))
{
    GameObject item = hit.collider.gameObject;

    // Remove this line:
    // GameObject item = hit.collider.gameObject;

    if (!hit.collider.isTrigger) 
    {
        // Use the already declared 'item'
        heldItem = item;
        heldItemRb = heldItem.GetComponent<Rigidbody>();
        itemCollider = heldItem.GetComponent<Collider>();

        heldItem.SetActive(true);
        heldItemRb.isKinematic = true;
        heldItem.transform.SetParent(holdPosition);
        heldItem.transform.localPosition = Vector3.zero;
        heldItem.transform.localRotation = Quaternion.identity;

        originalItemPosition = heldItem.transform.position;

        CreateGhostItem();
    }
}

    }

    void CreateGhostItem()
    {
        ghostItem = Instantiate(heldItem, heldItem.transform.position, heldItem.transform.rotation);
        Renderer ghostRenderer = ghostItem.GetComponent<Renderer>();
        if (ghostRenderer != null)
        {
            ghostRenderer.material = ghostMaterial;
        }

        Collider[] ghostColliders = ghostItem.GetComponentsInChildren<Collider>();
        foreach (Collider col in ghostColliders)
        {
            col.enabled = false;
        }

        ghostItem.SetActive(false);
    }

    void UpdateGhostItemPosition()
    {
        if (ghostItem == null) return;

        RaycastHit hit;
        if (Physics.Raycast(Camera.main.ScreenPointToRay(Input.mousePosition), out hit, 100f, surfaceLayer))
        {
            Vector3 surfaceNormal = hit.normal;

            if (Vector3.Angle(surfaceNormal, Vector3.up) <= maxAngle)
            {
                Vector3 targetPosition = hit.point - (Vector3.up * yOffset);
                ghostItem.transform.position = targetPosition;
                ghostItem.transform.rotation = Quaternion.identity;
                ghostItem.SetActive(true);
            }
            else
            {
                ghostItem.SetActive(false);
            }
        }
        else
        {
            ghostItem.SetActive(false);
        }

        float distanceToPlayer = Vector3.Distance(transform.position, ghostItem.transform.position);
        if (distanceToPlayer > maxPlaceDistance)
        {
            ghostItem.SetActive(false);
        }
    }

    void TryPlaceItem()
    {
        if (ghostItem == null || !ghostItem.activeSelf)
        {
            Debug.Log("Invalid Placement - Ghost Item Not Active");
            return;
        }

        float distanceToPlayer = Vector3.Distance(transform.position, ghostItem.transform.position);
        if (distanceToPlayer > maxPlaceDistance)
        {
            Debug.Log("Placement too far from player.");
            return;
        }

        heldItem.transform.SetParent(null);
        heldItem.transform.position = ghostItem.transform.position;
        heldItem.transform.rotation = Quaternion.identity;

        heldItemRb.isKinematic = false;

        Destroy(ghostItem);

        Debug.Log("Item placed at: " + heldItem.transform.position);

        heldItem = null;
    }

    public void SpawnItemToHand(GameObject item)
    {
        if (heldItem != null)
        {
            Debug.LogWarning("Already holding an item!");
            return;
        }

        heldItem = item;
        heldItemRb = heldItem.GetComponent<Rigidbody>();
        itemCollider = heldItem.GetComponent<Collider>();

        heldItem.SetActive(true);
        heldItemRb.isKinematic = true;
        heldItem.transform.SetParent(holdPosition);
        heldItem.transform.localPosition = Vector3.zero;
        heldItem.transform.localRotation = Quaternion.identity;

        CreateGhostItem();
        Debug.Log("Spawned item to hand: " + item.name);
    }

    public bool IsHoldingItem()
    {
        return heldItem != null;
    }

    private IEnumerator DrinkPotion()
{
    if (heldItem != null)
    {
        // Check if the held item is a potion and if it's not already consumed
        Potion potion = heldItem.GetComponent<Potion>();
        if (potion != null && !potion.HasBeenConsumed)
        {
            Liquid liquid = heldItem.GetComponentInChildren<Liquid>();
            if (liquid != null && liquid.gameObject.activeSelf && liquid.fillAmount >= 0.5f)
            {
                isDrinking = true; // Mark that we are drinking the potion

                Debug.Log("Drinking the potion...");

                // Show the effect name UI before drinking
                potion.ShowEffectName();

                // Start the drinking animation (if available)
                if (holdAnimator != null)
                {
                    holdAnimator.SetTrigger("Drink");
                }

                // Gradually increase the potion's fill amount while drinking
                while (liquid.fillAmount < 1f)
                {
                    liquid.SetFillAmount(liquid.fillAmount + 0.03f); // Gradually fill the potion
                    yield return new WaitForSeconds(0.1f); // Slow the drinking process
                }

                // Ensure the fill amount doesn't exceed 1
                liquid.SetFillAmount(1f);

                // Wait for the animation to finish (3 seconds or adjust as necessary)
                yield return new WaitForSeconds(3f);

                // Apply the potion effect after drinking
                ApplyPotionEffect(potion);

                // Hide the effect name UI after drinking
                potion.HideEffectName();

                // Mark the potion as consumed
                potion.Consume();

                // Destroy the held item after drinking
                Destroy(heldItem);
                heldItem = null;

                isDrinking = false; // Set the drinking flag back to false
            }
            else
            {
                Debug.Log("Potion is not ready to be drunk! Make sure it's filled.");
            }
        }
        else
        {
            Debug.Log("Potion is either missing or has already been consumed.");
        }
    }
}


private void ApplyPotionEffect(Potion potion)
{
    if (playerBuffs != null)
        playerBuffs.ApplyBuff(potion.potionEffectName);
    else
        Debug.LogWarning("PlayerBuffs reference is missing!");
}

    public bool IsDrinking()
    {
        return isDrinking;
    }

    private void DrainLiquid()
    {
        if (heldItem != null)
        {
            Liquid liquid = heldItem.GetComponentInChildren<Liquid>();
            if (liquid != null)
            {
                liquid.SetFillAmount(1f); // We'll add this SetFillAmount function to the Liquid script
            }
        }
    }

}













