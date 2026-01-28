using UnityEngine;
using System.Collections;

public class EnemyZoner : MonoBehaviour
{
    [Header("Vision")]
    public float visionRange = 8f;
    public LayerMask wallLayer;

    [Header("Movement")]
    public float patrolSpeed = 1.5f;
    public float retreatSpeed = 2.5f;
    public float rotationSpeed = 200f;
    public float patrolRadius = 5f;
    public float changeDirectionTime = 2f;

    [Header("Shooting")]
    public GameObject bulletPrefab;
    public Transform firePoint;
    public float bulletSpeed = 6f;
    public float fireCooldown = 1.2f;

    Rigidbody2D rb;
    Transform player;

    Vector2 patrolCenter;
    Vector2 patrolDirection;
    Vector2 moveDirection;

    float directionTimer;
    float fireTimer;

    void Start()
    {
        rb = GetComponent<Rigidbody2D>();
        rb.gravityScale = 0;

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

        // EXACT SAME VISION LOGIC AS RED TRIANGLE
        bool playerInSight =
            distance <= visionRange &&
            Physics2D.Raycast(rb.position, toPlayer.normalized, distance, wallLayer) == false;

        if (playerInSight)
            ZonerBehavior(toPlayer);
        else
            PatrolBehavior();

        rb.linearVelocity = moveDirection * (playerInSight ? retreatSpeed : patrolSpeed);

        // Rotation logic
        if (playerInSight)
            RotateTowards(toPlayer.normalized);   // look at player
        else
            RotateTowards(moveDirection);         // look where moving
    }

    // ================= ZONER MODE =================
    void ZonerBehavior(Vector2 toPlayer)
    {
        moveDirection = (-toPlayer).normalized;

        fireTimer -= Time.fixedDeltaTime;
        if (fireTimer <= 0f)
        {
            fireTimer = fireCooldown;
            Shoot(toPlayer.normalized);
        }

        patrolCenter = rb.position;
    }

    // ================= PATROL MODE =================
    void PatrolBehavior()
    {
        directionTimer -= Time.fixedDeltaTime;

        Vector2 nextPos = rb.position + patrolDirection * patrolSpeed * Time.fixedDeltaTime;

        if (directionTimer <= 0f || Vector2.Distance(nextPos, patrolCenter) > patrolRadius)
            PickNewPatrolDirection();

        moveDirection = patrolDirection;
    }

    // ================= WALL COLLISION =================
    void OnCollisionEnter2D(Collision2D collision)
    {
        if (((1 << collision.gameObject.layer) & wallLayer) == 0)
            return;

        Vector2 normal = collision.contacts[0].normal;

        moveDirection = Vector2.Reflect(moveDirection, normal).normalized;
        patrolDirection = moveDirection;
        directionTimer = changeDirectionTime;
    }

    // ================= HELPERS =================
    void Shoot(Vector2 dir)
    {
        GameObject bullet = Instantiate(bulletPrefab, firePoint.position, Quaternion.identity);
        Rigidbody2D brb = bullet.GetComponent<Rigidbody2D>();
        if (brb)
            brb.linearVelocity = dir * bulletSpeed;
    }

    void RotateTowards(Vector2 dir)
    {
        if (dir.sqrMagnitude < 0.01f) return;

        float targetAngle = Mathf.Atan2(dir.y, dir.x) * Mathf.Rad2Deg - 90f;
        rb.rotation = Mathf.MoveTowardsAngle(
            rb.rotation,
            targetAngle,
            rotationSpeed * Time.fixedDeltaTime
        );
    }

    void PickNewPatrolDirection()
    {
        float a = Random.Range(0f, 360f) * Mathf.Deg2Rad;
        patrolDirection = new Vector2(Mathf.Cos(a), Mathf.Sin(a)).normalized;
        directionTimer = changeDirectionTime;
    }

    void OnDrawGizmosSelected()
    {
        Gizmos.color = Color.red;
        Gizmos.DrawWireSphere(transform.position, visionRange);
    }
}
