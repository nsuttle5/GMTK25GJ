using UnityEngine;

public class Respawnable : MonoBehaviour
{
    private Vector3 startPos;
    private Quaternion startRot;

    private void Awake()
    {
        startPos = transform.position;
        startRot = transform.rotation;
    }

    public void Respawn()
    {
        transform.position = startPos;
        transform.rotation = startRot;

        // Add extra logic here if needed (e.g., re-enable AI)
        gameObject.SetActive(true);
    }
}