using UnityEngine;
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

    public LayerMask doorLayer;
    public LayerMask elfLayer;

    private GameObject currentHitObject;

    void Update()
    {
        if (Input.GetKeyDown(KeyCode.E))
        {
            Ray r = new Ray(InteractorSource.position + InteractorSource.forward * 0.5f, InteractorSource.forward);
            RaycastHit hitInfo;
            Debug.DrawRay(InteractorSource.position, InteractorSource.forward * InteractRange, Color.red, 0.1f);

            currentHitObject = null;

            if (Physics.Raycast(r, out hitInfo, InteractRange, doorLayer))
            {
                currentHitObject = hitInfo.collider.gameObject;
                Door door = currentHitObject.GetComponent<Door>();
                if (door != null)
                {
                    door.Interact();
                    Debug.Log("Raycast hit a Door: " + currentHitObject.name);
                    return;
                }
            }
            else if (Physics.Raycast(r, out hitInfo, InteractRange, elfLayer))
            {
                currentHitObject = hitInfo.collider.gameObject;
                ElfMovement elf = currentHitObject.GetComponent<ElfMovement>();
                if (elf != null && elf.waitingForPlayer)
                {
                    Debug.Log("Raycast hit an Elf: " + currentHitObject.name);
                    elf.Interact();
                }
            }

            if (currentHitObject == null)
            {
                Debug.Log("Raycast hit: Nothing");
            }
        }
    }

    void OnDrawGizmos()
    {
        Gizmos.color = Color.green;
        Gizmos.DrawWireSphere(InteractorSource.position, InteractRange);
    }
}
