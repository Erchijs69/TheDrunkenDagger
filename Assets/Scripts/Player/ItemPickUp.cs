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
    private List<GameObject> placedIngredients = new List<GameObject>();

    void Start()
    {
        ghostMaterial = new Material(Shader.Find("Standard"));
        ghostMaterial.color = new Color(1f, 1f, 1f, 0.3f);

        holdAnimator = holdPosition.GetComponent<Animator>();
    }

    private void Update()
    {
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

            // Check if the item is already placed in the potion system
            if (placedIngredients.Contains(item))
            {
                Debug.Log("Cannot pick up placed ingredient.");
                return; // Do nothing if the ingredient is already placed
            }

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

        // If it's an ingredient, mark it as placed
        if (heldItem.CompareTag("Ingredient"))
        {
            placedIngredients.Add(heldItem);
        }

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
        Liquid liquid = heldItem.GetComponentInChildren<Liquid>();
        if (liquid != null && liquid.gameObject.activeSelf && liquid.fillAmount >= 0.5f)
        {
            isDrinking = true;
            Debug.Log("Drinking the potion...");

            if (holdAnimator != null)
            {
                holdAnimator.SetTrigger("Drink");
            }

            // Gradually increase the potion's fill amount while drinking
            while (liquid.fillAmount < 1f)
            {
                liquid.SetFillAmount(liquid.fillAmount + 0.03f); // Increase the fill amount gradually
                yield return new WaitForSeconds(0.1f); // Slow down the drinking process
            }

            // Ensure the fill amount doesn't exceed 1
            liquid.SetFillAmount(1f);

            // Wait for the animation to finish
            yield return new WaitForSeconds(3f); // Adjust as necessary for the drink animation

            // Destroy the held item after drinking
            Destroy(heldItem);
            heldItem = null;
            isDrinking = false;
        }
        else
        {
            Debug.Log("Potion is not ready to be drunk!");
        }
    }
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













