using UnityEngine;

public class PlayerShooting : MonoBehaviour
{
    [Header("Shooting Setup")]
    public GameObject bulletPrefab;
    public Transform firePoint;
    public Transform barrel;
    public float bulletSpeed = 10f;
    public float recoilForce = 2f;  // NEW: Tweak kickback strength

    private Rigidbody2D rb;

    void Start()
    {
        rb = GetComponent<Rigidbody2D>();
    }

    void Update()
    {
        // Barrel + firepoint rotate to follow mouse (from player center - visual aim)
        Vector3 mousePos = Camera.main.ScreenToWorldPoint(Input.mousePosition);
        mousePos.z = 0;
        Vector2 aimDir = (mousePos - transform.position).normalized;
        float aimAngle = Mathf.Atan2(aimDir.y, aimDir.x) * Mathf.Rad2Deg - 90f;
        barrel.rotation = Quaternion.Euler(0, 0, aimAngle);

        // Shoot on left click
        if (Input.GetMouseButtonDown(0))
        {
            Shoot();
        }
    }

    void Shoot()
    {
        // Get EXACT mouse world position (fresh for precision)
        Vector3 mouseWorld = Camera.main.ScreenToWorldPoint(Input.mousePosition);
        mouseWorld.z = 0;

        // EXACT direction: from firepoint → mouse click point
        Vector2 shootDir = ((Vector2)mouseWorld - (Vector2)firePoint.position).normalized;

        // Spawn bullet at firepoint
        GameObject bullet = Instantiate(bulletPrefab, firePoint.position, firePoint.rotation);
        Rigidbody2D bulletRb = bullet.GetComponent<Rigidbody2D>();
        
        // Shoot EXACTLY at aimed point!
        bulletRb.linearVelocity = shootDir * bulletSpeed;

        // Recoil in EXACT opposite direction
        rb.AddForce(-shootDir * recoilForce);
    }
}