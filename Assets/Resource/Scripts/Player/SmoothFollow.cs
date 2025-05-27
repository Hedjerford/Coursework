using UnityEngine;

public class SmoothFollow : MonoBehaviour
{
    public Transform target;
    public float smoothSpeed = 0.125f;
    public Vector3 offset;

    public float cameraZoomSpeed = 2f;
    public float targetOrthoSize = 5f;

    private float currentOrthoSize;

    void Start()
    {
        if (Camera.main != null)
        {
            currentOrthoSize = Camera.main.orthographicSize;
            targetOrthoSize = currentOrthoSize;
        }
    }

    void LateUpdate()
    {
        if (target == null || Camera.main == null) return;

        Vector3 desiredPosition = target.position + offset;
        Vector3 smoothed = Vector3.Lerp(transform.position, desiredPosition, smoothSpeed);
        transform.position = new Vector3(smoothed.x, smoothed.y, transform.position.z);

        // Плавное отдаление камеры
        currentOrthoSize = Mathf.Lerp(currentOrthoSize, targetOrthoSize, Time.deltaTime * cameraZoomSpeed);
        Camera.main.orthographicSize = currentOrthoSize;
    }

    public void SetTarget(Transform newTarget)
    {
        target = newTarget;
    }

    public void SetZoom(float newOrthoSize)
    {
        targetOrthoSize = newOrthoSize;
    }
}
