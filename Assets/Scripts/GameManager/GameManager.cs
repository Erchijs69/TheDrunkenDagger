using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class GameManager : MonoBehaviour
{
    public static GameManager Managerinstance;
    public PlayerMovement player;
    public Transform respawnPoint;
    public int deathCount = 0;
    public int maxDeaths = 3;

    private void Awake()
    {
        // Just assign the static reference; allow duplicates on scene load
        Managerinstance = this;

        // Find player at start (if scene already loaded)
        if (player == null)
        {
            player = FindObjectOfType<PlayerMovement>();
        }

        // Subscribe to scene loaded event
        SceneManager.sceneLoaded += OnSceneLoaded;
    }

    private void OnDestroy()
    {
        // Unsubscribe when destroyed to avoid leaks
        SceneManager.sceneLoaded -= OnSceneLoaded;
    }

    private void OnSceneLoaded(Scene scene, LoadSceneMode mode)
    {
        GameObject playerObj = GameObject.Find("Player");
        if (playerObj != null)
        {
            player = playerObj.GetComponent<PlayerMovement>();
            if (player == null)
                Debug.LogWarning("PlayerMovement script not found on Player.");
        }
        else
        {
            Debug.LogWarning("Player GameObject not found in scene.");
        }
    }

    public void PlayerDied()
    {
        deathCount++;
        if (deathCount >= maxDeaths)
        {
            deathCount = 0; // Optional: reset counter on scene restart
            SceneManager.LoadScene(SceneManager.GetActiveScene().buildIndex);
        }
        else
        {
            RespawnPlayer();
            ResetAllEnemies();
        }
    }

    public void RespawnPlayer()
    {
        if (player != null && respawnPoint != null)
        {
            player.controller.enabled = false; // Disable to safely reposition
            player.transform.position = respawnPoint.position;
            player.transform.rotation = respawnPoint.rotation;
            player.controller.enabled = true; // Re-enable for normal movement
        }
    }

    private void ResetAllEnemies()
{
    EnemyMultiPatrol[] enemies = FindObjectsOfType<EnemyMultiPatrol>();
    foreach (var enemy in enemies)
    {
        enemy.ResetEnemy();
    }
}
}





