using UnityEngine;

public class Potion : MonoBehaviour
{
    public string potionEffectName;
    public string potionEffectDescription; 

    [Header("Floating UI")]
    public GameObject effectNameCanvas;
    public TMPro.TextMeshProUGUI effectNameText;

    public bool HasBeenConsumed { get; private set; } = false;

    private void Start()
    {
        if (effectNameCanvas != null)
            effectNameCanvas.SetActive(false);
    }

    public void Consume()
{
    HasBeenConsumed = true;
    Debug.Log($"Potion {potionEffectName} consumed!");

    // Disable colliders to prevent future pickup
    Collider[] colliders = GetComponents<Collider>();
    foreach (var col in colliders)
        col.enabled = false;
}


    public void ShowEffectName()
    {
        if (effectNameCanvas != null && effectNameText != null)
        {
            effectNameText.text = potionEffectName;
            effectNameCanvas.SetActive(true);
        }
    }

    public void HideEffectName()
    {
        if (effectNameCanvas != null)
            effectNameCanvas.SetActive(false);
    }
}






