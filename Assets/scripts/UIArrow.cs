using UnityEngine;

public class UIArrow : MonoBehaviour
{
    public float moveDistance = 0.1f;   // how far it moves forward/back
    public float speed = 2f;            // animation speed

    private RectTransform rect;
    private Vector2 startPos;

    void Start()
    {
        rect = GetComponent<RectTransform>();
        startPos = transform.localPosition;
    }

    void Update()
    {
        float offset = Mathf.Sin(Time.time * speed) * moveDistance;
        rect.anchoredPosition = startPos + new Vector2(offset, 0); // Move only on Y
    }
}
