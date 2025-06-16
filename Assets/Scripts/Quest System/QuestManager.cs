using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;

#if UNITY_EDITOR
using UnityEditor;  
#endif

public class QuestManager : MonoBehaviour
{
    public static QuestManager Instance;

    public QuestSO currentQuest;
    public event Action<QuestSO> OnQuestUpdated;

    public QuestChainSO questChain;
    private int currentQuestIndex = 0;

    private bool[] questCompletionStatus;

    void Awake()
    {
        if (Instance == null) Instance = this;
        else Destroy(gameObject);
    }

    void Start()
    {
        if (questChain != null)
        {
            StartQuestChain(questChain);
        }
    }

    public void CompleteStartingQuestAndAdvance()
    {
        if (currentQuest == null)
        {
            
            return;
        }

        
        CompleteQuest();

       
        QuestTrigger trigger = FindObjectOfType<QuestTrigger>();
        if (trigger != null)
        {
            trigger.TriggerNextQuest();

        }
    }


    public void StartQuestChain(QuestChainSO chain)
    {
        questChain = chain;
        questCompletionStatus = new bool[questChain.quests.Length];
        currentQuestIndex = 0;

        
        foreach (var quest in questChain.quests)
        {
            quest.isComplete = false;
        }

        SetQuest(questChain.quests[currentQuestIndex]);
    }


    public void SetQuest(QuestSO newQuest)
    {
        if (newQuest == null)
        {
            Debug.LogError("Tried to set a null quest!");
            return;
        }

        currentQuest = newQuest;
        currentQuest.isComplete = false;

#if UNITY_EDITOR
        
        if (currentQuest.questType == QuestSO.QuestType.PlaceItem && currentQuest.targetObject != null)
        {
            GameObject target = currentQuest.targetObject;

            bool isPrefabAsset = PrefabUtility.IsPartOfPrefabAsset(target);

            if (isPrefabAsset)
            {
                PlacementSpot[] allSpots = GameObject.FindObjectsOfType<PlacementSpot>();

                foreach (PlacementSpot spot in allSpots)
                {
                    GameObject prefabOfSpot = (GameObject)PrefabUtility.GetCorrespondingObjectFromSource(spot.gameObject);

                    if (prefabOfSpot == target)
                    {
                        currentQuest.targetObject = spot.gameObject;
                        Debug.Log($"Target prefab replaced with scene instance: {spot.gameObject.name}");
                        break;
                    }
                }
            }
        }
#endif

        Debug.Log($"[QuestManager] Quest set: {currentQuest.questName} (Index: {currentQuestIndex})");
        OnQuestUpdated?.Invoke(currentQuest);
    }

    public void CompleteQuest()
    {
        if (currentQuest != null && !questCompletionStatus[currentQuestIndex])
        {
            questCompletionStatus[currentQuestIndex] = true;
            currentQuest.isComplete = true; 
            Debug.Log($"[QuestManager] Completed quest: {currentQuest.name}, InstanceID = {currentQuest.GetInstanceID()}");

            Debug.Log($"Quest Completed: {currentQuest.questName} at index {currentQuestIndex}");

            currentQuestIndex++;

            if (currentQuestIndex < questChain.quests.Length)
            {
                SetQuest(questChain.quests[currentQuestIndex]);
            }
            else
            {
                Debug.Log("Quest chain completed!");
            }
        }
    }


    public bool IsQuestTarget(GameObject obj)
    {
        if (currentQuest == null || currentQuest.targetObject == null || obj == null)
            return false;

        return obj.name == currentQuest.targetObject.name;
    }

    public bool CheckItemPlacement(GameObject placedItem, GameObject placementSpot)
    {
        if (currentQuest == null || currentQuest.questType != QuestSO.QuestType.PlaceItem)
            return false;

        if (placementSpot == null || currentQuest.targetObject == null)
            return false;

        if (placementSpot.name != currentQuest.targetObject.name)
            return false;

        
        Potion potion = placedItem.GetComponent<Potion>();
        if (potion != null && potion.potionEffectName == currentQuest.requiredPotionEffectName)
        {
            CompleteQuest();
            return true;
        }

        
        Item item = placedItem.GetComponent<Item>();
        if (item != null && item.itemName == currentQuest.requiredItemName)
        {
            CompleteQuest();
            return true;
        }

        return false;
    }

    public void TriggerQuestFromDialogue(DialogueManager dialogue)
    {
        if (currentQuest == null || currentQuest.questType != QuestSO.QuestType.DialogueTriggered)
            return;

        if (currentQuest.targetObject != dialogue.gameObject)
            return;

        CompleteQuest();
    }

    public QuestSO GetQuestByName(string questName)
{
    if (questChain == null || questChain.quests == null) return null;

    foreach (var quest in questChain.quests)
    {
        if (quest.questName == questName)
            return quest;
    }
    return null;
}
}