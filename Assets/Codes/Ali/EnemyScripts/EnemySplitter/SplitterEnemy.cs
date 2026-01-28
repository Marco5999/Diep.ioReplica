using UnityEngine;

public class SplitterEnemy : MonoBehaviour
{
    [Header("Vision & Player")]
    public float visionRange = 8f;
    public LayerMask wallLayer;
    private Transform player;

    [Header("Movement")]
    public float chaseSpeed = 3f;
    public float patrolSpeed = 1.5f;
    public float rotationSpeed = 200f;
    public float patrolRadius = 5f;
    public float changeDirectionTime = 2f;

    [Header("Split Settings")]
    public GameObject miniPrefab;    // prefab for mini enemies
    public int miniCount = 2;        // number of mini enemies on death
    public float miniScale = 0.5f;   // scale of mini enemies
    

    private Rigidbody2D rb;
    private Vector2 patrolCenter;
    private Vector2 patrolDirection;
    private float directionTimer;

    void Start()
    {
        rb = GetComponent<Rigidbody2D>();

        GameObject p = GameObject.FindGameObjectWithTag("Player");
        if (p) player = p.transform;

        patrolCenter = rb.position;
        PickNewPatrolDirection();
    }

    void FixedUpdate()
    {
        if (!player) return;

        Vector2 toPlayer = (Vector2)player.position - rb.position;
        float distance = toPlayer.magnitude;

        // Player in sight and not blocked by wall?
        bool playerInSight = distance <= visionRange &&
                             Physics2D.Raycast(rb.position, toPlayer.normalized, distance, wallLayer) == false;

        Vector2 velocity;

        if (playerInSight)
        {
            // --- Chase / Move toward player ---
            velocity = toPlayer.normalized * chaseSpeed;

            // Update patrol center so enemy can resume patrol smoothly
            patrolCenter = rb.position;
        }
        else
        {
            // --- Patrol ---
            directionTimer -= Time.fixedDeltaTime;

            Vector2 nextPos = rb.position + patrolDirection * patrolSpeed * Time.fixedDeltaTime;
            if (directionTimer <= 0f || Vector2.Distance(nextPos, patrolCenter) > patrolRadius)
                PickNewPatrolDirection();

            // Wall bounce
            RaycastHit2D hit = Physics2D.Raycast(rb.position, patrolDirection, 0.5f, wallLayer);
            if (hit.collider != null)
            {
                patrolDirection = -patrolDirection;
                directionTimer = changeDirectionTime;
            }

            velocity = patrolDirection * patrolSpeed;

            // Stop tiny jitter
            if (velocity.sqrMagnitude < 0.01f)
                velocity = Vector2.zero;
        }

        // Apply velocity
        rb.linearVelocity = velocity;

        // Smooth rotation to face movement
        if (velocity.sqrMagnitude > 0.01f)
        {
            float targetAngle = Mathf.Atan2(velocity.y, velocity.x) * Mathf.Rad2Deg - 90f;
            rb.rotation = Mathf.MoveTowardsAngle(rb.rotation, targetAngle, rotationSpeed * Time.fixedDeltaTime);
        }
    }

    void PickNewPatrolDirection()
    {
        float angle = Random.Range(0f, 360f) * Mathf.Deg2Rad;
        patrolDirection = new Vector2(Mathf.Cos(angle), Mathf.Sin(angle)).normalized;
        directionTimer = changeDirectionTime;
    }

    // Called from Health.Die()
    public void SplitOnDeath()
    {
        if (miniPrefab == null) return;

        for (int i = 0; i < miniCount; i++)
        {
            GameObject mini = Instantiate(miniPrefab, transform.position, Quaternion.identity);
            mini.transform.localScale = Vector3.one * miniScale;
        }
    }
}
