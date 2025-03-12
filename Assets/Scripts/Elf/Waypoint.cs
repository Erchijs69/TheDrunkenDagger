using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Unity.AI.Navigation.Samples;

public class Waypoint : MonoBehaviour
{
    public string[] dialogueLines;  // Dialogue lines for this waypoint
    public bool useStopForSeconds = false;  // Whether to stop at this waypoint for a few seconds
    public float stopDuration = 3f;  // Duration to stop at this waypoint
}

