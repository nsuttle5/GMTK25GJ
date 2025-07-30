using UnityEngine;

public class BasicEnemy : MonoBehaviour
{
    [Header("Movement Settings")]
    [SerializeField] private float moveSpeed = 2f;
    [SerializeField] private float patrolDistance = 5f; // How far to walk in each direction
    [SerializeField] private bool useWaypoints = false; // Use specific waypoints instead of patrol distance

    [Header("Waypoints (Optional)")]
    [SerializeField] private Transform pointA;
    [SerializeField] private Transform pointB;

    [Header("Ground/Wall Detection")]
    [SerializeField] private Transform groundCheck;
    [SerializeField] private Transform wallCheck;
    [SerializeField] private float checkDistance = 0.5f;
    [SerializeField] private LayerMask groundLayerMask = 1; // Ground layer
    [SerializeField] private LayerMask wallLayerMask = 1; // Wall layer
    [SerializeField] private bool turnAtLedges = true; // Turn around when reaching edge of platform
    [SerializeField] private bool turnAtWalls = true; // Turn around when hitting walls
    [SerializeField] private bool onlyCheckLedgesWhenGrounded = true; // Only check for ledges when enemy is on ground

    [Header("Visual")]
    [SerializeField] private bool flipSpriteOnTurn = true;

    // Private variables
    private Rigidbody2D rb;
    private SpriteRenderer spriteRenderer;
    private bool movingRight = true;
    private Vector3 startPosition;
    private float leftBound;
    private float rightBound;
    private bool facingRight = true;
    private float lastTurnTime = 0f;
    private float turnCooldown = 0.05f; // Much shorter cooldown, just to prevent frame-perfect double turns
    private bool isGrounded = false; // Track if enemy is currently on ground

