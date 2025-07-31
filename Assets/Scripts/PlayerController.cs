using UnityEngine;

public class PlayerController : MonoBehaviour
{
    [Header("Movement Settings")]
    [SerializeField] private float walkSpeed = 6f;
    [SerializeField] private float acceleration = 12f;
    [SerializeField] private float deceleration = 16f;
    [SerializeField] private float airAcceleration = 6f;
    [SerializeField] private float airDeceleration = 8f;

    [Header("Jump Settings")]
    [SerializeField] public float jumpForce = 16f;
    [SerializeField] private float jumpHoldForce = 0.5f;
    [SerializeField] private float maxJumpTime = 0.35f;
    [SerializeField] private float fallMultiplier = 2.5f;
    [SerializeField] private float lowJumpMultiplier = 2f;

    [Header("Ground Detection")]
    [SerializeField] private Transform groundCheck;
    [SerializeField] private float groundCheckRadius = 0.2f;
    [SerializeField] private LayerMask groundLayerMask;

    [Header("Coyote Time & Jump Buffer")]
    [SerializeField] private float coyoteTime = 0.1f;
    [SerializeField] private float jumpBufferTime = 0.1f;

    // Components
    private Rigidbody2D rb;
    private SpriteRenderer spriteRenderer;

    // Movement variables
    private float horizontalInput;
    private bool facingRight = true;

    // Powerup integration
    public float moveSpeedMultiplier { get; set; } = 1f;

    // Jump variables
    private bool isGrounded;
    private bool isJumping;
    private float jumpTimeCounter;
    private float coyoteTimeCounter;
    private float jumpBufferCounter;

    // Physics
    private float currentSpeed;
    private Vector2 velocity;

    // Interaction logic for InteractableObject
    private InteractableObject currentInteractable;

    void Start()
    {
        rb = GetComponent<Rigidbody2D>();
        spriteRenderer = GetComponent<SpriteRenderer>();
        moveSpeedMultiplier = 1f;

        // Create ground check if it doesn't exist
        if (groundCheck == null)
        {
            GameObject groundCheckObj = new GameObject("GroundCheck");
            groundCheckObj.transform.SetParent(transform);
            groundCheckObj.transform.localPosition = new Vector3(0, -0.5f, 0);
            groundCheck = groundCheckObj.transform;
        }
    }

    void Update()
    {
        HandleInput();
        CheckGrounded();
        HandleCoyoteTime();
        HandleJumpBuffer();
    }

    void FixedUpdate()
    {
        // Get current velocity from rigidbody first
        velocity = rb.linearVelocity;

        HandleMovement();
        HandleJump(); // Handle jump after movement to preserve jump velocity
        ApplyJumpPhysics();

        rb.linearVelocity = velocity;
    }

    void HandleInput()
    {
        horizontalInput = Input.GetAxisRaw("Horizontal");

        // Jump input
        if (Input.GetKeyDown(KeyCode.Space) || Input.GetKeyDown(KeyCode.Z))
        {
            jumpBufferCounter = jumpBufferTime;
        }

        // Handle jump release for variable jump height
        if (Input.GetKeyUp(KeyCode.Space) || Input.GetKeyUp(KeyCode.Z))
        {
            if (velocity.y > 0)
            {
                velocity.y = velocity.y * 0.5f;
            }
            isJumping = false;
        }
    }

    void CheckGrounded()
    {
        bool wasGrounded = isGrounded;
        isGrounded = Physics2D.OverlapCircle(groundCheck.position, groundCheckRadius, groundLayerMask);

        // Reset jump when landing, but don't affect horizontal movement
        if (!wasGrounded && isGrounded)
        {
            isJumping = false;
            jumpTimeCounter = 0f;
        }
    }

    void HandleCoyoteTime()
    {
        if (isGrounded)
        {
            coyoteTimeCounter = coyoteTime;
        }
        else
        {
            coyoteTimeCounter -= Time.deltaTime;
        }
    }

    void HandleJumpBuffer()
    {
        if (jumpBufferCounter > 0)
        {
            jumpBufferCounter -= Time.deltaTime;
        }
    }

