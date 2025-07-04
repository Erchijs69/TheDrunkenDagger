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

    public HeartUI heartUI;

    private void Awake()
    {
        Managerinstance = this;

        if (player == null)
            player = FindObjectOfType<PlayerMovement>();

        SceneManager.sceneLoaded += OnSceneLoaded;
    }

    private void OnDestroy()
    {
        SceneManager.sceneLoaded -= OnSceneLoaded;
    }

    private void OnSceneLoaded(Scene scene, LoadSceneMode mode)
    {
        GameObject playerObj = GameObject.Find("Player");

        PlayerMovement[] players = FindObjectsOfType<PlayerMovement>();
        if (players.Length > 1)
        {
            foreach (var p in players)
            {
                if (p != playerObj.GetComponent<PlayerMovement>())
                {
                    Destroy(p.gameObject);
                }
            }
        }

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

        if (heartUI == null)
            heartUI = FindObjectOfType<HeartUI>();

        heartUI?.UpdateHearts(deathCount);
    }

    public void PlayerDied()
    {
        deathCount++;

        if (heartUI != null)
        {
            heartUI.UpdateHearts(deathCount);
        }
        else
        {
            Debug.LogWarning("HeartUI is not assigned!");
        }

        if (deathCount >= maxDeaths)
        {
            deathCount = 0;
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
        if (player != null && respawnPoint != null && ScreenFader.Instance != null)
        {
            // Disable the controller before moving
            player.controller.enabled = false;

            // Fade and move to respawn position
            ScreenFader.Instance.FadeAndTeleport(player.transform, respawnPoint.position, () =>
            {
                // After fading out and teleporting, set the correct rotation
                player.transform.rotation = respawnPoint.rotation;

                // Reactivate the controller after move and rotation
                player.controller.enabled = true;
            });
        }
        else
        {
            Debug.LogWarning("Missing reference for player, respawnPoint, or ScreenFader.");
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