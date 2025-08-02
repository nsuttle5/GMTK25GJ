using UnityEngine;
using System.Collections;

public class PlayerHealth : MonoBehaviour
{
    public int maxHealth = 2;
    public int currentHealth = 1;
    public float invincibleTime = 1.5f;
    public float knockbackForce = 10f;
    public Color flashColor = Color.red;
    public float flashDuration = 0.1f;

    private bool isInvincible = false;
    private float invincibleTimer = 0f;
    private SpriteRenderer sr;
    private Rigidbody2D rb;
    private Color originalColor;
    private PlayerController playerController;

    void Awake()
    {
        sr = GetComponent<SpriteRenderer>();
        rb = GetComponent<Rigidbody2D>();
        originalColor = sr.color;
        playerController = GetComponent<PlayerController>();
    }

    void Update()
    {
        if (isInvincible)
        {
            invincibleTimer -= Time.deltaTime;
            if (invincibleTimer <= 0f)
            {
                isInvincible = false;
                sr.color = originalColor;
                SetHurt(false); // Reset hurt animation
            }
        }
    }

    public void TakeDamage(Vector2 hitDirection)
    {
        if (isInvincible) return;
        currentHealth = Mathf.Max(0, currentHealth - 1);
        isInvincible = true;
        invincibleTimer = invincibleTime;
        sr.color = flashColor;
        SetHurt(true); // Trigger hurt animation
        // Knockback
        if (rb != null)
        {
            rb.linearVelocity = Vector2.zero;
            rb.AddForce(new Vector2(hitDirection.x, 1f).normalized * knockbackForce, ForceMode2D.Impulse);
        }
        // Optionally: trigger death if health == 0
        if (currentHealth <= 0)
        {
            // TODO: Handle player death (respawn, game over, etc.)
        }
    Optionally: StartCoroutine(FlashSprite());
    }

    // Call this to set the hurt animation state in PlayerController
    private void SetHurt(bool value)
    {
        if (playerController != null)
        {
            playerController.SetHurt(value);
        }
    }

    // Flash sprite for invincibility
    IEnumerator FlashSprite()
    {
        for (float t = 0; t < invincibleTime; t += flashDuration * 2)
        {
            sr.color = flashColor;
            yield return new WaitForSeconds(flashDuration);
            sr.color = originalColor;
            yield return new WaitForSeconds(flashDuration);
        }
        sr.color = originalColor;
    }
}