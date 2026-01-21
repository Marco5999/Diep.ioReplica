using UnityEngine;
using System.Collections;

public class PlayerShooting : MonoBehaviour
{
    [Header("Shooting Setup")]
    public GameObject bulletPrefab;
    public Transform firePoint;
    public Transform barrel;
    public float bulletSpeed = 10f;

    [Header("Push-Only Recoil (Ice Glide - No Auto Return!)")]
    public float launchForce = 15f;      // Strong launch impulse
    public float glideDrag = 0.08f;      // Ultra-low drag for endless slip
    public bool isGliding = false;       // Flag: true = sliding, ignore WASD

    [Header("Barrel Click Detection")]
    public LayerMask barrelLayerMask = -1;  // Default: All layers. Set to Barrel layer for precision (optional)

    public float originalDrag = 3f;      // PUBLIC: Normal drag (set in Inspector too)

    private Rigidbody2D rb;
    private Collider2D barrelCollider;   // Cache for fast check
    private float cachedGlideDrag;

    void Start()
    {
        rb = GetComponent<Rigidbody2D>();
        
        // TOP-DOWN LOCKED
        rb.gravityScale = 0f;
        rb.freezeRotation = true;
        
        // Cache barrel collider (MUST HAVE BoxCollider2D on Barrel!)
        barrelCollider = barrel.GetComponent<Collider2D>();
        if (barrelCollider == null)
        {
            Debug.LogError("Barrel needs a Collider2D (e.g. BoxCollider2D, NOT Trigger) for mouse-over shooting!");
        }
        
        rb.linearDamping = originalDrag;
        cachedGlideDrag = glideDrag;
    }

    void Update()
    {
        Vector3 mousePos = Camera.main.ScreenToWorldPoint(Input.mousePosition);
        mousePos.z = 0;
        Vector2 aimDir = (mousePos - transform.position).normalized;
        float aimAngle = Mathf.Atan2(aimDir.y, aimDir.x) * Mathf.Rad2Deg - 90f;
        barrel.rotation = Quaternion.Euler(0, 0, aimAngle);

        if (Input.GetMouseButtonDown(0))
        {
            Shoot();
        }
    }

    void Shoot()
    {
        Vector3 mouseWorld = Camera.main.ScreenToWorldPoint(Input.mousePosition);
        mouseWorld.z = 0;
        Vector2 mousePos2D = (Vector2)mouseWorld;

        // NEW: Check if cursor ON BARREL
        bool onBarrel = false;
        if (barrelCollider != null)
        {
            Collider2D hit = Physics2D.OverlapPoint(mousePos2D, barrelLayerMask);
            onBarrel = (hit == barrelCollider);
        }

        Vector2 shootDir;
        if (onBarrel)
        {
            // STRAIGHT THROUGH FIREPOINT DIRECTION!
            shootDir = firePoint.up;
            Debug.Log("Straight barrel shot!");  // Optional: Console feedback
        }
        else
        {
            // NORMAL: Exact to mouse
            shootDir = ((Vector2)mouseWorld - (Vector2)firePoint.position).normalized;
        }

        // Bullet
        GameObject bullet = Instantiate(bulletPrefab, firePoint.position, firePoint.rotation);
        Rigidbody2D bulletRb = bullet.GetComponent<Rigidbody2D>();
        bulletRb.linearVelocity = shootDir * bulletSpeed;

        // PUSH GLIDE (Same for both modes)
        isGliding = true;
        rb.linearDamping = glideDrag;
        rb.AddForce(-shootDir * launchForce, ForceMode2D.Impulse);
    }

    // PUBLIC: Called by Movement on WASD input
    public void StopGliding()
    {
        if (isGliding)
        {
            isGliding = false;
            rb.linearDamping = originalDrag;
        }
    }
}