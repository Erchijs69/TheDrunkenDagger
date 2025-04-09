using UnityEngine;

public class QuestTrigger : MonoBehaviour
{
    public QuestSO nextQuest;  // Reference to the next quest in the chain

    public void TriggerNextQuest()
    {
        if (nextQuest != null)
        {
            QuestManager.Instance.SetQuest(nextQuest);  // Set the next quest
            Debug.Log("Next quest: " + nextQuest.questName);  // For debugging
        }
    }
}

