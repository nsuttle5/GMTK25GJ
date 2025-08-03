using UnityEngine;

public class Heart : MonoBehaviour
{
    public float moveSpeed = 2f;
    public Vector2 moveDirection = Vector2.right;
    private Rigidbody2D rb;
    public PlayerHealth ph;
    public AudioClip heartSound;

    [Header("Audio")]
    public AudioSource pickupAudioSource; // Assign in inspector

    void Awake()
    {
        rb = GetComponent<Rigidbody2D>();
        ph = FindObjectsOfType<PlayerHealth>()[0];
    }

    void Start()
    {
        if (Random.value > 0.5f) moveDirection = Vector2.left;
        // Give the item an initial push
        rb.linearVelocity = new Vector2(moveDirection.x * moveSpeed, rb.linearVelocity.y);
    }

    void OnTriggerEnter2D(Collider2D other)
    {
        if (other.CompareTag("Player"))
        {
            if (pickupAudioSource != null && pickupAudioSource.clip != null)
            {
                pickupAudioSource.transform.SetParent(null); // Detach so it isn't destroyed
                if (heartSound != null)
                {
                    pickupAudioSource.PlayOneShot(heartSound);
                }
                ph.SetHealth(ph.getHealth() + 1);
            }
            gameObject.SetActive(false);
        }
    }
}