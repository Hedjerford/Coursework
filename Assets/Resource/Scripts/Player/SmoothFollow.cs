using UnityEngine;

public class SmoothFollow : MonoBehaviour
{
    public Transform target;      // —юда перетащи объект игрока
    public float smoothSpeed = 0.125f; // „ем меньше, тем плавнее
    public Vector3 offset;        // —мещение, если нужно

    private void LateUpdate()
    {
        if (target == null) return;

        Vector3 desiredPosition = target.position + offset;
        Vector3 smoothedPosition = Vector3.Lerp(transform.position, desiredPosition, smoothSpeed);
        transform.position = new Vector3(smoothedPosition.x, smoothedPosition.y, transform.position.z);
    }
}
