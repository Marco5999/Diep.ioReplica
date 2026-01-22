using UnityEngine;

public class EnemyChase : MonoBehaviour
{
    [Header("Vision")]
    public float visionRange = 8f;
    public LayerMask wallLayer;

    [Header("Movement")]
    public float moveSpeed = 3f;
    public float rotationSpeed = 200f; // Degrees per second

    Rigidbody2D rb;
    Transform player;

    void Start()
    {
        rb = GetComponent<Rigidbody2D>();

        // Find player at runtime (prefab-safe)
        GameObject playerObj = GameObject.FindGameObjectWithTag("Player");
        if (playerObj != null)
            player = playerObj.transform;
    }

    void FixedUpdate()
    {
        if (player == null)
            return;

        Vector2 toPlayer = player.position - transform.position;
        float distance = toPlayer.magnitude;

        // Stop if player is out of vision
        if (distance > visionRange)
        {
            rb.linearVelocity = Vector2.zero;
            return;
        }

        // Raycast to check if walls block view
        RaycastHit2D hit = Physics2D.Raycast(
            transform.position,
            toPlayer.normalized,
            distance,
            wallLayer
        );

        // Debug line
        Debug.DrawRay(transform.position, toPlayer, Color.red);

        if (hit.collider != null)
        {
            rb.linearVelocity = Vector2.zero;
            return;
        }

        // Move toward player
        rb.linearVelocity = toPlayer.normalized * moveSpeed;

        // --- NEW: Rotate to face player ---
        float targetAngle = Mathf.Atan2(toPlayer.y, toPlayer.x) * Mathf.Rad2Deg - 90f; // -90 if your sprite points up
        float angle = Mathf.MoveTowardsAngle(rb.rotation, targetAngle, rotationSpeed * Time.fixedDeltaTime);
        rb.rotation = angle;
    }
}
