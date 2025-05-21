using UnityEngine;
using System.Collections.Generic;

public class BuffUIManager : MonoBehaviour
{
    [System.Serializable]
    public class BuffSlot
    {
        public string buffName;
        public BuffUI buffUI;
        public Sprite buffIcon;
    }

    public List<BuffSlot> buffSlots;

    // Called from PlayerBuffs when a potion is consumed
    public void ShowBuff(string buffName, float duration)
    {
        foreach (BuffSlot slot in buffSlots)
        {
            if (slot.buffName == buffName)
            {
                slot.buffUI.ActivateBuff(slot.buffIcon, duration);
                return;
            }
        }

        Debug.LogWarning("No UI slot found for buff: " + buffName);
    }
}


