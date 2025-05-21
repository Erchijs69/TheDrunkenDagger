using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Unity.AI.Navigation.Samples;

[System.Serializable]
public class DialogueLine
{
    public string speakerName;
    [TextArea(2, 5)]
    public string lineText;
}

public class Waypoint : MonoBehaviour
{
    public DialogueLine[] dialogueLines;
    public bool useStopForSeconds = false;
    public float stopDuration = 3f;

    [Header("Quest Integration")]
    public bool triggersQuestOnEnd = false;
    public bool requiresQuestCompletion = false;
    public QuestSO requiredQuest;
}




