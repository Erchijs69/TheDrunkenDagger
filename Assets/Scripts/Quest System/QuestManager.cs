using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;

public class QuestManager : MonoBehaviour
{
    public static QuestManager Instance;
    public QuestSO currentQuest;

    public event Action<QuestSO> OnQuestUpdated;

    void Awake()
    {
        if (Instance == null) Instance = this;
        else Destroy(gameObject);
    }

    public void SetQuest(QuestSO newQuest)
    {
        currentQuest = newQuest;
        currentQuest.isComplete = false;
        OnQuestUpdated?.Invoke(currentQuest);
    }

    public void CompleteQuest()
    {
        if (currentQuest != null)
        {
            currentQuest.isComplete = true;
            Debug.Log($"Quest Completed: {currentQuest.questName}");
            OnQuestUpdated?.Invoke(currentQuest);
            // Here you can add logic to move to the next quest
        }
    }

    public bool IsQuestTarget(GameObject obj)
    {
        return currentQuest != null && currentQuest.targetObject == obj;
    }
}

