using UnityEngine;

public class TavernExitTrigger : MonoBehaviour
{
    private void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("Player"))
        {
            Debug.Log("PLAYER EXIT - Starting Day Ambience");
            SoundManager soundManager = FindObjectOfType<SoundManager>();
            if (soundManager != null)
            {
                soundManager.SwitchToDayAmbience();
            }
            else
            {
                Debug.LogError("SoundManager not found in scene!");
            }
        }
    }
}