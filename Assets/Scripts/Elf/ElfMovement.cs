using UnityEngine;
using UnityEngine.AI;
using System.Collections.Generic;
using System.Collections;

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

        private bool isUsingBowAnimations = false;
        private bool hasPlayedBowDraw = false;

        private bool isIdlePlaying = false;
        private float idleCheckTimer = 0f;
        private float idleCheckInterval = 5f;

        public Transform shootPoint; 
        public GameObject arrowPrefab; 

        private Transform shootingTarget;

        


        [Header("Shooting Settings")]
        public float arrowSpeed = 20f;
        public float maxShootingDistance = 100f; 
        public float arrowSpawnDelay = 0.5f;

        [Header("Quiver Settings")]
        public List<GameObject> quiverArrows = new List<GameObject>(); 

        private int maxArrows = 10;
        private int currentArrowCount;




        [SerializeField] private Animator animator;

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
            animator = GetComponentInChildren<Animator>();
            agent = GetComponent<NavMeshAgent>();
            agent.isStopped = true;
            dialogueManager = FindObjectOfType<DialogueManager>();
            currentArrowCount = maxArrows;


            followStopQuest = QuestManager.Instance.GetQuestByName("Stack up on Potions and Embark");

            if (playerTransform == null)
            {
                GameObject playerGO = GameObject.FindGameObjectWithTag("Player");
                if (playerGO != null)
                {
                    playerTransform = playerGO.transform;
                }
            }
        }


        void Update()
        {
            
            if (isFollowingPlayer && followStopQuest != null && followStopQuest.isComplete)
            {
                StopFollowingPlayerAndResume();
                return;
            }

           
            if (isFollowingPlayer)
            {
                FollowPlayerUpdate();
                return;
            }

            if (!isFollowingPlayer && !hasStartedFollowing && followStartQuest != null)
            {
                if (followStartQuest.isComplete)
                {
                    StartFollowingPlayer();
                    hasStartedFollowing = true;
                }
            }

            if (canMove && agent.remainingDistance <= agent.stoppingDistance && !agent.pathPending)
            {
                OnReachWaypoint();
            }

            if (isFollowingPlayer && !animator.GetBool("useBowSet"))
            {
                animator.SetBool("useBowSet", true);
            }


            UpdateAnimation();
        }


       public void ShootAtTarget(Transform target)
{
    if (target == null || animator == null)
        return;

    if (currentArrowCount <= 0)
    {
        Debug.Log("No arrows left!");
        return; 
    }

    float distance = Vector3.Distance(transform.position, target.position);
    if (distance > maxShootingDistance)
        return;

    
    Vector3 direction = (target.position - transform.position).normalized;
    direction.y = 0f;
    transform.rotation = Quaternion.LookRotation(direction);

   
    animator.SetTrigger("shootTrigger");

    
    ConsumeArrow();

  
    StartCoroutine(DelayedSpawnArrow(target));
}


private void ConsumeArrow()
{
    if (currentArrowCount <= 0)
        return;

    currentArrowCount--;

    if (quiverArrows != null && quiverArrows.Count > 0)
    {
       
        GameObject arrowGO = quiverArrows[quiverArrows.Count - 1];
        quiverArrows.RemoveAt(quiverArrows.Count - 1);

        if (arrowGO != null)
        {
            Destroy(arrowGO);
        }
    }
}


        private IEnumerator DelayedSpawnArrow(Transform target)
        {
            yield return new WaitForSeconds(arrowSpawnDelay);
            SpawnArrow(target);
        }

