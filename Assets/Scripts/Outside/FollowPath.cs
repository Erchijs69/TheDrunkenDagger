using UnityEngine;

public class SplineFollower : MonoBehaviour
{
    [Header("Spline Path")]
    public Transform pathParent;
    private Transform[] points;

    [Header("Movement")]
    public float speed = 5f;
    public bool loop = true;

    [Header("Optional Facing")]
    public Transform forwardFacingChild; 

    [Header("Rotation Offset")]
    public Vector3 rotationOffsetEuler = Vector3.zero; 

    private float t = 0f;

    void Start()
    {
        if (pathParent == null || pathParent.childCount < 4)
        {
            Debug.LogError("Need at least 4 path points for Catmull-Rom spline.");
            enabled = false;
            return;
        }

        points = new Transform[pathParent.childCount];
        for (int i = 0; i < pathParent.childCount; i++)
        {
            points[i] = pathParent.GetChild(i);
        }

        AlignToStartDirection();
    }

    void Update()
{
    t += speed * Time.deltaTime;

    int numPoints = points.Length;

    
    if (!loop)
    {
        float maxT = numPoints - 3;
        t = Mathf.Min(t, maxT);  
    }

    int segment = Mathf.FloorToInt(t);
    float localT = t - segment;

    
    if (!loop && segment >= numPoints - 3)
    {
        segment = numPoints - 4;
        localT = 1f; 
    }

   
    int p0 = (segment - 1 + numPoints) % numPoints;
    int p1 = (segment + 0) % numPoints;
    int p2 = (segment + 1) % numPoints;
    int p3 = (segment + 2) % numPoints;

   
    Vector3 currentPos = CatmullRom(points[p0].position, points[p1].position, points[p2].position, points[p3].position, localT);

   
    if (!loop && segment >= numPoints - 3)
    {
        currentPos = points[numPoints - 1].position;
    }

  
    transform.position = currentPos;

   
    Vector3 lookDir = Vector3.zero;
    if (!loop && segment < numPoints - 1)
    {
       
        Vector3 nextPoint = points[Mathf.Min(segment + 1, numPoints - 1)].position;
        lookDir = (nextPoint - currentPos).normalized;
    }
    else if (loop)
    {
        
        Vector3 nextPoint = points[Mathf.Min(segment + 1, numPoints - 1)].position;
        lookDir = (nextPoint - currentPos).normalized;
    }

    
    if (lookDir != Vector3.zero)
    {
        Quaternion baseRotation = Quaternion.LookRotation(lookDir);
        Quaternion offsetRotation = Quaternion.Euler(rotationOffsetEuler);
        transform.rotation = baseRotation * offsetRotation;

        if (forwardFacingChild != null)
            forwardFacingChild.rotation = baseRotation * offsetRotation;
    }

    
    if (loop && t >= numPoints)
        t -= numPoints;
}



    Vector3 CatmullRom(Vector3 p0, Vector3 p1, Vector3 p2, Vector3 p3, float t)
    {
        return 0.5f * (
            2f * p1 +
            (-p0 + p2) * t +
            (2f * p0 - 5f * p1 + 4f * p2 - p3) * t * t +
            (-p0 + 3f * p1 - 3f * p2 + p3) * t * t * t
        );
    }

    void AlignToStartDirection()
    {
        Vector3 initialPos = CatmullRom(points[0].position, points[0].position, points[1].position, points[2].position, 0f);
        Vector3 nextPos = points[2].position;
        Vector3 dir = (nextPos - initialPos).normalized;

        if (dir != Vector3.zero)
        {
            Quaternion baseRotation = Quaternion.LookRotation(dir);
            Quaternion offsetRotation = Quaternion.Euler(rotationOffsetEuler);
            transform.rotation = baseRotation * offsetRotation;
        }
    }
}




