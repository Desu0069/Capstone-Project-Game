using UnityEngine;
using UnityEngine.UI;

public class OnScreenPointer : MonoBehaviour
{
    public Transform target; // Assign the 3D object here
    public Camera mainCamera; // Assign your main camera here
    public RectTransform canvasRect; // Assign the Canvas's RectTransform

    private RectTransform pointerRect;

    void Awake()
    {
        pointerRect = GetComponent<RectTransform>();
        if (mainCamera == null) mainCamera = Camera.main;
    }

    void Update()
    {
        if (target == null)
        {
            gameObject.SetActive(false);
            return;
        }

        Vector3 screenPoint = mainCamera.WorldToViewportPoint(target.position);

        bool isOffScreen = screenPoint.z < 0 ||
                           screenPoint.x < 0 || screenPoint.x > 1 ||
                           screenPoint.y < 0 || screenPoint.y > 1;

        if (!isOffScreen)
        {
            // Target is on-screen: position pointer over target
            pointerRect.anchoredPosition = WorldToCanvasPosition(target.position);
            pointerRect.rotation = Quaternion.identity;
        }
        else
        {
            // Target is off-screen: clamp to screen edge
            Vector3 fromCenter = screenPoint - new Vector3(0.5f, 0.5f, 0);
            fromCenter.z = 0;
            fromCenter.Normalize();

            float edgeBuffer = 50f; // pixels from edge
            Vector2 canvasCenter = new Vector2(canvasRect.rect.width, canvasRect.rect.height) / 2f;
            Vector2 pointerPos = canvasCenter + new Vector2(fromCenter.x, fromCenter.y) * (canvasCenter.magnitude - edgeBuffer);
            pointerRect.anchoredPosition = pointerPos;

            // Rotate the pointer to face the target direction
            float angle = Mathf.Atan2(fromCenter.y, fromCenter.x) * Mathf.Rad2Deg;
            pointerRect.rotation = Quaternion.Euler(0, 0, angle - 90f);
        }

        gameObject.SetActive(true);
    }

    Vector2 WorldToCanvasPosition(Vector3 worldPos)
    {
        Vector2 viewportPosition = mainCamera.WorldToViewportPoint(worldPos);
        Vector2 canvasSize = canvasRect.sizeDelta;
        return new Vector2(
            (viewportPosition.x - 0.5f) * canvasSize.x,
            (viewportPosition.y - 0.5f) * canvasSize.y
        );
    }
}