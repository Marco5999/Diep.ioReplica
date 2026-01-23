using UnityEngine;

public class PlayerHealth : MonoBehaviour
{
    [Header("Health")]
    public float maxHP = 100f;
    public float currentHP;

    [Header("HP Bar Parts")]
    public Transform fillMiddle;
    public Transform rightCap;

    [Header("Bar Settings")]
    public float fullWidth = 1.6f; // total width of the middle + caps
    public float smoothSpeed = 12f;

    [Header("Damage Settings")]
    public float contactDamage = 10f;
    public float damageCooldown = 0.5f;

    private float currentWidth;
    private float nextDamageTime;

    void Start()
    {
        currentHP = maxHP;
        currentWidth = fullWidth;
    }

    void Update()
    {
        // Smooth HP bar
        float hpPercent = currentHP / maxHP;
        float targetWidth = fullWidth * hpPercent;

        currentWidth = Mathf.Lerp(currentWidth, targetWidth, Time.deltaTime * smoothSpeed);

        // Scale middle from left
        Vector3 midScale = fillMiddle.localScale;
        midScale.x = currentWidth;
        fillMiddle.localScale = midScale;

        // Move right cap
        Vector3 rightPos = rightCap.localPosition;
        rightPos.x = fillMiddle.localPosition.x + currentWidth;
        rightCap.localPosition = rightPos;
    }

    void OnCollisionStay2D(Collision2D collision)
    {
        if (collision.gameObject.CompareTag("Hit") && Time.time >= nextDamageTime)
        {
            TakeDamage(contactDamage);
            nextDamageTime = Time.time + damageCooldown;
        }
    }

    public void TakeDamage(float dmg)
    {
        currentHP = Mathf.Clamp(currentHP - dmg, 0f, maxHP);
    }
}
