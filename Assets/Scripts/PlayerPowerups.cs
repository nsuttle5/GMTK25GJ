using UnityEngine;

public class PlayerPowerups : MonoBehaviour
{
    [Header("References")]
    public PlayerController playerController;
    private Rigidbody2D rb;

    [Header("Double Jump")]
    public bool doubleJumpEnabled = false;
    private bool canDoubleJump = false;

    [Header("Dash")]
    public bool dashEnabled = false;
    public float dashForce = 20f;
    public float dashCooldown = 0.5f;
    private float lastDashTime = -10f;

    [Header("Speed Boost ")]
    public bool speedBoostEnabled = false;
    public float speedBoostMultiplier = 1.5f;
    public float speedBoostDuration = 2f;
    private float speedBoostEndTime = 0f;

    void Awake()
    {
        if (playerController == null)
            playerController = GetComponent<PlayerController>();
        rb = GetComponent<Rigidbody2D>();
    }

    void Update()
    {
        HandleDoubleJump();
        HandleDash();
        HandleSpeedBoost();
    }

    // --- DOUBLE JUMP ---
    void HandleDoubleJump()
    {
        if (!doubleJumpEnabled) return;
        if (playerController.IsGrounded())
        {
            canDoubleJump = true;
        }
        if (Input.GetButtonDown("Jump"))
        {
            if (!playerController.IsGrounded() && canDoubleJump)
            {
                rb.linearVelocity = new Vector2(rb.linearVelocity.x, playerController.jumpForce); // Use same jump force
                canDoubleJump = false;
            }
        }
    }

    // --- DASH ---
    void HandleDash()
    {
        if (!dashEnabled) return;
        if (Input.GetKeyDown(KeyCode.LeftShift) && Time.time > lastDashTime + dashCooldown)
        {
            float dashDir = Input.GetAxisRaw("Horizontal");
            if (dashDir != 0)
            {
                rb.linearVelocity = new Vector2(dashDir * dashForce, rb.linearVelocity.y);
                lastDashTime = Time.time;
            }
        }
    }

    // --- SPEED BOOST (EXAMPLE) ---
    void HandleSpeedBoost()
    {
        if (speedBoostEnabled && Time.time < speedBoostEndTime)
        {
            playerController.moveSpeedMultiplier = speedBoostMultiplier;
        }
        else
        {
            playerController.moveSpeedMultiplier = 1f;
        }
    }

    // --- PUBLIC API ---
    public void EnableDoubleJump(bool enable)
    {
        doubleJumpEnabled = enable;
    }

    public void EnableDash(bool enable)
    {
        dashEnabled = enable;
    }

    public void ActivateSpeedBoost(float duration)
    {
        speedBoostEnabled = true;
        speedBoostEndTime = Time.time + duration;
    }

    public void DisableAllPowerups()
    {
        doubleJumpEnabled = false;
        dashEnabled = false;
        speedBoostEnabled = false;
        playerController.moveSpeedMultiplier = 1f;
    }
}
