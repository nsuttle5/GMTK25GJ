using UnityEngine;

public class Checkpoint : MonoBehaviour
{
    private SpriteRenderer sr;
    void Awake()
    {
        sr = GetComponent<SpriteRenderer>();
    }
    private void OnTriggerEnter2D(Collider2D other)
    {
        Debug.Log($"Touched by: {other.gameObject.name}");

        // Optional: only react to the player
        if (other.CompareTag("Player"))
        {
            CheckpointManager.Instance.lastPos = transform.position; 
            Debug.Log("Checkpoint reached!");
            sr.color = Color.green;
        }
    }
}