using UnityEngine;

public class PlayerShooting : MonoBehaviour
{
    [Header("Shooting Setup")]
    public GameObject bulletPrefab;
    public Transform firePoint;
    public Transform barrel;
    public float bulletSpeed = 10f;

    [Header("Auto-Fire on Barrel Hover")]
    public float fireRate = 15f;  // Shots/sec (diep.io rapid!)

    [Header("Recoil Push (Diep.io Slippery)")]
    public float recoilImpulse = 25f;  // BIG push! (>> moveSpeed=5f)

    public float nextFireTime = 0f;
    private PlayerMoveAndCamera moveScript;

    void Start()
    {
        moveScript = GetComponent<PlayerMoveAndCamera>();
    }

    void Update()
    {
        if (PauseMenu.IsPaused) return;

        // Barrel aims at mouse (unchanged)
        Vector3 mousePos = Camera.main.ScreenToWorldPoint(Input.mousePosition);
        mousePos.z = 0;
        Vector2 aimDir = (mousePos - transform.position).normalized;
        float aimAngle = Mathf.Atan2(aimDir.y, aimDir.x) * Mathf.Rad2Deg - 90f;
        barrel.rotation = Quaternion.Euler(0, 0, aimAngle);

        // Barrel hover = AUTO-FIRE FORWARD! (unchanged)
        if (Input.GetMouseButtonDown(0) & Time.time >= nextFireTime || Input .GetMouseButton(0) & Time.time >= nextFireTime)
        {
            Shoot();
            nextFireTime = Time.time + (0.1f / fireRate);
        }
    }

    private bool IsMouseOverBarrel()
    {
        Vector2 mouseWorld = Camera.main.ScreenToWorldPoint(Input.mousePosition);
        Collider2D hit = Physics2D.OverlapPoint(mouseWorld);
        return hit != null && hit.transform == barrel;
    }

    void Shoot()
    {
        // Bullet shoots FIREPOINT FORWARD! (unchanged)
        GameObject bullet = Instantiate(bulletPrefab, firePoint.position, firePoint.rotation);
        Rigidbody2D bulletRb = bullet.GetComponent<Rigidbody2D>();
        bulletRb.linearVelocity = firePoint.up * bulletSpeed;

        // SIGNIFICANT BACKWARD PUSH! (calls movement for slippery blend)
        moveScript.AddRecoil(-firePoint.up * recoilImpulse);
    }
}