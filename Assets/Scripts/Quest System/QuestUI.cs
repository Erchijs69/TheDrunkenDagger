using TMPro;
using UnityEngine;

public class QuestUI : MonoBehaviour
{
    public TMP_Text questNameText;
    public TMP_Text questDescriptionText;

    void Start()
    {
        QuestManager.Instance.OnQuestUpdated += UpdateUI;
    }

    void UpdateUI(QuestSO quest)
    {
        questNameText.text = quest.questName;
        questDescriptionText.text = quest.questDescription;
    }

    void OnDestroy()
    {
        QuestManager.Instance.OnQuestUpdated -= UpdateUI;
    }
}

