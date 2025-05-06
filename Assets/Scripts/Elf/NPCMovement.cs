using UnityEngine;
using UnityEngine.AI;
using System.Collections.Generic;

namespace Unity.AI.Navigation.Samples
{
    /// <summary>
    /// Moves the NavMeshAgent between predefined points, allowing dynamic addition of new points
    /// </summary>
    [RequireComponent(typeof(NavMeshAgent))]
    public class PatrolPath : MonoBehaviour
    {
        public List<Transform> waypoints = new List<Transform>(); // List of points to move between
        private NavMeshAgent agent;
        private int currentWaypointIndex = 0;

        void Start()
        {
            agent = GetComponent<NavMeshAgent>();
            MoveToNextWaypoint();
        }

        void Update()
        {
            if (agent.remainingDistance <= agent.stoppingDistance && !agent.pathPending)
            {
                MoveToNextWaypoint();
            }
        }

        void MoveToNextWaypoint()
        {
            if (waypoints.Count == 0) return;
            agent.destination = waypoints[currentWaypointIndex].position;
            currentWaypointIndex = (currentWaypointIndex + 1) % waypoints.Count;
        }

        public void AddWaypoint(Transform newWaypoint)
        {
            waypoints.Add(newWaypoint);
        }
    }
}
