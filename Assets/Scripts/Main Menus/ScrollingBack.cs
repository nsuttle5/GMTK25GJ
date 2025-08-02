using UnityEngine;
using UnityEngine.UI;

public class BackgroundScroller : MonoBehaviour
{
    public RawImage image;
    public Vector2 scrollSpeed = new Vector2(0.1f, 0f);

    private Vector2 uvOffset = Vector2.zero;

    void Update()
    {
        uvOffset += scrollSpeed * Time.deltaTime;
        image.uvRect = new Rect(uvOffset, image.uvRect.size);
    }
}