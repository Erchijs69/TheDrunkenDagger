using UnityEngine;
using TMPro;

public class InventoryTooltip : MonoBehaviour
{
    public static InventoryTooltip Instance;

    public GameObject tooltipObject;
    public TextMeshProUGUI tooltipText;

    private void Awake()
    {
        Instance = this;
        tooltipObject.SetActive(false);
    }

    public void ShowTooltip(string content, Vector2 mousePosition)
{
    if (!InventoryManager.Instance.IsInventoryOpen)
        return;

    tooltipText.text = content;
    tooltipObject.SetActive(true);

    RectTransform tooltipRect = tooltipObject.GetComponent<RectTransform>();
    tooltipRect.pivot = new Vector2(0, 1);
    tooltipRect.position = mousePosition + new Vector2(10f, -10f);

    ClampToScreen(tooltipRect);
}


    public void HideTooltip()
    {
        tooltipObject.SetActive(false);
    }

    private void ClampToScreen(RectTransform tooltipRect)
{
    Vector2 anchoredPos = tooltipRect.position;
    Vector2 size = tooltipRect.sizeDelta * tooltipObject.transform.lossyScale;

    float screenWidth = Screen.width;
    float screenHeight = Screen.height;

    // Right edge
    if (anchoredPos.x + size.x > screenWidth)
        anchoredPos.x = screenWidth - size.x;
    // Left edge
    if (anchoredPos.x < 0)
        anchoredPos.x = 0;

    // Top edge
    if (anchoredPos.y < size.y)
        anchoredPos.y = size.y;
    // Bottom edge
    if (anchoredPos.y > screenHeight)
        anchoredPos.y = screenHeight;

    tooltipRect.position = anchoredPos;
}

}

