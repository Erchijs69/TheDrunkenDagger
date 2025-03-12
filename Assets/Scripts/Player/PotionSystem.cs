using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PotionSystem : MonoBehaviour
{
    public Transform bottlePlacementSpot;
    public Transform[] ingredientSpots;
    public GameObject potionPrefab;
    public GameObject[] ingredientPrefabs;
    private List<GameObject> placedIngredients = new List<GameObject>();
    private GameObject placedBottle;

    void Start()
    {
        // No need for InitializeIngredientColors method anymore
    }

    public void PlaceIngredient(GameObject ingredient)
    {
        if (placedIngredients.Count < 3)
        {
            ingredient.transform.position = ingredientSpots[placedIngredients.Count].position;
            placedIngredients.Add(ingredient);

            if (placedIngredients.Count == 3)
            {
                ProcessPotionCreation();
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
            // Get the ingredient colors from the Ingredient component
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
