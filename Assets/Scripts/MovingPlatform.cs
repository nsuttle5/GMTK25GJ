using UnityEngine;

public class MovingPlatform : MonoBehaviour
{
    [Header("Movement Settings")]
    [SerializeField] private float moveSpeed = 2f;
    [SerializeField] private bool useWaypoints = true; // Use waypoints or distance-based movement
    [SerializeField] private float patrolDistance = 5f; // Distance to move if not using waypoints

    [Header("Waypoints")]
    [SerializeField] private Transform pointA;
    [SerializeField] private Transform pointB;
    [SerializeField] private bool startAtPointA = true; // Which waypoint to start moving towards

    [Header("Movement Options")]
    [SerializeField] private bool pauseAtWaypoints = false;
    [SerializeField] private float pauseDuration = 1f;
    [SerializeField] private AnimationCurve movementCurve = AnimationCurve.Linear(0, 0, 1, 1); // Smooth movement curve

    [Header("Player Carrying")]
    [SerializeField] public bool carryPlayer = true; // Should the platform carry objects that land on it?
    [SerializeField] public string[] carryTags = { "Player", "Enemy" }; // Tags of objects to carry

    // Private variables
    private Vector3 startPosition;
    private Vector3 targetA;
    private Vector3 targetB;
    private Vector3 currentTarget;
    private bool movingTowardsA = true;
    private float journeyLength;
    private float journeyTime = 0f;
    private bool isPaused = false;
    private float pauseTimer = 0f;

    // For carrying objects
    private Transform platformTop;
    private Vector3 lastFramePosition;
    private Vector3 currentFrameDelta;

    void Start()
    {
        startPosition = transform.position;

        // Set up waypoints
        if (useWaypoints && pointA != null && pointB != null)
        {
            targetA = pointA.position;
            targetB = pointB.position;
        }
        else
        {
            // Use patrol distance from starting position
            targetA = startPosition + Vector3.left * patrolDistance;
            targetB = startPosition + Vector3.right * patrolDistance;
        }

        // Calculate journey length
        journeyLength = Vector3.Distance(targetA, targetB);

        // Set initial target based on startAtPointA setting
        if (startAtPointA)
        {
            movingTowardsA = true;
            currentTarget = targetA;
        }
        else
        {
            movingTowardsA = false;
            currentTarget = targetB;
        }

        // Create a child object to detect what's on top of the platform
        CreatePlatformTop();

        // Initialize position tracking
        lastFramePosition = transform.position;
    }

    void CreatePlatformTop()
    {
        GameObject topDetector = new GameObject("PlatformTop");
        topDetector.transform.SetParent(transform);

        // Position it slightly above the platform
        Collider2D platformCollider = GetComponent<Collider2D>();
        float platformHeight = platformCollider != null ? platformCollider.bounds.size.y : 1f;
        topDetector.transform.localPosition = new Vector3(0, platformHeight * 0.5f + 0.1f, 0);

        // Add a trigger collider to detect objects on top
        BoxCollider2D topCollider = topDetector.AddComponent<BoxCollider2D>();
        topCollider.isTrigger = true;
        topCollider.size = new Vector2(platformCollider.bounds.size.x, 0.2f);

        // Add the carry behavior component
        PlatformCarrier carrier = topDetector.AddComponent<PlatformCarrier>();
        carrier.Initialize(this);

        platformTop = topDetector.transform;
    }

    void Update()
    {
        if (isPaused)
        {
            HandlePause();
        }
    }

    void FixedUpdate()
    {
        if (!isPaused)
        {
            Vector3 oldPosition = transform.position;
            MovePlatform();

            // Calculate movement delta after movement
            currentFrameDelta = transform.position - oldPosition;
        }
        else
        {
            currentFrameDelta = Vector3.zero;
        }
    }

