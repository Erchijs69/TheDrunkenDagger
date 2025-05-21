using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "NewQuest", menuName = "Quest System/Quest")]
public class QuestSO : ScriptableObject
{
    public string questName;
    [TextArea] public string questDescription;
    public bool isComplete;

    public enum QuestType { Interact, PlaceItem, DialogueTriggered }

    public QuestType questType;

    public GameObject targetObject;   // Used for Interact and PlaceItem

    [Header("Place Item Quest Data")]
    public string requiredItemName;         // Used if item has itemName
    public string requiredPotionEffectName; // Used if item is a potion
}


