using UnityEngine;
using UnityEngine.UI;

public class Potion : MonoBehaviour
{
    public string potionEffectName;
    public string potionEffectDescription; 

    public Sprite neutralLiquidSprite;

    [Header("Floating UI")]
    public GameObject effectNameCanvas;
    public TMPro.TextMeshProUGUI effectNameText;

    public bool HasBeenConsumed { get; private set; } = false;

    public Color finalPotionColor = Color.white;

    private void Start()
    {
        if (effectNameCanvas != null)
            effectNameCanvas.SetActive(false);
    }

    public void Consume()
    {
        HasBeenConsumed = true;
        Debug.Log($"Potion {potionEffectName} consumed!");

       
        Collider[] colliders = GetComponents<Collider>();
        foreach (var col in colliders)
            col.enabled = false;
    }

    public void ShowEffectName()
{
    if (effectNameCanvas != null && effectNameText != null)
    {
        Debug.Log("Showing potion effect name: " + potionEffectName); 
        effectNameText.text = potionEffectName;
        effectNameCanvas.SetActive(true);
    }
    else
    {
        Debug.LogWarning("effectNameCanvas or effectNameText is null!");
    }
}

public void HideEffectName()
{
    if (effectNameCanvas != null)
    {
        Debug.Log("Hiding potion effect name."); 
        effectNameCanvas.SetActive(false);
    }
}


    public void UpdatePotionColor(Color newColor)
{
    Renderer renderer = GetComponent<Renderer>();
    if (renderer != null)
    {
      
        renderer.material.SetColor("_Color", newColor);
        Debug.Log($"Potion color updated to {newColor}");
    }
    else
    {
        Debug.LogError("Renderer not found on the potion.");
    }
}

} 







