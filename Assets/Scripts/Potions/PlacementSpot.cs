using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlacementSpot : MonoBehaviour
{
    public enum SpotType { Bottle, Ingredient, Quest }
    public SpotType spotType;

    private string lastCompletedQuest = "";

    private void OnTriggerEnter(Collider other)
    {
        PotionSystem potionSystem = FindObjectOfType<PotionSystem>();

        Debug.Log($"[DEBUG] Spot type: {spotType}, Object entered: {other.name}");

        if (spotType == SpotType.Bottle && other.CompareTag("Bottle"))
        {
            Debug.Log("Bottle entered bottle placement trigger.");
            potionSystem.PlaceBottle(other.gameObject);
        }
        else if (spotType == SpotType.Ingredient && other.CompareTag("Ingredient"))
        {
            Debug.Log($"Ingredient entered ingredient placement trigger: {other.name}");
            potionSystem.PlaceIngredient(other.gameObject);
        }

        if (spotType == SpotType.Quest)
        {
            Debug.Log("Quest spot detected.");
            QuestSO currentQuest = QuestManager.Instance.currentQuest;

            if (currentQuest != null)
            {
                Debug.Log($"Current quest: {currentQuest.questName}");

                // Prevent repeated completions for the same quest
                if (currentQuest.questName == lastCompletedQuest)
                {
                    Debug.Log("[Repeat Protection] Quest already completed. Skipping...");
                    return;
                }

                if (currentQuest.questType == QuestSO.QuestType.PlaceItem)
                {
                    Debug.Log("Quest is of type PlaceItem.");

                    if (QuestManager.Instance.IsQuestTarget(gameObject))
                    {
                        Debug.Log("This spot is the correct quest target.");

                        bool itemMatch = false;

                        // Check for Potion first
                        Potion potionComponent = other.GetComponent<Potion>();
                        if (potionComponent != null)
                        {
                            Debug.Log($"Found Potion component with effect: {potionComponent.potionEffectName}");
                            if (!string.IsNullOrEmpty(currentQuest.requiredPotionEffectName))
                            {
                                itemMatch = potionComponent.potionEffectName == currentQuest.requiredPotionEffectName;
                                Debug.Log($"Potion match result: {itemMatch}");
                            }
                        }
                        else
                        {
                            // Only check Item if no Potion component found
                            Item itemComponent = other.GetComponent<Item>();
                            if (itemComponent != null)
                            {
                                Debug.Log($"Found Item component with name: {itemComponent.itemName}");
                                if (!string.IsNullOrEmpty(currentQuest.requiredItemName))
                                {
                                    itemMatch = itemComponent.itemName == currentQuest.requiredItemName;
                                    Debug.Log($"Item match result: {itemMatch}");
                                }
                            }
                        }

                        if (itemMatch)
                        {
                            Debug.Log("Placed correct item/potion for quest.");
                            QuestManager.Instance.CheckItemPlacement(other.gameObject, gameObject);

                            // Store last completed quest name to prevent repeat
                            lastCompletedQuest = currentQuest.questName;
                        }
                        else
                        {
                            Debug.Log("Placed object, but it doesn't match quest requirements.");
                        }
                    }
                    else
                    {
                        Debug.Log("This spot is not the quest target.");
                    }
                }
                else
                {
                    Debug.Log("Current quest is not PlaceItem.");
                }
            }
            else
            {
                Debug.Log("No current quest active.");
            }
        }
    }
}




