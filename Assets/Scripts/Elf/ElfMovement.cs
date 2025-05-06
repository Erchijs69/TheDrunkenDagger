using UnityEngine;
using UnityEngine.AI;
using System.Collections.Generic;
using Unity.AI.Navigation.Samples;

namespace Unity.AI.Navigation.Samples
{
    [RequireComponent(typeof(NavMeshAgent))]
    public class ElfMovement : MonoBehaviour, IInteractable
    {
        public List<Transform> waypoints = new List<Transform>();
        private NavMeshAgent agent;
        private int currentWaypointIndex = 0;
        private bool canMove = false;
        public bool waitingForPlayer = false;
        private DialogueManager dialogueManager;
        public bool isDialogueActive = false;

        void Start()
        {
            agent = GetComponent<NavMeshAgent>();
            agent.isStopped = true;
            dialogueManager = FindObjectOfType<DialogueManager>();
        }

        void Update()
        {
            if (canMove && agent.remainingDistance <= agent.stoppingDistance && !agent.pathPending)
            {
                OnReachWaypoint();
            }
        }

         public void MoveToNextWaypoint()
        {
            if (isDialogueActive) return;
            if (waypoints.Count == 0) return;

            // Stop moving if at the last waypoint
            if (currentWaypointIndex >= waypoints.Count - 1)
            {
                StopMovement();
                Debug.Log("Elf has reached the final waypoint and will stop.");
                return;
            }

            agent.destination = waypoints[currentWaypointIndex].position;
            agent.isStopped = false;
            canMove = true;

            currentWaypointIndex++; // Move to the next waypoint normally
        }
        public void StartMovement()
        {
            if (!canMove && waypoints.Count > 0)
            {
                waitingForPlayer = false;
                MoveToNextWaypoint();
            }
        }

        public void StopMovement()
        {
            canMove = false;
            agent.isStopped = true;
        }

        public void OnReachWaypoint()
        {
            Waypoint waypoint = waypoints[currentWaypointIndex].GetComponent<Waypoint>();

            if (waypoint != null && dialogueManager != null)
            {
                dialogueManager.dialogueLines = waypoint.dialogueLines;

                canMove = false;
                agent.isStopped = true;

                waitingForPlayer = true;
                isDialogueActive = false;
            }
        }

        public void Interact()
        {
            if (waitingForPlayer && gameObject.CompareTag("Elf"))
            {
                dialogueManager.StartDialogue();
                waitingForPlayer = false;
                isDialogueActive = true;
            }
        }

        public void OnDialogueEnd()
        {
            isDialogueActive = false;
            MoveToNextWaypoint();
        }
    }
}







