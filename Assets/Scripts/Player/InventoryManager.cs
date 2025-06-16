using System.Collections.Generic;
using UnityEngine;
using TMPro;
using UnityEngine.UI;
using UnityEngine.EventSystems;
using System.Collections;

public class InventoryManager : MonoBehaviour
{
    public static InventoryManager Instance;

    public GameObject inventoryUI;
    public GameObject slotPrefab;
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

    PotionSystem potionSystem;

    public TextMeshProUGUI inventoryFullText;

    [System.Serializable]
    public class ItemDatabaseEntry
    {
        public string itemName;
        public GameObject itemPrefab;
    }

    public List<ItemDatabaseEntry> itemDatabase;
    public List<GameObject> slotObjects = new List<GameObject>();

    void Start()
    {
        playerMovement = FindObjectOfType<PlayerMovement>();
        potionSystem = FindObjectOfType<PotionSystem>();
        slotObjects = new List<GameObject>(); 
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
    if (!IsDialogueActive() && !itemPickup.IsDrinking() && !playerMovement.IsStealthed)
    {
        if (Input.GetKeyDown(KeyCode.Tab) || Input.GetKeyDown(KeyCode.I))
        {
            ToggleInventory();
        }
    }
}

private bool IsDialogueActive()
{
    // Check if dialogue is active in the DialogueManager
    return FindObjectOfType<DialogueManager>().dialoguePanel.activeSelf;
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
            InventoryTooltip.Instance.HideTooltip();
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

    public void RefreshInventoryUI()
    {
        foreach (GameObject slot in slotObjects)
        {
            Destroy(slot);
        }
        slotObjects.Clear();

        foreach (GameObject item in inventoryItems)
        {
            GameObject slot = Instantiate(slotPrefab, slotParent);
            slotObjects.Add(slot);

            Transform iconTransform = slot.transform.Find("Icon");
            Transform liquidTransform = slot.transform.Find("Liquid");
            Transform nameTransform = slot.transform.Find("ItemName");

            Image iconImage = iconTransform != null ? iconTransform.GetComponent<Image>() : null;
            Image liquidImage = liquidTransform != null ? liquidTransform.GetComponent<Image>() : null;
            TextMeshProUGUI text = nameTransform != null ? nameTransform.GetComponent<TextMeshProUGUI>() : null;

            Item itemData = item.GetComponent<Item>();
            Potion potion = item.GetComponent<Potion>();

            if (iconImage != null)
{
    // Always prioritize the itemIcon if it's assigned
    if (itemData != null && itemData.itemIcon != null)
    {
        iconImage.sprite = itemData.itemIcon;
        iconImage.enabled = true;
    }
    else
    {
        iconImage.enabled = false; // Hide icon if none is assigned
    }
}

if (text != null)
{
    if (potion != null && !string.IsNullOrEmpty(potion.potionEffectName))
    {
        text.text = potion.potionEffectName;
    }
    else if (itemData != null)
    {
        text.text = itemData.itemName;
    }
}

if (potion != null && liquidImage != null && potion.neutralLiquidSprite != null)
{
    liquidImage.sprite = potion.neutralLiquidSprite;
    liquidImage.enabled = true;
    liquidImage.color = potion.finalPotionColor;
}
else if (liquidImage != null)
{
    liquidImage.enabled = false; // Hide if not a potion
}


            if (potion != null)
            {
                if (text != null && !string.IsNullOrEmpty(potion.potionEffectName))
                {
                    text.text = potion.potionEffectName;
                }

                if (liquidImage != null && potion.neutralLiquidSprite != null)
                {
                    liquidImage.sprite = potion.neutralLiquidSprite;
                    liquidImage.enabled = true;
                    liquidImage.color = potion.finalPotionColor;
                }
            }

            Button button = slot.GetComponent<Button>();
            button.onClick.RemoveAllListeners();
            button.onClick.AddListener(() => OnInventorySlotClicked(item));

            EventTrigger trigger = slot.AddComponent<EventTrigger>();

            EventTrigger.Entry entryEnter = new EventTrigger.Entry
            {
                eventID = EventTriggerType.PointerEnter
            };
            entryEnter.callback.AddListener((data) =>
            {
                string description = "";
                if (potion != null && !string.IsNullOrEmpty(potion.potionEffectDescription))
                    description = potion.potionEffectDescription;

                if (!string.IsNullOrEmpty(description))
                    InventoryTooltip.Instance.ShowTooltip(description, Input.mousePosition);
            });

            EventTrigger.Entry entryExit = new EventTrigger.Entry
            {
                eventID = EventTriggerType.PointerExit
            };
            entryExit.callback.AddListener((data) =>
            {
                InventoryTooltip.Instance.HideTooltip();
            });

            // Right-click to spawn item and drop after 0.2s
            EventTrigger.Entry rightClickEntry = new EventTrigger.Entry
            {
                eventID = EventTriggerType.PointerClick
            };
            rightClickEntry.callback.AddListener((data) =>
            {
                PointerEventData pointerData = (PointerEventData)data;
                if (pointerData.button == PointerEventData.InputButton.Right)
                {
                    RightClickSpawnAndDrop(item);  // Direct call without coroutine
                }
            });


            trigger.triggers.Add(entryEnter);
            trigger.triggers.Add(entryExit);
            trigger.triggers.Add(rightClickEntry);
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

    private void RightClickSpawnAndDrop(GameObject item)
{
    if (itemPickup.IsHoldingItem())
    {
        Debug.Log("Cannot spawn item while already holding one.");
        return;
    }

    itemPickup.SpawnItemToHand(item);
    inventoryItems.Remove(item);
    RefreshInventoryUI();

    // Drop the item immediately after it's spawned
    itemPickup.ForceDropHeldItem();

   
}



    public List<GameObject> GetSlotObjects()
    {
        return slotObjects;
    }

    public void ShowInventoryFullMessage()
    {
        StopAllCoroutines();
        StartCoroutine(ShowInventoryFullCoroutine());
    }

    private IEnumerator ShowInventoryFullCoroutine()
    {
        inventoryFullText.gameObject.SetActive(true);
        yield return new WaitForSeconds(2f);
        inventoryFullText.gameObject.SetActive(false);
    }
}













