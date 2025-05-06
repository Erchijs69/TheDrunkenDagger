using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class IngredientSpawner : MonoBehaviour
{
    public GameObject ingredientPrefab;  // The prefab of the ingredient
    public LayerMask interactionLayer;  // Layer mask to detect player interaction
    public float interactionRange = 3f;  // Range to interact with the cube
    public KeyCode spawnKey = KeyCode.E;  // The key to trigger ingredient spawn

    public Vector3 spawnOffset = new Vector3(0, -1, 0);  // Offset for the spawn position (spawn below the cube)

    private Vector3 lastSpawnPosition;  // Store the last position of the spawned ingredient
    private bool canSpawn = false;  // Whether the cube is in range to spawn an ingredient

    void Start()
    {
        // Initialize lastSpawnPosition to the cube's current position
        lastSpawnPosition = transform.position;
    }

    void Update()
    {
        CheckForInteraction();

        if (canSpawn && Input.GetKeyDown(spawnKey))
        {
            SpawnIngredient();
        }
    }

    void CheckForInteraction()
    {
        RaycastHit hit;
        if (Physics.Raycast(Camera.main.transform.position, Camera.main.transform.forward, out hit, interactionRange, interactionLayer))
        {
            if (hit.collider.gameObject == gameObject)
            {
                canSpawn = true;
            }
            else
            {
                canSpawn = false;
            }
        }
        else
        {
            canSpawn = false;
        }
    }

    void SpawnIngredient()
    {
        if (ingredientPrefab != null)
        {
            // Calculate the spawn position with the offset (below the cube)
            Vector3 spawnPosition = transform.position + spawnOffset;

            GameObject newIngredient = Instantiate(ingredientPrefab, spawnPosition, Quaternion.identity);
            Ingredient ingredientScript = newIngredient.GetComponent<Ingredient>();

            if (ingredientScript != null)
            {
                // Store the spawn position of the new ingredient for future spawns
                lastSpawnPosition = newIngredient.transform.position;
            }

            Debug.Log("Ingredient spawned at " + spawnPosition);
        }
        else
        {
            Debug.LogError("No ingredient prefab assigned!");
        }
    }
}

