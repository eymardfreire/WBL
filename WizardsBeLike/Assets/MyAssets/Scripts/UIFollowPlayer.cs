using UnityEngine;
using UnityEngine.UI;

public class UIFollowPlayer : MonoBehaviour
{
    public Transform playerTransform;
    private RectTransform rectTransform;
    public Vector2 offset;  // The offset from the player's screen position

    void Awake()
    {
        rectTransform = GetComponent<RectTransform>();
    }

    void Update()
    {
        Vector3 screenPosition = Camera.main.WorldToScreenPoint(playerTransform.position);
        rectTransform.position = screenPosition + (Vector3)offset;
    }
}