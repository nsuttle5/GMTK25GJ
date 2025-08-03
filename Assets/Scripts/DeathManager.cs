using UnityEngine;

public class DeathManager : MonoBehaviour
{
    public GameObject player;
    public Respawnable[] respawnables;
    public PlayerHealth playerHealth;
    
    private void Start()
    {
        // Automatically find all Respawnables in the scene
        respawnables = FindObjectsOfType<Respawnable>();
        playerHealth = FindObjectOfType<PlayerHealth>();
    }
    public void KillPlayer()
    {
        // Teleport player back to checkpoint
        player.transform.position = CheckpointManager.Instance.lastPos;

        // Reset monsters, blocks, etc.
        foreach (Respawnable r in respawnables)
        {
            r.Respawn();
        }

        if (playerHealth != null)
        {
            playerHealth.SetHealth(3);
        }
        Debug.Log("Player respawned at checkpoint");
    }
}
