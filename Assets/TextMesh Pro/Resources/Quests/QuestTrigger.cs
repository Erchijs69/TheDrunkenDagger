using UnityEngine;

public class QuestTrigger : MonoBehaviour
{
    public QuestSO nextQuest;  // Reference to the next quest in the chain

    public void TriggerNextQuest()
    {
        QuestManager.Instance.CompleteQuest();  // Tell QuestManager to complete current and go to next
        Debug.Log("Triggered next quest via CompleteQuest");
    }
}

