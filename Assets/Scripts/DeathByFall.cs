using UnityEngine;

public class DeathByFall : MonoBehaviour
{

    Collider2D enteringObject;
    public DeathManager deathManager;
    public AudioSource aSource;

    private void OnTriggerEnter2D(Collider2D other)
    {
        aSource.Play();
        deathManager.KillPlayer();
    }
}
