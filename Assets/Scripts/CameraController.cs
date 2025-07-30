using UnityEngine;

public class CameraController : MonoBehaviour
{
    [Header("Target")]
    [SerializeField] private Transform target; // The player to follow

    [Header("Smoothing")]
    [SerializeField] private bool useInterpolation = true; // Reduces jitter

    [Header("Horizontal Following")]
    [SerializeField] private float horizontalDeadzone = 2f;
    [SerializeField] private float horizontalSmoothTime = 0.15f; // Reduced for smoother following
    [SerializeField] private float lookaheadDistance = 3f;
    [SerializeField] private float lookaheadSmoothTime = 1f; // Reduced for quicker response

    [Header("Vertical Following")]
    [SerializeField] private float verticalDeadzone = 3f;
    [SerializeField] private float verticalSmoothTime = 0.4f; // Reduced for smoother following
    [SerializeField] private float maxVerticalOffset = 5f; [Header("Bounds (Optional)")]
    [SerializeField] private bool useBounds = false;
    [SerializeField] private float leftBound = -10f;
    [SerializeField] private float rightBound = 10f;
    [SerializeField] private float bottomBound = -5f;
    [SerializeField] private float topBound = 10f;

    // Private variables
    private Vector3 velocity;
    private Vector3 horizontalVelocity;
    private Vector3 verticalVelocity;
    private float targetX;
    private float targetY;
    private float currentLookahead;
    private float lookaheadVelocity;
    private PlayerController playerController;

    void Start()
    {
        if (target == null)
        {
            // Try to find player automatically
            PlayerController playerController = FindObjectOfType<PlayerController>();
            if (playerController != null)
            {
                target = playerController.transform;
            }
        }

        if (target != null)
        {
            playerController = target.GetComponent<PlayerController>();

            // Initialize camera position to target immediately
            Vector3 startPos = target.position;
            startPos.z = transform.position.z; // Keep camera's Z position
            transform.position = startPos;

            targetX = startPos.x;
            targetY = startPos.y;
        }
        else
        {
            Debug.LogWarning("CameraController: No target found! Please assign a target or ensure a PlayerController exists in the scene.");
        }
    }

    void LateUpdate()
    {
        // Safety check - if target is missing, try to find it again
        if (target == null)
        {
            PlayerController playerController = FindObjectOfType<PlayerController>();
            if (playerController != null)
            {
                target = playerController.transform;
                this.playerController = playerController;

                // Reset camera targets to current position to avoid jumping
                targetX = transform.position.x;
                targetY = transform.position.y;
            }
            else
            {
                return; // No target found, don't update camera
            }
        }

        HandleHorizontalFollow();
        HandleVerticalFollow();
        HandleLookahead();

        // Apply bounds if enabled
        if (useBounds)
        {
            targetX = Mathf.Clamp(targetX, leftBound, rightBound);
            targetY = Mathf.Clamp(targetY, bottomBound, topBound);
        }

        // Use separate smooth damping for X and Y with different velocities for smoother movement
        Vector3 currentPos = transform.position;
        float newX = Mathf.SmoothDamp(currentPos.x, targetX + currentLookahead, ref horizontalVelocity.x, horizontalSmoothTime);
        float newY = Mathf.SmoothDamp(currentPos.y, targetY, ref verticalVelocity.y, verticalSmoothTime);

        transform.position = new Vector3(newX, newY, currentPos.z);
    }

    void HandleHorizontalFollow()
    {
        Vector3 playerPos = target.position;

        // Use interpolated position if enabled to reduce jitter
        if (useInterpolation && target.GetComponent<Rigidbody2D>() != null)
        {
            Rigidbody2D rb = target.GetComponent<Rigidbody2D>();
            playerPos = rb.transform.position + (Vector3)rb.linearVelocity * Time.fixedDeltaTime;
        }

        float playerX = playerPos.x;
        float distanceFromCenter = playerX - targetX;

        // Only move camera if player is outside the deadzone
        if (Mathf.Abs(distanceFromCenter) > horizontalDeadzone)
        {
            float targetDirection = Mathf.Sign(distanceFromCenter);
            float newTargetX = playerX - (targetDirection * horizontalDeadzone);

            // Use a more responsive movement for horizontal following
            targetX = Mathf.Lerp(targetX, newTargetX, Time.deltaTime * (1f / horizontalSmoothTime) * 2f);
        }
    }

