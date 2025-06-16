using UnityEngine;

public class HeartUI : MonoBehaviour
{
    public GameObject[] hearts; 

    public void UpdateHearts(int deathCount)
    {
        for (int i = 0; i < hearts.Length; i++)
        {
            hearts[i].SetActive(i >= deathCount);
        }
    }
}



