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
    public LayerMask itemLayer; // In the inspector, uncheck "UITrigger"
    public ItemPickup itemPickup;
    private string lastPotionKey; // Store the key of the last created potion

    public PlayerMovement playerMovement; // Add this line

    //JUMP BUFF
    public float jumpHeightBoostAmount = 5f; // Amount to boost jump height
    public float jumpHeightBoostDuration = 10f; // Duration of the boost
    private float originalJumpHeight; // To store the player's original jump height

    private PlayerBuffs playerBuffs;

    

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

        playerBuffs = FindObjectOfType<PlayerBuffs>();

        if (playerMovement == null)
        playerMovement = FindObjectOfType<PlayerMovement>(); // Automatically find the PlayerMovement component

        DisableIngredientSpots();

        // Initialize potion effects with specific effect names you gave
        potionEffects.Add("BlueRedGreen", new PotionEffect("Speed Boost", "Increases running speed."));
        potionEffects.Add("RedGreenYellow", new PotionEffect("Sneak Speed Boost", "Increases sneaking speed."));
        potionEffects.Add("BlueYellowRed", new PotionEffect("Assassination Speed Boost", "Enhances speed for stealth attacks."));
        potionEffects.Add("RedRedRed", new PotionEffect("Jump Height", "Increases how high you can jump."));
        potionEffects.Add("BlueBlueCyan", new PotionEffect("Swim Speed", "Makes you swim faster."));
        potionEffects.Add("GreenGreenGreen", new PotionEffect("Speed Boost", "Increases running speed."));
        potionEffects.Add("CyanCyanCyan", new PotionEffect("Wraith Form", "Turn into a wrait, ignoring enemies and interactions"));
        potionEffects.Add("PinkPinkPink", new PotionEffect("Tiny Tina's Curse", "Shrinks you down permanently"));

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

// Add a default effect to handle unknown potions
private void ProcessPotionCreation()
{
    string potionKey = GeneratePotionKey(); // Generate a unique key based on the ingredients
    lastPotionKey = potionKey;
    Debug.Log($"Potion key generated: {potionKey}");

    if (potionEffects.ContainsKey(potionKey))
    {
        PotionEffect effect = potionEffects[potionKey];
        Debug.Log($"Potion effect: {effect.effectName} - {effect.description}");

        // Rename the potion only after the effect has been determined
        if (placedBottle != null)
        {
            Potion potion = placedBottle.GetComponent<Potion>();
            if (potion != null)
            {
                potion.potionEffectName = effect.effectName; // Rename after effect is assigned
            }
        }
    }
    else
    {
        // No specific effect found, create potion with no effect
        Debug.Log("Potion effect: None - Creating default potion.");
        // Optionally apply some default behavior for no effect (like a neutral color or visual)
        Color defaultColor = Color.gray;
        if (placedBottle != null)
        {
            Transform bottleVisual = placedBottle.transform.GetChild(0); // Assuming the visual is the first child
            if (bottleVisual != null)
            {
                Renderer bottleRenderer = bottleVisual.GetComponent<Renderer>();
                if (bottleRenderer != null)
                {
                    // Apply default color to the bottle
                    bottleRenderer.material.SetColor("_BottomColor", defaultColor);
                    Color topColor = defaultColor * 0.5f;
                    topColor.a = 1f;
                    bottleRenderer.material.SetColor("_TopColor", topColor);
                    Color foamColor = Color.Lerp(defaultColor, Color.white, 0.5f);
                    foamColor.a = 1f;
                    bottleRenderer.material.SetColor("_FoamColor", foamColor);
                    Debug.Log("Applied default potion colors.");
                }
            }
        }
    }

    // Continue with potion creation (color blending and filling)
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

    if (placedBottle != null)
    {
        Potion potion = placedBottle.GetComponent<Potion>();
        if (potion != null)
        {
            potion.ShowEffectName(); // <<< Show the effect name (even if it's empty or default)
        }
    }
}

private void ApplyPotionEffect(PotionEffect effect)
{
    if (effect.effectName.Contains("Speed"))
    {
        playerBuffs.ApplyBuff("Speed Boost");
    }
    else if (effect.effectName.Contains("Jump Height"))
    {
        playerBuffs.ApplyBuff("Jump Height");
    }
    else if (effect.effectName.Contains("Swim Speed"))
    {
        playerBuffs.ApplySwimSpeedBoost(1.5f, 5f); 
    }
    else if (effect.effectName.Contains("Wraith Form"))
    {
        playerBuffs.ApplyBuff("Wraith Form"); // <<< Add this line
    }

    else if (effect.effectName.Contains("Tiny Tina"))
    {
        playerBuffs.ApplyPermanentShrink(); // Shrinks to 30% scale for 10 seconds
    }

    else
    {
        Debug.Log("Potion has no specific effect to apply.");
    }
}





    // New method to apply the speed boost
    private void ApplySpeedBoost()
{
    if (itemPickup != null && itemPickup.playerMovement != null)
    {
        float speedBoostMultiplier = 1.5f;
        itemPickup.playerMovement.SetSpeedMultiplier(speedBoostMultiplier);
        Debug.Log($"Speed multiplier applied: {speedBoostMultiplier}");
    }
}



    string GeneratePotionKey()
{
    List<string> ingredientTypes = new List<string>();

    foreach (GameObject ingredient in placedIngredients)
    {
        Ingredient ingredientComponent = ingredient.GetComponent<Ingredient>();
        if (ingredientComponent != null)
        {
            ingredientTypes.Add(ingredientComponent.ingredientType); // Use ingredientType here
        }
    }

    // Sort the ingredient types to ensure consistency for the key
    ingredientTypes.Sort();

    // Join the sorted types to form a unique key
    return string.Join("", ingredientTypes);
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
    // Don't apply the speed boost here anymore.
}



private bool effectApplied = false;
private bool HasEffectApplied()
{
    return effectApplied;
}

private void SetEffectApplied()
{
    effectApplied = true;
}


    public void DrinkPotion(GameObject bottle)
{
    if (!isPotionBlended || bottle != placedBottle)
    {
        Debug.Log("Cannot drink: No valid blended potion.");
        return;
    }

    // Apply the effect now, when the potion is drunk
    string key = lastPotionKey;
    if (potionEffects.ContainsKey(key))
    {
        PotionEffect effect = potionEffects[key];
        Debug.Log($"Drank potion: {effect.effectName}");

        ApplyPotionEffect(effect); // ← Apply the effect here

        // Optionally destroy bottle or play a drink animation
        Destroy(bottle);
    }
}


public void ApplyJumpHeightBoost()
{
    playerBuffs.ApplyBuff("Jump Height");
}


private IEnumerator ResetJumpHeightAfterBoost()
{
    yield return new WaitForSeconds(jumpHeightBoostDuration);

    if (playerMovement != null)
    {
        playerMovement.jumpHeight = originalJumpHeight;
        Debug.Log("Jump Height Boost ended.");
    }
}
}


























