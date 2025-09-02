using UnityEngine;
using UnityEngine.UI;

namespace OnScreenPointerPlugin
{
    public class OnScreenPointerObject : MonoBehaviour
    {
        public Vector2 offset_local;
        public bool moveInCircle = false;
        [Range(0f, 1f)]
        public float circleSizeNormalized = 0.5f;

        public Sprite inScreenSprite;
        public Sprite outScreenSprite;
        public Image uiImagePrefab;

        public float pointerEdgeMargin = 40f;

        private Image uiImage;
        private bool isPointerInScreen = false;

        private OnScreenPointerController onScreenPointerController
        {
            get
            {
                return OnScreenPointerController.Instance;
            }
        }

        private int screenSizeX
        {
            get
            {
                return onScreenPointerController.playerCamera.pixelWidth;
            }
        }
        private int screenSizeY
        {
            get
            {
                return onScreenPointerController.playerCamera.pixelHeight;
            }
        }

        private Vector2 ScreenMidPoint
        {
            get
            {
                return new Vector2((int)screenSizeX / 2, (int)screenSizeY / 2);
            }
        }

        private Camera camera_local { get { return OnScreenPointerController.Instance.playerCamera; } }
        public event System.Action<OnScreenPointerObject> OnMarkerDisabled;
        void Awake()
        {
            uiImage = Instantiate<Image>(uiImagePrefab);
            uiImage.raycastTarget = false;
            uiImage.rectTransform.SetParent(onScreenPointerController.uiContainerOfPointers, false);
        }

        private void OnEnable()
        {
            if (uiImage != null)
                uiImage.gameObject.SetActive(true);
        }

        private void OnDisable()
        {
            if (uiImage != null)
                uiImage.gameObject.SetActive(false);
        }

        private void OnDestroy()
        {
            // Check if uiImage exists and hasn't already been destroyed
            if (uiImage != null && uiImage.gameObject != null)
            {
                Destroy(uiImage.gameObject);
            }
        }

        void Update()
        {
            // If marker is manually disabled, don't update its position
            if (uiImage == null || !uiImage.gameObject.activeSelf)
                return;

            var screenPos = MyScreenPosition(transform);

            isPointerInScreen = IsPointerInScreen(screenPos);

            if (isPointerInScreen)
            {
                uiImage.sprite = inScreenSprite;
                uiImage.transform.rotation = Quaternion.Euler(0, 0, 0);
            }
            else
            {
                uiImage.sprite = outScreenSprite;
                Vector2 screenPosCentered = (Vector2)screenPos - ScreenMidPoint;

                if (screenPos.z < 0)//object is behind player/camera
                {
                    screenPosCentered = screenPosCentered * -1;
                }

                float angle = Mathf.Atan2(screenPosCentered.y, screenPosCentered.x);
                screenPosCentered = PositionPointerObjectOffScreen(angle);
                screenPos = screenPosCentered + ScreenMidPoint;

                uiImage.transform.rotation = Quaternion.Euler(0, 0, angle * Mathf.Rad2Deg);
            }

            screenPos = ClampToOffsetBounds(screenPos);
            uiImage.transform.position = screenPos;
        }

        private Vector2 PositionPointerObjectOffScreen(float angle)
        {
            float margin = pointerEdgeMargin;
            float halfWidth = screenSizeX / 2f;
            float halfHeight = screenSizeY / 2f;

            // Calculate max x/y for pointer, minus margin so it's always inside canvas
            float maxX = halfWidth - margin;
            float maxY = halfHeight - margin;

            // Get normalized direction
            float cos = Mathf.Cos(angle);
            float sin = Mathf.Sin(angle);

            // Find how far the pointer can go in x/y without crossing margin
            float x = cos * maxX;
            float y = sin * maxY;

            // Clamp so the pointer never passes the margin boundary
            x = Mathf.Clamp(x, -maxX, maxX);
            y = Mathf.Clamp(y, -maxY, maxY);

            return new Vector2(x, y);
        }

        private Vector2 ClampToOffsetBounds(Vector2 screenPos)
        {
            int x = (int)Mathf.Clamp(screenPos.x, offset_local.x * screenSizeX, screenSizeX - offset_local.x * screenSizeX);
            int y = (int)Mathf.Clamp(screenPos.y, offset_local.y * screenSizeY, screenSizeY - offset_local.y * screenSizeY);

            return new Vector2(x, y);
        }

        private Vector3 MyScreenPosition(Transform transform)
        {
            var screenpos = onScreenPointerController.playerCamera.WorldToScreenPoint(transform.position);
            return screenpos;
        }

        private bool IsPointerInScreen(Vector3 screenPosition)
        {
            bool isTargetVisible = screenPosition.z > 0 && screenPosition.x > 0 && screenPosition.x < camera_local.pixelWidth && screenPosition.y > 0 && screenPosition.y < camera_local.pixelHeight;
            return isTargetVisible;
        }

        // --- NEW CODE BELOW ---

        // Disables the marker (can call from other scripts or triggers)
        public void DisableMarker()
        {
            if (uiImage != null)
                uiImage.gameObject.SetActive(false);

            // Fire event if anyone is listening
            OnMarkerDisabled?.Invoke(this);
        }

        // Enables the marker again (optional)
        public void EnableMarker()
        {
            if (uiImage != null)
                uiImage.gameObject.SetActive(true);
        }

        // Disables the marker if the player is within a certain range
        public void DisableMarkerIfInRange(Transform player, float range)
        {
            if (player == null) return;
            float distance = Vector3.Distance(player.position, transform.position);
            if (distance <= range)
            {
                DisableMarker();
            }
        }

        // Example (optional): for trigger colliders on this object
        private void OnTriggerEnter(Collider other)
        {
            if (other.CompareTag("Player"))
            {
                DisableMarker();
            }
        }
    }
}