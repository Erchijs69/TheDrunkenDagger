using UnityEngine;

public class Potion : MonoBehaviour
{
    public string potionEffectName;

    [Header("Floating UI")]
    public GameObject effectNameCanvas;
    public TMPro.TextMeshProUGUI effectNameText;

    public bool HasBeenConsumed { get; private set; } = false;

    private void Start()
    {
        if (effectNameCanvas != null)
            effectNameCanvas.SetActive(false); // Hide at start
    }

    public void Consume()
    {
        HasBeenConsumed = true; // Mark as consumed
        Debug.Log($"Potion {potionEffectName} consumed!");
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