    void Start()
    {
        rb = GetComponent<Rigidbody2D>();
        spriteRenderer = GetComponent<SpriteRenderer>();
        startPosition = transform.position;

        // Set up patrol bounds
        if (useWaypoints && pointA != null && pointB != null)
        {
            leftBound = Mathf.Min(pointA.position.x, pointB.position.x);
            rightBound = Mathf.Max(pointA.position.x, pointB.position.x);

            // Determine initial direction based on position relative to waypoints
            float distanceToLeft = Mathf.Abs(transform.position.x - leftBound);
            float distanceToRight = Mathf.Abs(transform.position.x - rightBound);

            // Start moving towards the farther waypoint
            movingRight = distanceToLeft > distanceToRight;
        }
        else
        {
            leftBound = startPosition.x - patrolDistance;
            rightBound = startPosition.x + patrolDistance;
            // Default: start moving right
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

        // Create wall check if it doesn't exist
        if (wallCheck == null)
        {
            GameObject wallCheckObj = new GameObject("WallCheck");
            wallCheckObj.transform.SetParent(transform);
            wallCheckObj.transform.localPosition = new Vector3(0.5f, 0, 0);
            wallCheck = wallCheckObj.transform;
        }

        // Make sure the enemy starts facing the right direction
        UpdateFacing();
    }

    void FixedUpdate()
    {
        CheckGrounded(); // Check if we're on ground first
        MoveEnemy();
        CheckForTurnaround();
    }

    void MoveEnemy()
    {
        // Move the enemy
        float moveDirection = movingRight ? 1f : -1f;
        Vector2 velocity = rb.linearVelocity;
        velocity.x = moveDirection * moveSpeed;
        rb.linearVelocity = velocity;
    }

    void CheckGrounded()
    {
        // Check if enemy is currently standing on ground
        isGrounded = Physics2D.OverlapCircle(groundCheck.position, checkDistance * 0.3f, groundLayerMask);
    }

    void CheckForTurnaround()
    {
        // Prevent rapid direction changes (very short cooldown)
        if (Time.time - lastTurnTime < turnCooldown)
        {
            return;
        }

        bool shouldTurn = false;
        float positionTolerance = 0.05f; // Smaller tolerance

        // Check patrol bounds with tolerance
        if (movingRight && transform.position.x >= (rightBound - positionTolerance))
        {
            shouldTurn = true;
        }
        else if (!movingRight && transform.position.x <= (leftBound + positionTolerance))
        {
            shouldTurn = true;
        }

        // Check for walls
        if (turnAtWalls && IsWallAhead())
        {
            shouldTurn = true;
        }

        // Check for ledges (no ground ahead) - only when grounded
        if (turnAtLedges && (!onlyCheckLedgesWhenGrounded || isGrounded) && !IsGroundAhead())
        {
            shouldTurn = true;
        }

        if (shouldTurn)
        {
            TurnAround();
        }
    }

    bool IsWallAhead()
    {
        Vector2 wallCheckPos = wallCheck.position;
        if (!movingRight)
        {
            wallCheckPos.x = transform.position.x - Mathf.Abs(wallCheck.localPosition.x);
        }

        return Physics2D.OverlapCircle(wallCheckPos, checkDistance * 0.5f, wallLayerMask);
    }

    bool IsGroundAhead()
    {
        Vector2 groundCheckPos = groundCheck.position;
        float checkOffset = checkDistance;

        if (movingRight)
        {
            groundCheckPos.x += checkOffset;
        }
        else
        {
            groundCheckPos.x -= checkOffset;
        }

        return Physics2D.OverlapCircle(groundCheckPos, checkDistance * 0.3f, groundLayerMask);
    }

    void TurnAround()
    {
        movingRight = !movingRight;
        lastTurnTime = Time.time; // Record when we turned
        UpdateFacing();
    }

    void UpdateFacing()
    {
        if (flipSpriteOnTurn && spriteRenderer != null)
        {
            facingRight = movingRight;
            spriteRenderer.flipX = !facingRight;
        }

        // Update wall check position
        if (wallCheck != null)
        {
            Vector3 wallPos = wallCheck.localPosition;
            wallPos.x = Mathf.Abs(wallPos.x) * (movingRight ? 1 : -1);
            wallCheck.localPosition = wallPos;
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

    // Called when player or other objects collide with enemy
    void OnTriggerEnter2D(Collider2D other)
    {
        if (other.CompareTag("Player"))
        {
            // You can add damage logic here
            Debug.Log("Player touched enemy!");


        }
    }

    void OnDrawGizmosSelected()
    {
        // Draw patrol bounds
        Gizmos.color = Color.yellow;
        Vector3 leftPoint = new Vector3(leftBound, transform.position.y, transform.position.z);
        Vector3 rightPoint = new Vector3(rightBound, transform.position.y, transform.position.z);
        Gizmos.DrawLine(leftPoint, rightPoint);
        Gizmos.DrawWireSphere(leftPoint, 0.3f);
        Gizmos.DrawWireSphere(rightPoint, 0.3f);

        // Draw ground check
        if (groundCheck != null)
        {
            Gizmos.color = Color.green;
            Gizmos.DrawWireSphere(groundCheck.position, checkDistance * 0.3f);

            // Draw ground check ahead
            Vector2 groundCheckPos = groundCheck.position;
            float checkOffset = checkDistance;
            if (Application.isPlaying)
            {
                if (movingRight)
                    groundCheckPos.x += checkOffset;
                else
                    groundCheckPos.x -= checkOffset;
            }
            Gizmos.color = Color.blue;
            Gizmos.DrawWireSphere(groundCheckPos, checkDistance * 0.3f);
        }

        // Draw wall check
        if (wallCheck != null)
        {
            Gizmos.color = Color.red;
            Gizmos.DrawWireSphere(wallCheck.position, checkDistance * 0.5f);
        }

        // Draw waypoints if using them
        if (useWaypoints)
        {
            if (pointA != null)
            {
                Gizmos.color = Color.cyan;
                Gizmos.DrawWireSphere(pointA.position, 0.5f);
            }
            if (pointB != null)
            {
                Gizmos.color = Color.magenta;
                Gizmos.DrawWireSphere(pointB.position, 0.5f);
            }
        }
    }
}
