using System.Collections.Generic;
using UnityEngine;
using TMPro;
using UnityEngine.UI;

public class InventoryManager : MonoBehaviour
{
    public static InventoryManager Instance;

    public GameObject inventoryUI;
    public GameObject slotPrefab; // Prefab with a Button + TMP Text
    public Transform slotParent;

    private List<GameObject> inventoryItems = new List<GameObject>();
    private bool inventoryOpen = false;

    public GameObject player;
    public GameObject pickUpButton;
    public Transform holdPosition;

    public MouseLook mouseLook;
    public ItemPickup itemPickup;

    public int maxInventorySlots = 20;
    public int ItemCount => inventoryItems.Count;
    public bool IsInventoryOpen => inventoryOpen;

    

    [System.Serializable]
    public class ItemDatabaseEntry
    {
        public string itemName;
        public GameObject itemPrefab;
    }

    public List<ItemDatabaseEntry> itemDatabase;

    private List<GameObject> slotObjects = new List<GameObject>();

    void Awake()
    {
        if (Instance == null)
            Instance = this;
        else
            Destroy(gameObject);

        inventoryUI.SetActive(false);
    }

    void Update()
{
    if ((Input.GetKeyDown(KeyCode.Tab) || Input.GetKeyDown(KeyCode.I)) && !itemPickup.IsDrinking())
    {
        ToggleInventory();
    }
}


    public void ToggleInventory()
    {
        inventoryOpen = !inventoryOpen;
        inventoryUI.SetActive(inventoryOpen);
        RefreshInventoryUI();

        Cursor.visible = inventoryOpen;
        Cursor.lockState = inventoryOpen ? CursorLockMode.None : CursorLockMode.Locked;

        if (mouseLook != null)
            mouseLook.FreezeLook(inventoryOpen);
    }

    public void AddItem(GameObject item)
{
    if (inventoryItems.Count >= maxInventorySlots)
        return;

    inventoryItems.Add(item);
    item.SetActive(false);
    item.transform.SetParent(player.transform); // Parent under player
    RefreshInventoryUI();
}

    private void RefreshInventoryUI()
{
    foreach (GameObject slot in slotObjects)
        Destroy(slot);
    slotObjects.Clear();

    foreach (GameObject item in inventoryItems)
    {
        GameObject slot = Instantiate(slotPrefab, slotParent);
        slotObjects.Add(slot);

        TextMeshProUGUI text = slot.GetComponentInChildren<TextMeshProUGUI>();
        text.text = item.name;

        Button button = slot.GetComponent<Button>();
        button.onClick.RemoveAllListeners();
        button.onClick.AddListener(() => OnInventorySlotClicked(item));
    }
}


    public void OnInventorySlotClicked(GameObject item)
{
    // Only proceed if no item is being held
    if (itemPickup.IsHoldingItem()) 
    {
        Debug.Log("Cannot interact with inventory while holding an item.");
        return; // Skip any inventory interactions
    }

    itemPickup.SpawnItemToHand(item);

    // Remove item from inventory and refresh UI
    inventoryItems.Remove(item);
    RefreshInventoryUI();

    ToggleInventory();
}

}







