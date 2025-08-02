using UnityEngine;

public class TitleRock : MonoBehaviour
{
    // Start is called once before the first execution of Update after the MonoBehaviour is created
    public float rotationAmount = 10f;
    public float speed = 1f;
    
    private RectTransform rectTransform;
    void Start()
    {
        rectTransform = GetComponent<RectTransform>();
    }

    // Update is called once per frame
    void Update()
    {
        float angle = Mathf.Sin(Time.time * speed) * rotationAmount;
        rectTransform.localRotation = Quaternion.Euler(0, 0, angle);
    }
}
