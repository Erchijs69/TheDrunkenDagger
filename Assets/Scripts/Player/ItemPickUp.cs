using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ItemPickup : MonoBehaviour
{
    public Transform holdPosition;
    public float pickupRange = 3f;
    public LayerMask itemLayer;
    public LayerMask placementLayer;
    public Material highlightMaterial;

    private GameObject heldItem;
    private Rigidbody heldItemRb;
    private Collider itemCollider;
    private GameObject[] placementSpots;
    private Material[] originalMaterials;

    private List<Color> placedIngredients = new List<Color>();
    private Transform bottleSpot;
    private GameObject currentBottle;

    void Start()
    {
        placementSpots = GameObject.FindGameObjectsWithTag("PlacementSpot");
        originalMaterials = new Material[placementSpots.Length];

        for (int i = 0; i < placementSpots.Length; i++)
        {
            Renderer renderer = placementSpots[i].GetComponent<Renderer>();
            if (renderer != null)
            {
                originalMaterials[i] = renderer.material;
                renderer.enabled = false;
            }
        }
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
            HighlightPlacementSpots();
        else
            ResetPlacementHighlights();
    }

    void TryPickupItem()
    {
        RaycastHit hit;
        if (Physics.Raycast(transform.position, transform.forward, out hit, pickupRange, itemLayer))
        {
            heldItem = hit.collider.gameObject;
            heldItemRb = heldItem.GetComponent<Rigidbody>();
            itemCollider = heldItem.GetComponent<Collider>();

            if (heldItem.CompareTag("Bottle"))
            {
                currentBottle = heldItem; // Assign the bottle
            }

            heldItemRb.isKinematic = true;
            heldItem.transform.SetParent(holdPosition);
            heldItem.transform.localPosition = Vector3.zero;
            heldItem.transform.localRotation = Quaternion.identity;

            IgnoreCollisionWithPlacementSpots(true);
        }
    }

    void TryPlaceItem()
    {
        Collider[] colliders = Physics.OverlapSphere(heldItem.transform.position, 1.5f, placementLayer);
        if (colliders.Length > 0)
        {
            Transform closestSpot = GetClosestPlacementSpot(colliders);
            heldItem.transform.position = closestSpot.position;
            heldItem.transform.rotation = closestSpot.rotation;

            if (heldItem.CompareTag("Ingredient"))
            {
                Ingredient ingredient = heldItem.GetComponent<Ingredient>();
                if (ingredient != null)
                {
                    placedIngredients.Add(ingredient.ingredientColor);  // Add the color of the ingredient
                }

                // Only destroy ingredients after all 3 have been placed
                if (placedIngredients.Count == 3)
                {
                    ApplyPotionColor();  // Apply color change to the potion
                    DestroyPlacedIngredients();  // Destroy ingredients after color is applied
                }
            }
            else if (heldItem.CompareTag("Bottle"))
            {
                bottleSpot = closestSpot;
                currentBottle = heldItem;  // Assign the bottle when it is placed
                Debug.Log("Bottle placed at " + bottleSpot.position); // Debugging
            }

            DropItem(); // Drop the item after placing
        }
    }

    void ApplyPotionColor()
    {
        if (currentBottle != null)
        {
            Color finalColor = MixColors(placedIngredients);
            currentBottle.transform.GetChild(0).GetComponent<Renderer>().material.color = finalColor;
        }
        else
        {
            Debug.LogError("currentBottle is null when trying to apply potion color!");
        }
    }

    Color MixColors(List<Color> ingredients)
    {
        Color mixedColor = Color.black;
        foreach (Color ingredientColor in ingredients)
        {
            mixedColor += ingredientColor;  // Add the color of the ingredient
        }
        return mixedColor / ingredients.Count;  // Average the color
    }

    void DropItem()
    {
        if (heldItem != null)
        {
            heldItem.transform.SetParent(null);
            heldItem = null;
            heldItemRb = null;
            IgnoreCollisionWithPlacementSpots(false);
        }
    }

    void DestroyPlacedIngredients()
    {
        // Destroy the GameObject representing each ingredient after the potion color is applied
        foreach (Color ingredientColor in placedIngredients)
        {
            GameObject ingredientObject = FindIngredientByColor(ingredientColor);
            if (ingredientObject != null)
            {
                Destroy(ingredientObject); // Destroy the ingredient GameObject
            }
        }

        // Clear placed ingredients list to reset it for future use
        placedIngredients.Clear();
    }

    GameObject FindIngredientByColor(Color color)
    {
        // Logic to find the ingredient GameObject by its color tag
        // You may need to implement this based on how ingredients are instantiated or placed in the scene
        foreach (GameObject ingredientObject in GameObject.FindGameObjectsWithTag("Ingredient"))
        {
            Ingredient ingredientScript = ingredientObject.GetComponent<Ingredient>();
            if (ingredientScript != null && ingredientScript.ingredientColor == color)
            {
                return ingredientObject;
            }
        }
        return null;
    }

    Transform GetClosestPlacementSpot(Collider[] colliders)
    {
        Transform closestSpot = null;
        float closestDistance = float.MaxValue;

        foreach (Collider col in colliders)
        {
            float distance = Vector3.Distance(heldItem.transform.position, col.transform.position);
            if (distance < closestDistance)
            {
                closestDistance = distance;
                closestSpot = col.transform;
            }
        }
        return closestSpot;
    }

    void IgnoreCollisionWithPlacementSpots(bool ignore)
    {
        foreach (GameObject spot in placementSpots)
        {
            Collider spotCollider = spot.GetComponent<Collider>();
            if (spotCollider != null && itemCollider != null)
            {
                Physics.IgnoreCollision(itemCollider, spotCollider, ignore);
            }
        }
    }

    void HighlightPlacementSpots()
    {
        foreach (GameObject spot in placementSpots)
        {
            Renderer renderer = spot.GetComponent<Renderer>();
            if (renderer != null)
            {
                renderer.enabled = true;
                renderer.material = highlightMaterial;
            }
        }
    }

    void ResetPlacementHighlights()
    {
        for (int i = 0; i < placementSpots.Length; i++)
        {
            Renderer renderer = placementSpots[i].GetComponent<Renderer>();
            if (renderer != null)
            {
                renderer.material = originalMaterials[i];
                renderer.enabled = false;
            }
        }
    }
}


