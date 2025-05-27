using UnityEngine;
using UnityEngine.AI;
using System.Collections.Generic;

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

        private bool hasStartedFollowing = false;


        private bool questRequiredAndIncomplete = false;
        private QuestSO currentRequiredQuest = null;

        public QuestSO relatedQuest;

        [Header("Follow Player Settings")]
        public Transform playerTransform;
        public QuestSO followStartQuest;
        private QuestSO followStopQuest;


        private bool isFollowingPlayer = false;
        private QuestSO nextQuestToWaitFor = null;

        void Start()
        {
            agent = GetComponent<NavMeshAgent>();
            agent.isStopped = true;
            dialogueManager = FindObjectOfType<DialogueManager>();

            followStopQuest = QuestManager.Instance.GetQuestByName("SPEED POTION");

            if (playerTransform == null)
            {
                GameObject playerGO = GameObject.FindGameObjectWithTag("Player");
                if (playerGO != null)
                {
                    playerTransform = playerGO.transform;
                    Debug.Log("Follower: Found player transform.");
                }
            }
        }

        void Update()
{
    Debug.Log($"Follower: Update running. isFollowingPlayer={isFollowingPlayer}, canMove={canMove}, currentWaypointIndex={currentWaypointIndex}");

    // Always check if we should stop following FIRST
    if (isFollowingPlayer && followStopQuest != null && followStopQuest.isComplete)
    {
        Debug.Log("Follower: followStopQuest complete. Calling StopFollowingPlayerAndResume.");
        StopFollowingPlayerAndResume();
        return;
    }

    // Only follow if follow hasn't been stopped above
    if (isFollowingPlayer)
    {
        Debug.Log($"Follower: Following player. Agent destination = {agent.destination}");
        FollowPlayerUpdate();
        return;
    }

    if (!isFollowingPlayer && !hasStartedFollowing && followStartQuest != null)
{
    Debug.Log($"Follower: Checking followStartQuest. IsComplete = {followStartQuest.isComplete}");
    if (followStartQuest.isComplete)
    {
        Debug.Log("Follower: followStartQuest is complete. Calling StartFollowingPlayer.");
        StartFollowingPlayer();
        hasStartedFollowing = true;
    }
}


    if (canMove && agent.remainingDistance <= agent.stoppingDistance && !agent.pathPending)
    {
        OnReachWaypoint();
    }
}



        void FollowPlayerUpdate()
{
    if (playerTransform == null)
    {
        Debug.Log("Follower: Player transform is null. Cannot follow.");
        return;
    }

    if (followStopQuest != null && followStopQuest.isComplete)
    {
        Debug.Log("Follower: followStopQuest complete during follow. Stopping.");
        StopFollowingPlayerAndResume();
        return;
    }

    agent.isStopped = false;
    agent.destination = playerTransform.position;

    if (nextQuestToWaitFor != null && nextQuestToWaitFor.isComplete)
    {
        Debug.Log("Follower: Next quest completed. Stopping follow and resuming waypoints.");
        isFollowingPlayer = false;
        nextQuestToWaitFor = null;
        canMove = true;
        agent.isStopped = true;
        MoveToNextWaypoint();
    }
}


        public void MoveToNextWaypoint()
        {
            if (isDialogueActive) return;
            if (waypoints.Count == 0) return;

            if (currentWaypointIndex >= waypoints.Count)
                {
                    Debug.Log("Follower: Final waypoint reached. Stopping movement.");
                    StopMovement(); // <- This disables canMove
                    return;
                }


            Debug.Log($"Follower: Moving to waypoint {currentWaypointIndex}: {waypoints[currentWaypointIndex].name}");
            agent.destination = waypoints[currentWaypointIndex].position;
            agent.isStopped = false;
            canMove = true;
            currentWaypointIndex++;
        }

        public void StartMovement()
        {
            Debug.Log($"Follower: StartMovement called. canMove={canMove}, waypoints={waypoints.Count}");
            if (!canMove && waypoints.Count > 0)
            {
                waitingForPlayer = false;
                MoveToNextWaypoint();
            }
        }

        public void StopMovement()
        {
            Debug.Log("Follower: Stopping movement.");
            canMove = false;
            agent.isStopped = true;
        }

        public void OnReachWaypoint()
        {
            if (currentWaypointIndex > waypoints.Count || currentWaypointIndex == 0) return;

            int prevIndex = currentWaypointIndex - 1;
            Debug.Log($"Follower: Reached waypoint {prevIndex}: {waypoints[prevIndex].name}");

            Transform waypointTransform = waypoints[prevIndex];
            Waypoint waypoint = waypointTransform.GetComponent<Waypoint>();

            if (waypoint != null)
            {
                currentRequiredQuest = waypoint.requiredQuest;

                if (waypoint.requiresQuestCompletion && currentRequiredQuest != null && !currentRequiredQuest.isComplete)
                {
                    Debug.Log($"Follower: Quest '{currentRequiredQuest.questName}' required but incomplete. Waiting.");
                    StopMovement();
                    questRequiredAndIncomplete = true;
                    waitingForPlayer = true;
                    SetupDialogue(waypoint);
                    return;
                }

                if (waypoint.requiresQuestCompletion && currentRequiredQuest != null && currentRequiredQuest.isComplete)
                {
                    nextQuestToWaitFor = GetNextQuestAfter(currentRequiredQuest);
                    if (nextQuestToWaitFor != null)
                    {
                        Debug.Log($"Follower: Required quest '{currentRequiredQuest.questName}' complete. Starting follow until '{nextQuestToWaitFor.questName}' is done.");
                        StartFollowingPlayer();
                        return;
                    }
                    else
                    {
                        Debug.Log("Follower: No next quest found. Proceeding to next waypoint.");
                        MoveToNextWaypoint();
                        return;
                    }
                }

                questRequiredAndIncomplete = false;
                SetupDialogue(waypoint);

                canMove = false;
                agent.isStopped = true;
                waitingForPlayer = true;

                Debug.Log("Follower: Waiting for player interaction at waypoint.");
            }
        }

        private QuestSO GetNextQuestAfter(QuestSO quest)
        {
            QuestManager qm = QuestManager.Instance;
            if (qm == null || qm.questChain == null || qm.questChain.quests == null) return null;

            QuestSO[] chain = qm.questChain.quests;

            for (int i = 0; i < chain.Length; i++)
            {
                if (chain[i] == quest)
                {
                    if (i + 1 < chain.Length)
                    {
                        Debug.Log($"Follower: Next quest after '{quest.questName}' is '{chain[i + 1].questName}'");
                        return chain[i + 1];
                    }
                    else
                    {
                        Debug.Log($"Follower: '{quest.questName}' is the last quest in the chain.");
                        return null;
                    }
                }
            }

            Debug.Log($"Follower: Quest '{quest.questName}' not found in quest chain.");
            return null;
        }

        void StartFollowingPlayer()
        {
            Debug.Log("Follower: Starting to follow player.");
            isFollowingPlayer = true;
            canMove = false;
            agent.isStopped = false;
            waitingForPlayer = false;
            questRequiredAndIncomplete = false;
            agent.stoppingDistance = 2f;
        }

        void StopFollowingPlayerAndResume()
        {
            Debug.Log("Follower: Stopping follow and resuming waypoint movement.");

            isFollowingPlayer = false;
            canMove = true;
            agent.isStopped = true;
            agent.stoppingDistance = 0f;
            MoveToNextWaypoint();
        }


        private void SetupDialogue(Waypoint waypoint)
        {
            if (dialogueManager != null)
            {
                Debug.Log("Follower: Setting up dialogue from waypoint.");
                dialogueManager.dialogueLines = waypoint.dialogueLines;
                dialogueManager.triggersQuestOnEnd = waypoint.triggersQuestOnEnd;
                dialogueManager.requiresQuestCompletion = waypoint.requiresQuestCompletion;
                dialogueManager.requiredQuest = waypoint.requiredQuest;
            }
        }

        public void Interact()
        {
            if ((waitingForPlayer || questRequiredAndIncomplete) && gameObject.CompareTag("Elf"))
            {
                Debug.Log("Follower: Player interacted. Starting dialogue.");
                dialogueManager.StartDialogue();
                waitingForPlayer = false;
                isDialogueActive = true;
            }
        }

        public void OnDialogueEnd()
        {
            isDialogueActive = false;

            if (dialogueManager != null && dialogueManager.dialogueWasCancelled)
            {
                Debug.Log("Follower: Dialogue cancelled. Elf stays.");
                waitingForPlayer = true;
                return;
            }

            if (questRequiredAndIncomplete && currentRequiredQuest != null && currentRequiredQuest.isComplete)
            {
                Debug.Log("Follower: Required quest now complete after dialogue. Proceeding.");
                questRequiredAndIncomplete = false;
                waitingForPlayer = false;
                MoveToNextWaypoint();
                return;
            }

            if (questRequiredAndIncomplete && currentRequiredQuest != null && !currentRequiredQuest.isComplete)
            {
                Debug.Log("Follower: Quest still incomplete. Elf stays.");
                waitingForPlayer = true;
                return;
            }

            Debug.Log("Follower: Dialogue finished. Proceeding to next waypoint.");
            waitingForPlayer = false;
            MoveToNextWaypoint();
        }
    }  
}