    void MovePlatform()
    {
        // Calculate movement progress
        float distanceToTarget = Vector3.Distance(transform.position, currentTarget);
        float totalDistance = Vector3.Distance(movingTowardsA ? targetB : targetA, currentTarget);

        // Move towards target using FixedUpdate timing
        Vector3 startPos = movingTowardsA ? targetB : targetA;
        journeyTime += Time.fixedDeltaTime * moveSpeed / journeyLength;

        // Apply movement curve for smooth acceleration/deceleration
        float curveValue = movementCurve.Evaluate(journeyTime);
        Vector3 newPosition = Vector3.Lerp(startPos, currentTarget, curveValue);

        transform.position = newPosition;

        // Check if we've reached the target
        if (journeyTime >= 1f)
        {
            transform.position = currentTarget;

            if (pauseAtWaypoints)
            {
                StartPause();
            }
            else
            {
                SwitchTarget();
            }
        }
    }

    void HandlePause()
    {
        pauseTimer += Time.deltaTime;
        if (pauseTimer >= pauseDuration)
        {
            isPaused = false;
            pauseTimer = 0f;
            SwitchTarget();
        }
    }

    void SwitchTarget()
    {
        movingTowardsA = !movingTowardsA;
        currentTarget = movingTowardsA ? targetA : targetB;
        journeyTime = 0f;
    }

    void StartPause()
    {
        isPaused = true;
        pauseTimer = 0f;
    }

    // Public methods for external control
    public void SetSpeed(float newSpeed)
    {
        moveSpeed = newSpeed;
    }

    public void SetWaypoints(Vector3 newPointA, Vector3 newPointB)
    {
        targetA = newPointA;
        targetB = newPointB;
        journeyLength = Vector3.Distance(targetA, targetB);
    }

    public void PausePlatform()
    {
        StartPause();
    }

    // Public method for getting movement delta
    public Vector3 GetMovementDelta()
    {
        return currentFrameDelta;
    }

    public bool IsMoving()
    {
        return !isPaused;
    }

    void OnDrawGizmosSelected()
    {
        // Draw waypoints and path
        Gizmos.color = Color.green;

        if (useWaypoints && pointA != null && pointB != null)
        {
            Gizmos.DrawWireSphere(pointA.position, 0.5f);
            Gizmos.DrawWireSphere(pointB.position, 0.5f);
            Gizmos.DrawLine(pointA.position, pointB.position);
        }
        else
        {
            Vector3 leftPoint = transform.position + Vector3.left * patrolDistance;
            Vector3 rightPoint = transform.position + Vector3.right * patrolDistance;
            Gizmos.DrawWireSphere(leftPoint, 0.5f);
            Gizmos.DrawWireSphere(rightPoint, 0.5f);
            Gizmos.DrawLine(leftPoint, rightPoint);
        }

        // Draw current target
        if (Application.isPlaying)
        {
            Gizmos.color = Color.red;
            Gizmos.DrawWireSphere(currentTarget, 0.3f);
        }

        // Draw platform detection area
        if (platformTop != null)
        {
            Gizmos.color = Color.blue;
            Gizmos.DrawWireCube(platformTop.position, new Vector3(2f, 0.2f, 0f));
        }
    }
}

// Helper component for carrying objects on the platform
public class PlatformCarrier : MonoBehaviour
{
    private MovingPlatform platform;

    public void Initialize(MovingPlatform parentPlatform)
    {
        platform = parentPlatform;
    }

    void OnTriggerStay2D(Collider2D other)
    {
        // Only carry objects if the platform is set to carry and the object has a carry tag
        if (platform.carryPlayer && HasCarryTag(other.gameObject))
        {
            // Get the platform's movement delta from this frame
            Vector3 platformDelta = platform.GetMovementDelta();

            // Debug logging
            // Debug.Log($"Carrying {other.name}, Delta: {platformDelta}, Magnitude: {platformDelta.magnitude}");

            // Move the object by the same amount the platform moved
            if (platformDelta.magnitude > 0.001f) // Only move if there's significant movement
            {
                other.transform.position += platformDelta;
            }
        }
        else
        {
            Debug.Log($"Not carrying {other.name}, CarryPlayer: {platform.carryPlayer}, HasTag: {HasCarryTag(other.gameObject)}");
        }
    }

    bool HasCarryTag(GameObject obj)
    {
        foreach (string tag in platform.carryTags)
        {
            if (obj.CompareTag(tag))
            {
                return true;
            }
        }
        return false;
    }
}
