using System.Collections.Generic;
using UnityEngine;
using TMPro;
using UnityEngine.UI;
using UnityEngine.EventSystems;

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

    public PlayerMovement playerMovement;

    [System.Serializable]
    public class ItemDatabaseEntry
    {
        public string itemName;
        public GameObject itemPrefab;
    }

    public List<ItemDatabaseEntry> itemDatabase;

    private List<GameObject> slotObjects = new List<GameObject>();

    void Start()
    {
        playerMovement = FindObjectOfType<PlayerMovement>();
    }

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
        if ((Input.GetKeyDown(KeyCode.Tab) || Input.GetKeyDown(KeyCode.I)) && !itemPickup.IsDrinking() && !playerMovement.IsStealthed)
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

        if (!inventoryOpen)
        InventoryTooltip.Instance.HideTooltip(); // 👈 Add this
    }

    public void AddItem(GameObject item)
    {
        if (inventoryItems.Count >= maxInventorySlots)
            return;

        inventoryItems.Add(item);
        item.SetActive(false);
        item.transform.SetParent(player.transform);
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

            Potion potion = item.GetComponent<Potion>();
            string description = "";
            if (potion != null && !string.IsNullOrEmpty(potion.potionEffectName))
            {
                text.text = potion.potionEffectName;
                description = potion.potionEffectDescription;
            }
            else
            {
                text.text = item.name;
            }

            Button button = slot.GetComponent<Button>();
            button.onClick.RemoveAllListeners();
            button.onClick.AddListener(() => OnInventorySlotClicked(item));

            // Tooltip Hover Events
            EventTrigger trigger = slot.AddComponent<EventTrigger>();

            EventTrigger.Entry entryEnter = new EventTrigger.Entry();
            entryEnter.eventID = EventTriggerType.PointerEnter;
            entryEnter.callback.AddListener((data) =>
            {
                if (!string.IsNullOrEmpty(description))
                    InventoryTooltip.Instance.ShowTooltip(description, Input.mousePosition);
            });

            EventTrigger.Entry entryExit = new EventTrigger.Entry();
            entryExit.eventID = EventTriggerType.PointerExit;
            entryExit.callback.AddListener((data) =>
            {
                InventoryTooltip.Instance.HideTooltip();
            });

            trigger.triggers.Add(entryEnter);
            trigger.triggers.Add(entryExit);
        }
    }

    public void OnInventorySlotClicked(GameObject item)
    {
        if (itemPickup.IsHoldingItem())
        {
            Debug.Log("Cannot interact with inventory while holding an item.");
            return;
        }

        itemPickup.SpawnItemToHand(item);
        inventoryItems.Remove(item);
        RefreshInventoryUI();
        ToggleInventory();
    }
}








