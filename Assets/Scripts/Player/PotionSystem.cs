using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PotionSystem : MonoBehaviour
{
    [SerializeField] private List<GameObject> placedIngredients = new List<GameObject>();
    public Transform[] ingredientSpots; // 3 ingredient spots placed in the scene
    private GameObject placedBottle;

    void Start()
    {
        // Ensure ingredient spots are disabled initially
        DisableIngredientSpots();
    }

    // Place the bottle at the desired spot (where the player places it)
    public void PlaceBottle(GameObject bottle)
    {
        // If there's an already placed bottle, remove it first (reset the potion system)
        if (placedBottle != null)
        {
            RemoveBottle();
        }

        // Set the new bottle as the placed bottle
        placedBottle = bottle;
        placedBottle.transform.SetParent(transform); // Keep the parent for organization (optional)
        
        // Debugging to check if bottle is correctly placed
        Debug.Log($"Bottle placed: {bottle.name}");

        // Show the ingredient spots for this bottle
        ShowIngredientSpots();
    }

    void ShowIngredientSpots()
    {
        foreach (Transform spot in ingredientSpots)
        {
            spot.gameObject.SetActive(true); // Enable ingredient spots when bottle is placed
        }
    }

    public void PlaceIngredient(GameObject ingredient)
    {
        if (placedBottle == null)
        {
            Debug.Log("Cannot place ingredient: Bottle has not been placed yet.");
            return;
        }

        if (placedIngredients.Count >= 3)
        {
            Debug.Log("Already placed 3 ingredients.");
            return;
        }

        for (int i = 0; i < ingredientSpots.Length; i++)
        {
            if (placedIngredients.Count <= i && ingredientSpots[i] != null && !placedIngredients.Contains(ingredient))
            {
                ingredient.transform.position = ingredientSpots[i].position;
                ingredient.transform.SetParent(ingredientSpots[i]);
                placedIngredients.Add(ingredient);

                Debug.Log($"Ingredient placed on spot {i + 1}: {ingredient.name}");

                if (placedIngredients.Count == 3)
                {
                    Debug.Log("All ingredients placed. Processing potion...");
                    ProcessPotionCreation();
                }

                return;
            }
        }
    }

    void ProcessPotionCreation()
    {
        Color potionColor = GeneratePotionColor();
        Debug.Log($"Potion color generated: {potionColor}");

        if (placedBottle != null)
        {
            Transform bottleVisual = placedBottle.transform.GetChild(0); // Assuming the bottle's visual is its first child
            if (bottleVisual != null)
            {
                Renderer bottleRenderer = bottleVisual.GetComponent<Renderer>();
                if (bottleRenderer != null)
                {
                    bottleRenderer.material.color = potionColor;
                    Debug.Log("Applied color to bottle.");
                }
                else
                {
                    Debug.LogError("Bottle renderer NOT found!");
                }
            }
            else
            {
                Debug.LogError("Bottle visual child not found!");
            }
        }

        // Destroy all placed ingredients after potion is created
        foreach (GameObject ingredient in placedIngredients)
        {
            Destroy(ingredient);
        }

        placedIngredients.Clear();
    }

    Color GeneratePotionColor()
    {
        Color blendedColor = Color.black;

        if (placedIngredients.Count == 3)
        {
            Color color1 = placedIngredients[0].GetComponent<Ingredient>().ingredientColor;
            Color color2 = placedIngredients[1].GetComponent<Ingredient>().ingredientColor;
            Color color3 = placedIngredients[2].GetComponent<Ingredient>().ingredientColor;

            blendedColor = new Color(
                (color1.r + color2.r + color3.r) / 3f,
                (color1.g + color2.g + color3.g) / 3f,
                (color1.b + color2.b + color3.b) / 3f
            );
        }

        return blendedColor;
    }

    public void RemoveBottle()
    {
        if (placedBottle != null)
        {
            // Reset the placed bottle and clear ingredients
            placedBottle = null;
            placedIngredients.Clear();
            DisableIngredientSpots(); // Hide ingredient spots when bottle is removed
            Debug.Log("Bottle removed.");
        }
    }

    void DisableIngredientSpots()
    {
        foreach (Transform spot in ingredientSpots)
        {
            spot.gameObject.SetActive(false); // Disable ingredient spots if bottle is removed
        }
    }
}
















