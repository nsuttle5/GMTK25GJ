using UnityEngine;
using System.Collections;

public class ExclaimBlock : MonoBehaviour
{
    public Sprite usedBlockSprite;
    public GameObject itemPrefab;
    public float itemPopUpDistance = 1f;
    public float itemPopUpSpeed = 4f;
    private bool used = false;
    private SpriteRenderer sr;

    void Awake()
    {
        sr = GetComponent<SpriteRenderer>();
    }

    void OnCollisionEnter2D(Collision2D collision)
    {
        if (used) return;

        // Check if player hit from below
        foreach (ContactPoint2D contact in collision.contacts)
        {
            if (contact.normal.y > 0.5f && collision.gameObject.CompareTag("Player"))
            {
                ActivateBlock();
                break;
            }
        }
    }

    void ActivateBlock()
    {
        used = true;
        sr.sprite = usedBlockSprite;

        if (itemPrefab != null)
        {
            GameObject item = Instantiate(itemPrefab, transform.position, Quaternion.identity);
            StartCoroutine(PopItem(item));
        }
    }

    IEnumerator PopItem(GameObject item)
    {
        Vector3 start = item.transform.position;
        Vector3 end = start + Vector3.up * itemPopUpDistance;
        float t = 0;
        while (t < 1)
        {
            t += Time.deltaTime * itemPopUpSpeed;
            item.transform.position = Vector3.Lerp(start, end, t);
            yield return null;
        }
        // Optionally, enable item movement script here
        var move = item.GetComponent<ItemMove>();
        if (move != null) move.enabled = true;
    }
}
