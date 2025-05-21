using UnityEngine;

[System.Serializable]
public class Item : MonoBehaviour
{
    public string itemName;           // Item's name
    public GameObject itemPrefab;     // 3D model prefab of the item
    public Sprite itemIcon;           // 📷 New: Inventory icon
}

