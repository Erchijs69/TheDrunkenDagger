using UnityEngine;
using UnityEngine.SceneManagement; // Required for scene changing
using System.Collections;
using Unity.AI.Navigation.Samples;

interface IInteractable
{
    void Interact();
}

public class Interactor : MonoBehaviour
{
    public Transform InteractorSource;
    public float InteractRange = 3f;

    public LayerMask interactableLayers; // Set this to include Door, Elf, Cart layers

    private GameObject currentHitObject;

    void Update()
    {
        if (Input.GetKeyDown(KeyCode.E))
        {
            Ray ray = new Ray(InteractorSource.position + InteractorSource.forward * 0.5f, InteractorSource.forward);
            RaycastHit hit;

            Debug.DrawRay(InteractorSource.position, InteractorSource.forward * InteractRange, Color.red, 0.1f);

            if (Physics.Raycast(ray, out hit, InteractRange, interactableLayers))
            {
                currentHitObject = hit.collider.gameObject;

                IInteractable interactable = currentHitObject.GetComponent<IInteractable>();

                if (interactable != null)
                {
                    interactable.Interact();
                    Debug.Log($"Interacted with {currentHitObject.name}");
                }
                else
                {
                    Debug.LogWarning($"Hit object {currentHitObject.name} does not implement IInteractable.");
                }
            }
            else
            {
                currentHitObject = null;
                Debug.Log("Raycast hit: Nothing");
            }
        }
    }

    void OnDrawGizmos()
    {
        if (InteractorSource != null)
        {
            Gizmos.color = Color.green;
            Gizmos.DrawWireSphere(InteractorSource.position, InteractRange);
        }
    }
}


