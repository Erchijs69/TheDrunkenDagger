using UnityEngine;

public class ArrowProjectile : MonoBehaviour
{
    public float speed = 20f;
    public float maxDistance = 100f;

    private Vector3 startPosition;
    private Rigidbody rb;

    private float lifeDelay = 0.1f; 
    private float timeSinceSpawn;

    void Start()
    {
        startPosition = transform.position;
        rb = GetComponent<Rigidbody>();

        if (rb != null)
        {
            rb.velocity = transform.forward * speed;
            rb.useGravity = false;
            rb.drag = 0f;
        }
        timeSinceSpawn = 0f;
    }

    void Update()
    {
        timeSinceSpawn += Time.deltaTime;

        if (Vector3.Distance(startPosition, transform.position) >= maxDistance)
        {
            Destroy(gameObject);
        }
    }

    private void OnTriggerEnter(Collider other)
    {
       
        if (timeSinceSpawn < lifeDelay)
            return;

        BaseEnemy enemy = other.GetComponentInParent<BaseEnemy>();
        if (enemy != null)
        {
            Destroy(enemy.gameObject);
            Destroy(gameObject);
        }
        else
        {
            
            Destroy(gameObject);
        }
    }
}


