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

    void Start()
    {
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
    Ray ray = Camera.main.ScreenPointToRay(new Vector3(Screen.width / 2, Screen.height / 2));
    RaycastHit hit;

    if (Physics.Raycast(ray, out hit, pickupRange, itemLayer))
    {
        heldItem = hit.collider.gameObject;
        heldItemRb = heldItem.GetComponent<Rigidbody>();
        itemCollider = heldItem.GetComponent<Collider>();

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
        heldItem.transform.position = ghostItem.transform.position + (Vector3.up * placementHeightOffset);
        heldItem.transform.rotation = Quaternion.identity;

        heldItemRb.isKinematic = false;

        Destroy(ghostItem);
        Debug.Log("Item placed at: " + heldItem.transform.position);

        heldItem = null;
    }
}


