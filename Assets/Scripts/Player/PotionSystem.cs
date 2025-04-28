using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PotionSystem : MonoBehaviour
{
    [SerializeField] private List<GameObject> placedIngredients = new List<GameObject>();
    public Transform[] ingredientSpots;
    private GameObject placedBottle;
    private bool isPotionBlended = false; // Flag to track potion blending status
    private float fillAmount = 1f; // Start full
    private Coroutine fillCoroutine;

    // Class to hold the potion effect data
    public class PotionEffect
    {
        public string effectName;
        public string description;

        public PotionEffect(string effectName, string description)
        {
            this.effectName = effectName;
            this.description = description;
        }
    }

    // Mapping of ingredient combination to specific potion effects
    private Dictionary<string, PotionEffect> potionEffects = new Dictionary<string, PotionEffect>();

   void Start()
{
    DisableIngredientSpots();

    // Initialize potion effects with specific effect names you gave
    potionEffects.Add("BlueRedGreen", new PotionEffect("Speed Boost", "Increases running speed."));
    potionEffects.Add("RedGreenYellow", new PotionEffect("Sneak Speed Boost", "Increases sneaking speed."));
    potionEffects.Add("BlueYellowRed", new PotionEffect("Assassination Speed Boost", "Enhances speed for stealth attacks."));
    potionEffects.Add("GreenGreenBlue", new PotionEffect("Jump Height Boost", "Increases how high you can jump."));
    potionEffects.Add("YellowRedBlue", new PotionEffect("Swim Speed Boost", "Makes you swim faster."));
    
    // You can continue adding more if needed!
}


    public void PlaceBottle(GameObject bottle)
    {
        // Remove the current bottle if one exists
        if (placedBottle != null)
        {
            RemoveBottle();
        }

        // Set the new bottle
        placedBottle = bottle;
        placedBottle.transform.SetParent(transform);
        Debug.Log($"Bottle placed: {bottle.name}");

        // Reset the fill amount to full when a new bottle is placed
        fillAmount = 1f;

        // Show ingredient spots
        ShowIngredientSpots();
    }

    void ShowIngredientSpots()
    {
        foreach (Transform spot in ingredientSpots)
        {
            spot.gameObject.SetActive(true);
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
        string potionKey = GeneratePotionKey(); // Generate a unique key based on the ingredients
        Debug.Log($"Potion key generated: {potionKey}");

        if (potionEffects.ContainsKey(potionKey))
        {
            PotionEffect effect = potionEffects[potionKey];
            Debug.Log($"Potion effect: {effect.effectName} - {effect.description}");

            // Apply effect (for now, just log it)
            // This could be a call to apply the effect in-game
        }

        // Handle potion creation as normal...
        Color potionColor = GeneratePotionColor();
        Debug.Log($"Potion color generated: {potionColor}");

        if (placedBottle != null)
        {
            Transform bottleVisual = placedBottle.transform.GetChild(0); // Assuming the visual is the first child
            if (bottleVisual != null)
            {
                Renderer bottleRenderer = bottleVisual.GetComponent<Renderer>();
                if (bottleRenderer != null)
                {
                    // Apply color to the bottle
                    bottleRenderer.material.SetColor("_BottomColor", potionColor);
                    Color topColor = potionColor * 0.5f;
                    topColor.a = 1f;
                    bottleRenderer.material.SetColor("_TopColor", topColor);
                    Color foamColor = Color.Lerp(potionColor, Color.white, 0.5f);
                    foamColor.a = 1f;
                    bottleRenderer.material.SetColor("_FoamColor", foamColor);
                    Debug.Log("Applied colors.");
                }
            }

            // Gradually reduce the fill amount from 1 to 0.5
            if (fillCoroutine != null)
                StopCoroutine(fillCoroutine);
            fillCoroutine = StartCoroutine(GradualFillDecrease());

            // Remove ingredients and prepare the potion for drinking
            foreach (GameObject ingredient in placedIngredients)
            {
                Destroy(ingredient);
            }

            placedIngredients.Clear();
            isPotionBlended = true;
        }
    }

    string GeneratePotionKey()
    {
        // Generate a unique key based on the names of the placed ingredients
        List<string> ingredientNames = new List<string>();

        foreach (GameObject ingredient in placedIngredients)
        {
            ingredientNames.Add(ingredient.name);
        }

        // Sort the ingredient names to ensure consistency for the key (so BlueRedGreen is always the same)
        ingredientNames.Sort();

        // Join the sorted names to form a unique key
        return string.Join("", ingredientNames);
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
            // Reset fill amount when the bottle is removed
            fillAmount = 1f;

            // Stop any ongoing fill coroutine
            if (fillCoroutine != null)
            {
                StopCoroutine(fillCoroutine);
            }

            // Clear the ingredients list and disable spots
            placedIngredients.Clear();
            DisableIngredientSpots();
            placedBottle = null;
            Debug.Log("Bottle removed.");
        }
    }

    void DisableIngredientSpots()
    {
        foreach (Transform spot in ingredientSpots)
            spot.gameObject.SetActive(false);
    }

    public bool IsPotionBlended()
    {
        return isPotionBlended;
    }

    private IEnumerator GradualFillDecrease()
{
    while (fillAmount > 0.5f)
    {
        fillAmount -= 0.05f;
        fillAmount = Mathf.Max(fillAmount, 0.5f);

        Liquid liquid = placedBottle.GetComponentInChildren<Liquid>();
        if (liquid != null)
        {
            liquid.SetFillAmount(fillAmount);
        }
        yield return new WaitForSeconds(0.1f);
    }

    Debug.Log("Potion fill is at 0.5 and won't decrease further.");

    // ====> HERE show the effect name!
    if (placedBottle != null)
    {
        Potion potion = placedBottle.GetComponent<Potion>();
        if (potion != null)
        {
            potion.ShowEffectName();
        }
    }
}

}

























