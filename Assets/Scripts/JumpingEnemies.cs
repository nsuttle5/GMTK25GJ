using UnityEngine;

public class JumpingEnemies : MonoBehaviour
{
    [Header("Movement Settings")]
    [SerializeField] private float hopForce = 8f;
    [SerializeField] private float hopInterval = 1.2f; // Time between hops
    [SerializeField] private float patrolDistance = 5f;
    [SerializeField] private bool useWaypoints = false;

    [Header("Waypoints (Optional)")]
    [SerializeField] private Transform pointA;
    [SerializeField] private Transform pointB;

    [Header("Ground Detection")]
    [SerializeField] private Transform groundCheck;
    [SerializeField] private float checkDistance = 0.3f;
    [SerializeField] private LayerMask groundLayerMask = 1;

    [Header("Visual")]
    [SerializeField] private bool flipSpriteOnTurn = true;

    [Header("Audio")]
    public AudioSource deathAudioSource; // Assign in inspector

    private Rigidbody2D rb;
    private SpriteRenderer spriteRenderer;
    private Animator animator;
    private bool movingRight = true;
    private Vector3 startPosition;
    private float leftBound;
    private float rightBound;
    private bool facingRight = true;
    private float nextHopTime = 0f;
    private bool isGrounded = false;

    void Start()
    {
        rb = GetComponent<Rigidbody2D>();
        spriteRenderer = GetComponent<SpriteRenderer>();
        animator = GetComponent<Animator>();
        startPosition = transform.position;

        // Set up patrol bounds
        if (useWaypoints && pointA != null && pointB != null)
        {
            leftBound = Mathf.Min(pointA.position.x, pointB.position.x);
            rightBound = Mathf.Max(pointA.position.x, pointB.position.x);
            float distanceToLeft = Mathf.Abs(transform.position.x - leftBound);
            float distanceToRight = Mathf.Abs(transform.position.x - rightBound);
            movingRight = distanceToLeft > distanceToRight;
        }
        else
        {
            leftBound = startPosition.x - patrolDistance;
            rightBound = startPosition.x + patrolDistance;
            movingRight = true;
        }

        // Create ground check if it doesn't exist
        if (groundCheck == null)
        {
            GameObject groundCheckObj = new GameObject("GroundCheck");
            groundCheckObj.transform.SetParent(transform);
            groundCheckObj.transform.localPosition = new Vector3(0, -0.5f, 0);
            groundCheck = groundCheckObj.transform;
        }

        UpdateFacing();
        nextHopTime = Time.time + Random.Range(0f, hopInterval * 0.5f); // Randomize first hop
    }

    void FixedUpdate()
    {
        CheckGrounded();
        PatrolLogic();
        UpdateAnimator();
    }

    void UpdateAnimator()
    {
        if (animator != null)
        {
            animator.SetBool("isJumping", !isGrounded);
        }
    }

    void PatrolLogic()
    {
        // Only hop if on ground and enough time has passed
        if (isGrounded && Time.time >= nextHopTime)
        {
            Hop();
            nextHopTime = Time.time + hopInterval;
        }

        // Check if we need to turn around at patrol bounds
        float positionTolerance = 0.05f;
        if (movingRight && transform.position.x >= (rightBound - positionTolerance))
        {
            movingRight = false;
            UpdateFacing();
        }
        else if (!movingRight && transform.position.x <= (leftBound + positionTolerance))
        {
            movingRight = true;
            UpdateFacing();
        }
    }

    void Hop()
    {
        // Only hop if on ground
        if (!isGrounded) return;
        float xDir = movingRight ? 1f : -1f;
        Vector2 hopVelocity = new Vector2(xDir * hopForce * 0.6f, hopForce);
        rb.linearVelocity = hopVelocity;
    }

    void CheckGrounded()
    {
        isGrounded = Physics2D.OverlapCircle(groundCheck.position, checkDistance, groundLayerMask);
    }

    void UpdateFacing()
    {
        if (flipSpriteOnTurn && spriteRenderer != null)
        {
            facingRight = movingRight;
            spriteRenderer.flipX = !facingRight;
        }
    }

    // Public method to set patrol bounds manually
    public void SetPatrolBounds(float leftX, float rightX)
    {
        leftBound = leftX;
        rightBound = rightX;
        useWaypoints = false;
    }

    // Public method to set waypoints
    public void SetWaypoints(Transform pointA, Transform pointB)
    {
        this.pointA = pointA;
        this.pointB = pointB;
        useWaypoints = true;
        if (pointA != null && pointB != null)
        {
            leftBound = Mathf.Min(pointA.position.x, pointB.position.x);
            rightBound = Mathf.Max(pointA.position.x, pointB.position.x);
        }
    }

    // Optional: OnDrawGizmosSelected for debugging patrol bounds and ground check
    void OnDrawGizmosSelected()
    {
        Gizmos.color = Color.yellow;
        Vector3 leftPoint = new Vector3(leftBound, transform.position.y, transform.position.z);
        Vector3 rightPoint = new Vector3(rightBound, transform.position.y, transform.position.z);
        Gizmos.DrawLine(leftPoint, rightPoint);
        Gizmos.DrawWireSphere(leftPoint, 0.3f);
        Gizmos.DrawWireSphere(rightPoint, 0.3f);

        if (groundCheck != null)
        {
            Gizmos.color = Color.green;
            Gizmos.DrawWireSphere(groundCheck.position, checkDistance);
        }
    }

    // --- PLAYER INTERACTION: Stomp to destroy ---
    void OnTriggerEnter2D(Collider2D other)
    {
        if (other.CompareTag("Player"))
        {
            var player = other.GetComponent<PlayerController>();
            var health = other.GetComponent<PlayerHealth>();
            Rigidbody2D playerRb = other.attachedRigidbody;

            // Stomp detection (player comes from above with downward velocity)
            float playerBottom = other.bounds.min.y;
            float enemyTop = GetComponent<Collider2D>().bounds.max.y;
            float yDiff = playerBottom - enemyTop;
            float stompThreshold = 0.15f;
            float playerDownwardVelocity = playerRb != null ? playerRb.linearVelocity.y : 0f;
            if (yDiff > -stompThreshold && playerDownwardVelocity < -0.1f)
            {
                // Player stomped enemy
                if (deathAudioSource != null && deathAudioSource.clip != null)
                {
                    deathAudioSource.Play();
                    // Hide enemy visuals/collider immediately, destroy after sound
                    GetComponent<Collider2D>().enabled = false;
                    if (spriteRenderer != null) spriteRenderer.enabled = false;
                    if (animator != null) animator.enabled = false;
                    Destroy(gameObject, deathAudioSource.clip.length);
                }
                else
                {
                    Destroy(gameObject);
                }
                // Bounce player up
                if (playerRb != null)
                {
                    playerRb.linearVelocity = new Vector2(playerRb.linearVelocity.x, 12f);
                }
            }
            else if (health != null)
            {
                // Player hit from side/below, take damage
                Vector2 hitDir = (other.transform.position - transform.position).normalized;
                health.TakeDamage(hitDir);
            }
        }
    }
}
