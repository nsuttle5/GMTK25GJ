using UnityEngine;

public class DeathByFall : MonoBehaviour
{

    Collider2D enteringObject;

    private void OnTriggerEnter2D(Collider2D other)
    {
        Destroy(other.gameObject);
    }
}