    void HandleJump()
    {
        // Start jump
        if (jumpBufferCounter > 0 && (isGrounded || coyoteTimeCounter > 0) && !isJumping)
        {
            isJumping = true;
            jumpTimeCounter = 0f;
            velocity.y = jumpForce;
            jumpBufferCounter = 0f;
            coyoteTimeCounter = 0f;
        }

        // Continue jump (variable height)
        if ((Input.GetKey(KeyCode.Space) || Input.GetKey(KeyCode.Z)) && isJumping)
        {
            if (jumpTimeCounter < maxJumpTime)
            {
                velocity.y += jumpHoldForce * Time.deltaTime * 60f; // 60f for frame rate independence
                jumpTimeCounter += Time.deltaTime;
            }
            else
            {
                isJumping = false;
            }
        }
    }

    void HandleMovement()
    {
        // Determine target speed
        float targetSpeed = 0f;
        if (horizontalInput != 0)
        {
            targetSpeed = walkSpeed * moveSpeedMultiplier * horizontalInput;
        }

        // Choose acceleration/deceleration values based on ground state
        float accel = isGrounded ? acceleration : airAcceleration;
        float decel = isGrounded ? deceleration : airDeceleration;

        // Apply acceleration or deceleration
        if (Mathf.Abs(targetSpeed) > 0.01f)
        {
            // Accelerating toward target speed
            if (Mathf.Sign(targetSpeed) == Mathf.Sign(velocity.x))
            {
                // Same direction - accelerate
                velocity.x = Mathf.MoveTowards(velocity.x, targetSpeed, accel * Time.fixedDeltaTime);
            }
            else
            {
                // Different direction - decelerate faster (turnaround)
                velocity.x = Mathf.MoveTowards(velocity.x, targetSpeed, decel * 1.5f * Time.fixedDeltaTime);
            }
        }
        else
        {
            // Decelerating to stop
            velocity.x = Mathf.MoveTowards(velocity.x, 0f, decel * Time.fixedDeltaTime);
        }

        // Handle sprite flipping
        if (horizontalInput > 0 && !facingRight)
        {
            Flip();
        }
        else if (horizontalInput < 0 && facingRight)
        {
            Flip();
        }
    }

    // --- POWERUP SUPPORT ---
    public bool IsGrounded()
    {
        return isGrounded;
    }

    void ApplyJumpPhysics()
    {
        // Apply different gravity based on jump state (Mario-style variable jump)
        if (velocity.y < 0)
        {
            // Falling - apply stronger gravity
            velocity.y += Physics2D.gravity.y * (fallMultiplier - 1) * Time.fixedDeltaTime;
        }
        else if (velocity.y > 0 && (!Input.GetKey(KeyCode.Space) && !Input.GetKey(KeyCode.Z)))
        {
            // Rising but not holding jump - apply moderate gravity
            velocity.y += Physics2D.gravity.y * (lowJumpMultiplier - 1) * Time.fixedDeltaTime;
        }
    }

    void Flip()
    {
        facingRight = !facingRight;
        spriteRenderer.flipX = !facingRight;
    }

    void OnDrawGizmosSelected()
    {
        if (groundCheck != null)
        {
            Gizmos.color = isGrounded ? Color.green : Color.red;
            Gizmos.DrawWireSphere(groundCheck.position, groundCheckRadius);
        }
    }

    void OnTriggerEnter2D(Collider2D other)
    {
        if (other.TryGetComponent(out InteractableObject interactable))
        {
            currentInteractable = interactable;
            // Optionally show an interaction prompt here
        }
    }

    void OnTriggerExit2D(Collider2D other)
    {
        if (other.TryGetComponent(out InteractableObject interactable) && interactable == currentInteractable)
        {
            currentInteractable = null;
            // Optionally hide the interaction prompt here
        }
    }

    void LateUpdate()
    {
        // Interact with nearby object
        if (currentInteractable != null && Input.GetKeyDown(KeyCode.E))
        {
            currentInteractable.Interact();
        }
    }
}