    void HandleVerticalFollow()
    {
        float playerY = target.position.y;
        float distanceFromCenter = playerY - targetY;

        // Mario-style vertical camera: less responsive, larger deadzone
        if (Mathf.Abs(distanceFromCenter) > verticalDeadzone)
        {
            float targetDirection = Mathf.Sign(distanceFromCenter);
            float maxOffset = Mathf.Min(Mathf.Abs(distanceFromCenter), maxVerticalOffset);
            float newTargetY = playerY - (targetDirection * (verticalDeadzone + (maxOffset * 0.5f)));

            // Very slow vertical movement - classic Mario feel
            targetY = Mathf.MoveTowards(targetY, newTargetY, Time.deltaTime / verticalSmoothTime);
        }

        // Quick snap down when player lands (Mario-style)
        if (playerController != null && WasInAirNowGrounded())
        {
            float groundLevel = target.position.y;
            if (targetY > groundLevel + 2f) // If camera is significantly above ground
            {
                targetY = Mathf.MoveTowards(targetY, groundLevel, Time.deltaTime * 8f); // Quick snap down
            }
        }
    }

    void HandleLookahead()
    {
        if (playerController == null) return;

        // Determine lookahead direction based on player facing direction
        float targetLookahead = 0f;

        // Get horizontal input to determine movement direction
        float horizontalInput = Input.GetAxisRaw("Horizontal");

        if (Mathf.Abs(horizontalInput) > 0.1f)
        {
            targetLookahead = Mathf.Sign(horizontalInput) * lookaheadDistance;
        }

        // Smooth the lookahead movement
        currentLookahead = Mathf.SmoothDamp(currentLookahead, targetLookahead, ref lookaheadVelocity, lookaheadSmoothTime);
    }

    bool WasInAirNowGrounded()
    {
        // Improved ground check - only snap if player was recently in air and is now grounded
        if (playerController == null) return false;

        // Check if camera is significantly above player and player has low vertical velocity
        bool cameraAbovePlayer = transform.position.y > target.position.y + 2f;
        bool playerNotRising = target.GetComponent<Rigidbody2D>().linearVelocity.y <= 0.1f;

        return cameraAbovePlayer && playerNotRising;
    }

    // Public method to set target manually
    public void SetTarget(Transform newTarget)
    {
        target = newTarget;
        if (target != null)
        {
            playerController = target.GetComponent<PlayerController>();

            // Smoothly transition to new target instead of jumping
            targetX = target.position.x;
            targetY = target.position.y;
        }
    }

    // Public method to get current target
    public Transform GetTarget()
    {
        return target;
    }

    // Helper method to set camera bounds at runtime
    public void SetBounds(float left, float right, float bottom, float top)
    {
        useBounds = true;
        leftBound = left;
        rightBound = right;
        bottomBound = bottom;
        topBound = top;
    }

    // Helper method to disable bounds
    public void DisableBounds()
    {
        useBounds = false;
    }

    void OnDrawGizmosSelected()
    {
        if (target == null) return;

        // Draw horizontal deadzone
        Gizmos.color = Color.yellow;
        Vector3 center = new Vector3(targetX, target.position.y, 0);
        Gizmos.DrawWireCube(center, new Vector3(horizontalDeadzone * 2, 0.5f, 0));

        // Draw vertical deadzone
        Gizmos.color = Color.cyan;
        center = new Vector3(target.position.x, targetY, 0);
        Gizmos.DrawWireCube(center, new Vector3(0.5f, verticalDeadzone * 2, 0));

        // Draw camera bounds if enabled
        if (useBounds)
        {
            Gizmos.color = Color.red;
            Vector3 boundsCenter = new Vector3((leftBound + rightBound) / 2, (bottomBound + topBound) / 2, 0);
            Vector3 boundsSize = new Vector3(rightBound - leftBound, topBound - bottomBound, 0);
            Gizmos.DrawWireCube(boundsCenter, boundsSize);
        }

        // Draw lookahead
        if (Application.isPlaying)
        {
            Gizmos.color = Color.green;
            Vector3 lookaheadPos = new Vector3(targetX + currentLookahead, targetY, 0);
            Gizmos.DrawWireSphere(lookaheadPos, 0.5f);
        }
    }
}
