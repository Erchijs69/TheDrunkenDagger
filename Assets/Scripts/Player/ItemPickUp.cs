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

    private GameObject heldItem;
    private Rigidbody heldItemRb;
    private Collider itemCollider;

    private GameObject ghostItem;  
    private Material originalMaterial;  
    private Material ghostMaterial;    

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

            if (heldItem.GetComponent<Renderer>())
                originalMaterial = heldItem.GetComponent<Renderer>().material;

            CreateGhostItem();
        }
    }

    void CreateGhostItem()
    {
        ghostItem = Instantiate(heldItem, heldItem.transform.position, heldItem.transform.rotation);
        ghostItem.GetComponent<Renderer>().material = ghostMaterial;  
        ghostItem.SetActive(false);
    }

    void UpdateGhostItemPosition()
    {
        RaycastHit hit;
        if (Physics.Raycast(Camera.main.ScreenPointToRay(Input.mousePosition), out hit, 100f, surfaceLayer))
        {
            Vector3 surfaceNormal = hit.normal;

            Debug.DrawRay(hit.point, surfaceNormal, Color.green, 0.1f);
            Debug.Log("Surface Normal Angle: " + Vector3.Angle(surfaceNormal, Vector3.up));

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
    }

    void TryPlaceItem()
    {
        if (ghostItem.activeSelf)  
        {
            heldItem.transform.SetParent(null);
            heldItem.transform.position = ghostItem.transform.position + (Vector3.up * placementHeightOffset);
            heldItem.transform.rotation = Quaternion.identity; // Keep upright

            Destroy(ghostItem);
            heldItemRb.isKinematic = false;
            heldItem = null;

            Debug.Log("Item placed at: " + heldItem.transform.position);
        }
        else
        {
            Debug.Log("Invalid Placement - Ghost Item Not Active");
        }
    }
}



