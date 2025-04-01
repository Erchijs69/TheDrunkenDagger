using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PotionSystem : MonoBehaviour
{
    public Transform bottlePlacementSpot;
    public LayerMask ingredientLayer;  // Layer where ingredients are placed
    public GameObject potionPrefab;

    private GameObject placedBottle;
    private List<GameObject> placedIngredients = new List<GameObject>();

    public Material ghostIngredientMaterial;  // Material for the ghost ingredients (semi-transparent)

    void Update()
    {
        // This would be used to place ingredients on the ingredient layers
    }

    public void PlaceIngredient(GameObject ingredient)
    {
        if (placedIngredients.Count < 3)
        {
            RaycastHit hit;
            if (Physics.Raycast(ingredient.transform.position, Vector3.down, out hit, 1f, ingredientLayer))
            {
                // Make sure it's a valid ingredient placement area
                if (hit.collider.CompareTag("IngredientLayer"))
                {
                    ingredient.transform.position = hit.point;
                    placedIngredients.Add(ingredient);
                    if (placedIngredients.Count == 3)
                    {
                        ProcessPotionCreation();
                    }
                }
            }
        }
    }

    void ProcessPotionCreation()
    {
        Color potionColor = GeneratePotionColor();

        if (placedBottle != null)
        {
            Renderer bottleRenderer = placedBottle.transform.GetChild(0).GetComponent<Renderer>();
            bottleRenderer.material.color = potionColor;
        }

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
                (color1.r + color2.r + color3.r) / 3,
                (color1.g + color2.g + color3.g) / 3,
                (color1.b + color2.b + color3.b) / 3
            );
        }
        return blendedColor;
    }

    public void PlaceBottle(GameObject bottle)
    {
        if (placedBottle == null && bottle.CompareTag("PickupItem"))
        {
            placedBottle = bottle;
            bottle.transform.position = bottlePlacementSpot.position;
        }
    }
}
