using System;
using System.Collections;
using TMPro;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;


public class PlayerHealth : MonoBehaviour
{
    public int maxHealth = 3;
    public int currentHealth = 3;
    public float invincibleTime = 1.5f;
    public float knockbackForce = 10f;
    public Color flashColor = Color.red;
    public float flashDuration = 0.1f;
    
    public Color color3 = Color.green;
    public Color color2 = Color.yellow;
    public Color color1 = Color.red;

    public Image healthRing;
    public TextMeshProUGUI healthText;

    private bool isInvincible = false;
    private float invincibleTimer = 0f;
    private SpriteRenderer sr;
    private Rigidbody2D rb;
    private Color originalColor;
    private PlayerController playerController;

    public AudioClip damageSound;
    public AudioClip deathSound;
    public AudioSource aSource;
    private float startTime = 0.35f;

    public DeathManager dm;

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
        UpdateHealthUI();
        isInvincible = true;
        invincibleTimer = invincibleTime;
        sr.color = flashColor;
        SetHurt(true); // Trigger hurt animation
        // Knockback
        if (rb != null && currentHealth > 0)
        {
            rb.linearVelocity = Vector2.zero;
            rb.AddForce(new Vector2(hitDirection.x, 1f).normalized * knockbackForce, ForceMode2D.Impulse);
        }
        // Optionally: trigger death if health == 0
        if (aSource != null)
        {
            if (currentHealth > 0)
            {
                aSource.time = startTime;
                aSource.Play();
            }
            else
            {
                aSource.PlayOneShot(deathSound);
            }
        }
        if (currentHealth <= 0)
        {
            dm.KillPlayer();
        }
    }

    public void SetHealth(int health)
    {
        if (health > maxHealth)
        {
            currentHealth = maxHealth;
        }
        else
        {
            currentHealth = health;
        }
        UpdateHealthUI();
    }

    public int getHealth()
    {
        return currentHealth;
    }

    private void UpdateHealthUI()
    {
        healthText.text = currentHealth.ToString();
        healthRing.fillAmount = (float) currentHealth / maxHealth;
        if (currentHealth == 3)
        {
            healthRing.color = color3;
            healthRing.fillAmount = 1f;
        } else if (currentHealth == 2)
        {
            healthRing.color = color2;
            healthRing.fillAmount = 0.63f;
        } else if (currentHealth == 1)
        {
            healthRing.color = color1;
            healthRing.fillAmount = 0.365f;
        }
        else
        {
            healthRing.color = Color.cyan;
        }
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
        Color og = healthRing.color;
        for (float t = 0; t < invincibleTime; t += flashDuration * 2)
        {
            sr.color = flashColor;
            healthRing.color = Color.white;
            yield return new WaitForSeconds(flashDuration);
            sr.color = originalColor;
            healthRing.color = og;
            yield return new WaitForSeconds(flashDuration);
        }
        sr.color = originalColor;
    }
}