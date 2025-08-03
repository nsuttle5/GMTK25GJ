using UnityEngine;

public class JumpPad : MonoBehaviour
{
    [Header("Jump Pad Settings")]
    public float jumpForce = 18f;
    public string[] launchTags = { "Player", "Enemy" };
    public float minDownwardVelocity = -1f; // Only launch if falling at least this fast

    [Header("Audio")]
    public AudioSource jumpPadAudioSource; // Assign in inspector

    private void OnTriggerEnter2D(Collider2D other)
    {
        foreach (string tag in launchTags)
        {
            if (other.CompareTag(tag))
            {
                Rigidbody2D rb = other.attachedRigidbody;
                if (rb != null)
                {
                    // Only launch if coming from above (falling onto pad)
                    if (rb.linearVelocity.y <= minDownwardVelocity)
                    {
                        rb.linearVelocity = new Vector2(rb.linearVelocity.x, 0f);
                        rb.AddForce(Vector2.up * jumpForce, ForceMode2D.Impulse);
                        if (jumpPadAudioSource != null && jumpPadAudioSource.clip != null)
                        {
                            jumpPadAudioSource.Play();
                        }
                    }
                }
                break;
            }
        }
    }
}
