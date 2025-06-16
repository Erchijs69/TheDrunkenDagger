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

    }
}


