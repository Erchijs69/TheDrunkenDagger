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
    public float placementHeightOffset = 0.1f; // Adjustable height offset above ghost
    public float maxPlaceDistance = 5f; // Maximum distance the item can be placed from the player

    private GameObject heldItem;
    private Rigidbody heldItemRb;
    private Collider itemCollider;

    private GameObject ghostItem;  
    private Material originalMaterial;  
    private Material ghostMaterial;    

    private Vector3 originalItemPosition; // Track the original position of the item

    void Start()
    {
        // Create the ghost material
        ghostMaterial = new Material(Shader.Find("Standard"));
        ghostMaterial.color = new Color(1f, 1f, 1f, 0.3f);  
    }

    void Update()
    {
        if (Input.GetKeyDown(KeyCode.E))
        {
            if (heldItem == null)
                TryPickupItem();
            else
                TryPlaceItem();
        }

        if (heldItem != null)
            UpdateGhostItemPosition();
    }

    void TryPickupItem()
    {
        RaycastHit hit;
        if (Physics.Raycast(transform.position, transform.forward, out hit, pickupRange, itemLayer))
        {
            heldItem = hit.collider.gameObject;
            heldItemRb = heldItem.GetComponent<Rigidbody>();
            itemCollider = heldItem.GetComponent<Collider>();

            heldItemRb.isKinematic = true;
            heldItem.transform.SetParent(holdPosition);
            heldItem.transform.localPosition = Vector3.zero;
            heldItem.transform.localRotation = Quaternion.identity;

            // Track the original position of the item for placement distance checks
            originalItemPosition = heldItem.transform.position;

            // Create the ghost item for preview
            CreateGhostItem();
        }
    }

    void CreateGhostItem()
    {
        ghostItem = Instantiate(heldItem, heldItem.transform.position, heldItem.transform.rotation);
        ghostItem.GetComponent<Renderer>().material = ghostMaterial; // Apply ghost material

        // Disable any colliders on the ghost object
        Collider[] ghostColliders = ghostItem.GetComponentsInChildren<Collider>();
        foreach (Collider col in ghostColliders)
        {
            col.enabled = false;
        }

        ghostItem.SetActive(false); // Make it inactive initially
    }

    void UpdateGhostItemPosition()
    {
        RaycastHit hit;
        if (Physics.Raycast(Camera.main.ScreenPointToRay(Input.mousePosition), out hit, 100f, surfaceLayer))
        {
            Vector3 surfaceNormal = hit.normal;

            if (Vector3.Angle(surfaceNormal, Vector3.up) <= maxAngle)
            {
                ghostItem.SetActive(true);
                Vector3 targetPosition = hit.point - (Vector3.up * yOffset);  
                ghostItem.transform.position = targetPosition;
                ghostItem.transform.rotation = Quaternion.identity; // Keep upright
            }
            else
            {
                ghostItem.SetActive(false);
            }
        }
        else
        {
            // If no surface is detected by the raycast, hide the ghost item
            ghostItem.SetActive(false);
        }

        // Check if the ghost item is too far from the player
        float distanceToPlayer = Vector3.Distance(transform.position, ghostItem.transform.position);
        if (distanceToPlayer > maxPlaceDistance)
        {
            ghostItem.SetActive(false); // Hide ghost item if it's too far from the player
        }
    }

    void TryPlaceItem()
    {
        // Ensure the item is within the maximum placement distance from the player
        float distanceToPlayer = Vector3.Distance(transform.position, ghostItem.transform.position);
        
        if (distanceToPlayer > maxPlaceDistance)
        {
            Debug.Log("Placement too far from player.");
            return; // Do not allow placement if it's too far
        }

        if (ghostItem.activeSelf)  
        {
            heldItem.transform.SetParent(null);
            heldItem.transform.position = ghostItem.transform.position + (Vector3.up * placementHeightOffset);
            heldItem.transform.rotation = Quaternion.identity; // Keep upright

            // Make the item interactable with physics again
            heldItemRb.isKinematic = false;

            // Disable the ghost and finalize placement
            Destroy(ghostItem);
            heldItem = null;

            Debug.Log("Item placed at: " + heldItem.transform.position);
        }
        else
        {
            Debug.Log("Invalid Placement - Ghost Item Not Active");
        }
    }
}

