using System;
using UnityEngine;

public class CameraMovement : MonoBehaviour
{
    public GameObject playerToFollow;
    public Vector2 followOffset;
    public Vector2 moveOffsetBox;
    private Vector2 threshold;
    public float speed = 1f;

    void Start()
    {
        threshold = calculateThreshold();
    }

    // Update is called once per frame
    void FixedUpdate()
    {
        Vector2 follow = playerToFollow.transform.position;
        float xDifference = Vector2.Distance(Vector2.right * transform.position.x - moveOffsetBox, Vector2.right * follow.x);
        Debug.Log("This is the current xDIfference: " + xDifference);
        Debug.Log("This is the current threshold: " + threshold.x);

        Vector3 newPosition = transform.position;
        if(xDifference >= threshold.x)
        {
            newPosition.x = follow.x;
        }

        transform.position = Vector3.MoveTowards(transform.position, newPosition, speed * Time.deltaTime);
    }

    private Vector3 calculateThreshold()
    {
        Rect aspect = Camera.main.pixelRect;
        Vector2 t = new Vector2(Camera.main.orthographicSize * aspect.width / aspect.height, Camera.main.orthographicSize);
        t.x -= followOffset.x;
        t.y -= followOffset.y;
        return t;
    }

    private void OnDrawGizmos()
    {
        Gizmos.color = Color.yellow;
        Vector2 border = calculateThreshold();
        Gizmos.DrawWireCube(transform.position - new Vector3(moveOffsetBox.x, moveOffsetBox.y, 0), new Vector3(border.x * 2, border.y * 2, 1));
    }


}
