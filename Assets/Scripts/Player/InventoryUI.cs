using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class InventoryUI : MonoBehaviour
{
    private List<GameObject> inventoryItems = new List<GameObject>();

   
    public void RemoveItemFromUI(string itemName)
    {
        GameObject itemToRemove = inventoryItems.Find(item => item.name == itemName);
        if (itemToRemove != null)
        {
            inventoryItems.Remove(itemToRemove);
        }
    }
}
