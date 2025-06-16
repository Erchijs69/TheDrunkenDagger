using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class IngredientSpawner : MonoBehaviour
{
    public GameObject ingredientPrefab;
    public LayerMask interactionLayer;
    public float interactionRange = 3f;
    public KeyCode spawnKey = KeyCode.E;
    public Vector3 spawnOffset = new Vector3(0, -1, 0);

    private Vector3 lastSpawnPosition;
    private bool canSpawn = false;

    void Start()
    {
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
            Vector3 spawnPosition = transform.position + spawnOffset;

            GameObject newIngredient = Instantiate(ingredientPrefab, spawnPosition, Quaternion.identity);
            Ingredient ingredientScript = newIngredient.GetComponent<Ingredient>();

            if (ingredientScript != null)
            {
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


