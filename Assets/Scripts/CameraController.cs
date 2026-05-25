using UnityEngine;

public class CameraController : MonoBehaviour
{
    [Header("Movement")]
    public float moveSpeed = 10f;

    [Header("Zoom")]
    public float scrollZoomSpeed = 2f;
    public float minZoom = 5f;
    public float maxZoom = 50f;

    [Header("Bounds")]
    public float minX = -50f;
    public float maxX = 50f;
    public float minY = -50f;
    public float maxY = 50f;

    [Header("Reset")]
    public KeyCode resetKey = KeyCode.O;
    public float resetLerpDuration = 0.2f;

    private Camera cam;
    private Transform shipTransform;

    // Reset lerp state
    private bool isResetting;
    private Vector3 resetStartPos;
    private float resetStartTime;

    // Drag state
    private bool isDragging;
    private Vector3 dragScreenOrigin;
    private Vector3 dragWorldOrigin;

    private void Start()
    {
        cam = GetComponent<Camera>();
        if (cam == null)
            cam = Camera.main;

        GameObject player = GameObject.FindWithTag("Player");
        if (player != null)
            shipTransform = player.transform;
    }

    private void Update()
    {
        Vector3 targetPos = transform.position;

        // --- Reset to ship (O key) ---
        if (Input.GetKeyDown(resetKey) && shipTransform != null)
        {
            isResetting = true;
            resetStartPos = transform.position;
            resetStartTime = Time.time;
        }

        if (isResetting && shipTransform != null)
        {
            float elapsed = Time.time - resetStartTime;
            float t = Mathf.Clamp01(elapsed / resetLerpDuration);
            // Smoothstep easing
            float eased = t * t * (3f - 2f * t);
            targetPos = Vector3.Lerp(resetStartPos, shipTransform.position, eased);
            targetPos.z = transform.position.z; // preserve Z

            if (t >= 1f)
                isResetting = false;
        }
        else
        {
            // --- WASD movement ---
            float h = Input.GetAxisRaw("Horizontal");
            float v = Input.GetAxisRaw("Vertical");
            Vector3 moveInput = new Vector3(h, v, 0f).normalized;
            targetPos += moveInput * moveSpeed * (cam.orthographicSize / minZoom) * Time.deltaTime;

            // --- Middle mouse drag ---
            if (Input.GetMouseButtonDown(2))
            {
                isDragging = true;
                dragScreenOrigin = Input.mousePosition;
                dragWorldOrigin = transform.position;
            }

            if (Input.GetMouseButtonUp(2))
                isDragging = false;

            if (isDragging)
            {
                Vector3 screenDelta = Input.mousePosition - dragScreenOrigin;
                // Convert screen pixels to world units based on zoom level
                float worldPerPixel = 2f * cam.orthographicSize / cam.pixelHeight;
                Vector3 worldDelta = new Vector3(
                    -screenDelta.x * worldPerPixel,
                    -screenDelta.y * worldPerPixel,
                    0f
                );
                targetPos = dragWorldOrigin + worldDelta;
            }

            // --- Scroll zoom ---
            float scroll = Input.GetAxis("Mouse ScrollWheel");
            if (Mathf.Abs(scroll) > 0.01f)
            {
                float newSize = cam.orthographicSize - scroll * scrollZoomSpeed * (cam.orthographicSize / minZoom);
                cam.orthographicSize = Mathf.Clamp(newSize, minZoom, maxZoom);
            }
        }

        // Clamp position to bounds
        targetPos.x = Mathf.Clamp(targetPos.x, minX, maxX);
        targetPos.y = Mathf.Clamp(targetPos.y, minY, maxY);
        targetPos.z = transform.position.z;

        transform.position = targetPos;
    }

    /// <summary>
    /// Set the camera position and zoom programmatically. Cancels any active reset lerp.
    /// </summary>
    public void SetPositionImmediate(Vector3 worldPos)
    {
        isResetting = false;
        Vector3 clamped = worldPos;
        clamped.x = Mathf.Clamp(clamped.x, minX, maxX);
        clamped.y = Mathf.Clamp(clamped.y, minY, maxY);
        clamped.z = transform.position.z;
        transform.position = clamped;
    }

    /// <summary>
    /// Set zoom level programmatically.
    /// </summary>
    public void SetZoomImmediate(float orthographicSize)
    {
        cam.orthographicSize = Mathf.Clamp(orthographicSize, minZoom, maxZoom);
    }
}
