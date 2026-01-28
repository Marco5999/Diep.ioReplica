using UnityEngine;

public class EnemyChase : MonoBehaviour
{
    [Header("Vision & Player")]
    public float visionRange = 8f;
    public LayerMask wallLayer;
    private Transform player;

    [Header("Movement")]
    public float chaseSpeed = 3f;      // Speed when chasing player
    public float patrolSpeed = 1.5f;   // Slower speed when patrolling
    public float rotationSpeed = 200f; // Degrees/sec for smooth facing
    public float patrolRadius = 5f;    // How far enemy can roam from patrol center
    public float changeDirectionTime = 2f; // How often to pick a new patrol direction

    private Rigidbody2D rb;
    private Vector2 velocity;
    private Vector2 patrolCenter;
    private Vector2 patrolDirection;
    private float directionTimer;

    void Start()
    {
        rb = GetComponent<Rigidbody2D>();

        // Find player at runtime
        GameObject playerObj = GameObject.FindGameObjectWithTag("Player");
        if (playerObj != null)
            player = playerObj.transform;

        // Start patrol center at current position
        patrolCenter = rb.position;
        PickNewPatrolDirection();
    }

    void FixedUpdate()
    {
        if (player == null)
            return;

        Vector2 playerPos = (Vector2)player.position;
        Vector2 toPlayer = playerPos - rb.position;
        float distance = toPlayer.magnitude;

        // Check if player is in sight and not blocked by walls
        bool playerInSight = distance <= visionRange &&
            Physics2D.Raycast(rb.position, toPlayer.normalized, distance, wallLayer) == false;

        if (playerInSight)
        {
            // --- Chase mode ---
            velocity = toPlayer.normalized * chaseSpeed;
            patrolCenter = rb.position; // update patrol center for when player leaves
        }
        else
        {
            // --- Patrol mode ---
            directionTimer -= Time.fixedDeltaTime;

            Vector2 nextPos = rb.position + patrolDirection * patrolSpeed * Time.fixedDeltaTime;

            // Pick new direction if timer ran out or about to leave patrol circle
            if (directionTimer <= 0f || Vector2.Distance(nextPos, patrolCenter) > patrolRadius)
            {
                PickNewPatrolDirection();
            }

            velocity = patrolDirection * patrolSpeed;

            // Wall bounce check
            RaycastHit2D hit = Physics2D.Raycast(rb.position, patrolDirection, 0.5f, wallLayer);
            if (hit.collider != null)
            {
                patrolDirection = -patrolDirection;
                directionTimer = changeDirectionTime;
            }
        }

        // Apply velocity
        rb.linearVelocity = velocity;

        // Smooth rotation to face movement direction
        if (velocity.sqrMagnitude > 0.01f)
        {
            float targetAngle = Mathf.Atan2(velocity.y, velocity.x) * Mathf.Rad2Deg - 90f;
            float angle = Mathf.MoveTowardsAngle(rb.rotation, targetAngle, rotationSpeed * Time.fixedDeltaTime);
            rb.rotation = angle;
        }
    }

    void PickNewPatrolDirection()
    {
        float angle = Random.Range(0f, 360f) * Mathf.Deg2Rad;
        patrolDirection = new Vector2(Mathf.Cos(angle), Mathf.Sin(angle)).normalized;
        directionTimer = changeDirectionTime;
    }

    void OnDrawGizmosSelected()
    {
        Gizmos.color = Color.cyan;
        Gizmos.DrawWireSphere(patrolCenter, patrolRadius);

        Gizmos.color = Color.red;
        if (rb != null) Gizmos.DrawRay(rb.position, velocity);
    }
}
