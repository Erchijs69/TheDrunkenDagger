using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "NewQuest", menuName = "Quest System/Quest")]
public class QuestSO : ScriptableObject
{
    public string questName;
    [TextArea] public string questDescription;
    public bool isComplete;

    public enum QuestType { OpenDoor, TalkToNPC, CollectItem }
    public QuestType questType;

    public GameObject targetObject; // For door, NPC, etc.
}

