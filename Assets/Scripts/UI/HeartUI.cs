using UnityEngine;
using UnityEngine.UI;
public class HeartUI : MonoBehaviour
{
    public GameObject[] heartParents; // Each parent has two children: Full and Empty

    public void UpdateHearts(int deathCount)
    {
        int livesLeft = Mathf.Clamp(3 - deathCount, 0, 3);

        for (int i = 0; i < heartParents.Length; i++)
        {
            Transform full = heartParents[i].transform.Find("Full");
            Transform empty = heartParents[i].transform.Find("Empty");

            if (full != null && empty != null)
            {
                full.gameObject.SetActive(i < livesLeft);
                empty.gameObject.SetActive(i >= livesLeft);
            }
        }
    }
}