public void SpawnArrow(Transform target)
{
    if (arrowPrefab == null || shootPoint == null || target == null)
        return;

    Vector3 direction = (target.position - shootPoint.position).normalized;
    GameObject arrow = Instantiate(arrowPrefab, shootPoint.position, Quaternion.LookRotation(direction));

    ArrowProjectile projectile = arrow.GetComponent<ArrowProjectile>();
    if (projectile != null)
    {
        projectile.speed = arrowSpeed;
        projectile.maxDistance = maxShootingDistance;
    }
}



        void FollowPlayerUpdate()
        {
            if (playerTransform == null)
                return;

            float distanceToPlayer = Vector3.Distance(transform.position, playerTransform.position);


            if (distanceToPlayer > 25f)
            {
                Vector3 teleportOffset = -playerTransform.forward * 3f;
                Vector3 teleportPosition = playerTransform.position + teleportOffset;

                NavMeshHit hit;
                if (NavMesh.SamplePosition(teleportPosition, out hit, 2f, NavMesh.AllAreas))
                {
                    agent.Warp(hit.position); 
                }
                else
                {
                  
                    agent.Warp(teleportPosition);
                }
            }

            agent.isStopped = false;
            agent.destination = playerTransform.position;

            if (nextQuestToWaitFor != null && nextQuestToWaitFor.isComplete)
            {
                isFollowingPlayer = false;
                nextQuestToWaitFor = null;
                canMove = true;
                agent.isStopped = true;
                MoveToNextWaypoint();
            }

            UpdateAnimation();
        }


        public void MoveToNextWaypoint()
        {
            if (isDialogueActive) return;
            if (waypoints.Count == 0) return;

            if (currentWaypointIndex >= waypoints.Count)
            {
                StopMovement();
                return;
            }

            agent.destination = waypoints[currentWaypointIndex].position;
            agent.isStopped = false;
            canMove = true;
            currentWaypointIndex++;
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
            if (currentWaypointIndex > waypoints.Count || currentWaypointIndex == 0) return;

            int prevIndex = currentWaypointIndex - 1;

            Transform waypointTransform = waypoints[prevIndex];
            Waypoint waypoint = waypointTransform.GetComponent<Waypoint>();

            if (waypoint != null)
            {
                currentRequiredQuest = waypoint.requiredQuest;

                if (waypoint.requiresQuestCompletion && currentRequiredQuest != null && !currentRequiredQuest.isComplete)
                {
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
                        StartFollowingPlayer();
                        return;
                    }
                    else
                    {
                        MoveToNextWaypoint();
                        return;
                    }
                }

                questRequiredAndIncomplete = false;
                SetupDialogue(waypoint);

                canMove = false;
                agent.isStopped = true;
                waitingForPlayer = true;
            }
        }

        // ANIMATION
        private void UpdateAnimation()
        {
            if (animator == null) return;

            bool isMoving = agent.velocity.magnitude > 0.1f && !agent.isStopped;
            animator.SetBool("isWalking", isMoving);

            if (!isMoving)
            {
                idleCheckTimer += Time.deltaTime;

                if (idleCheckTimer >= idleCheckInterval)
                {
                    float chance = Random.Range(0f, 1f);
                    if (chance <= 0.25f)
                    {
                        animator.SetBool("isIdlePlaying", true);
                        StartCoroutine(ResetIdleFlagAfterAnimation());
                    }
                }
            }
            else
            {
                animator.SetBool("isIdlePlaying", false);
                idleCheckTimer = 0f;
            }

        }

        private IEnumerator ResetIdleFlagAfterAnimation()
        {
            yield return new WaitForSeconds(2f);
            animator.SetBool("isIdlePlaying", false);
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
                        return chain[i + 1];
                    }
                    else
                    {
                        return null;
                    }
                }
            }

            return null;
        }

        void StartFollowingPlayer()
        {
            isFollowingPlayer = true;
            canMove = false;
            agent.isStopped = true; 
            waitingForPlayer = false;
            questRequiredAndIncomplete = false;

            agent.speed = 5f;

            if (!hasPlayedBowDraw)
            {
                animator.SetTrigger("bowDrawTrigger");
                StartCoroutine(WaitForBowDrawAnimation());
            }
            else
            {
                ActivateBowAnimationSet();
                agent.isStopped = false;
            }

            agent.stoppingDistance = 6f;
        }

        void StopFollowingPlayerAndResume()
        {
            isFollowingPlayer = false;
            canMove = true;
            agent.isStopped = true;
            agent.stoppingDistance = 0f;


            animator.SetBool("useBowSet", false);

            MoveToNextWaypoint();
        }


        private void SetupDialogue(Waypoint waypoint)
        {
            if (dialogueManager != null)
            {
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
                waitingForPlayer = true;
                return;
            }

            if (questRequiredAndIncomplete && currentRequiredQuest != null && currentRequiredQuest.isComplete)
            {
                questRequiredAndIncomplete = false;
                waitingForPlayer = false;
                MoveToNextWaypoint();
                return;
            }

            if (questRequiredAndIncomplete && currentRequiredQuest != null && !currentRequiredQuest.isComplete)
            {
                waitingForPlayer = true;
                return;
            }

            waitingForPlayer = false;
            MoveToNextWaypoint();
        }

        public bool AgentIsMoving()
        {
            return agent != null && agent.velocity.magnitude > 0.1f && !agent.isStopped;
        }

        private IEnumerator WaitForBowDrawAnimation()
        {
           
            while (!animator.GetCurrentAnimatorStateInfo(0).IsName("DrawBow"))
            {
                yield return null;
            }

            
            float duration = animator.GetCurrentAnimatorStateInfo(0).length;
            yield return new WaitForSeconds(duration);

            hasPlayedBowDraw = true;
            ActivateBowAnimationSet();
            agent.isStopped = false;
        }

        private void ActivateBowAnimationSet()
        {
            isUsingBowAnimations = true;
            animator.SetBool("useBowSet", true);
        }

        private void PlayRandomIdleVariation()
        {
          
            animator.SetTrigger("idleVariationTrigger");
        }

        public void LookAtTarget(Transform target)
        {
            Vector3 direction = (target.position - transform.position).normalized;
            direction.y = 0f;
            if (direction != Vector3.zero)
            {
                transform.rotation = Quaternion.LookRotation(direction);
            }
        }
    }

}










