using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class QuestInitializer : MonoBehaviour
{
    public QuestSO startingQuest;

    void Start()
    {
        QuestManager.Instance.SetQuest(startingQuest);
    }
}

