using UnityEngine;

public class CameraFollow : MonoBehaviour
{
    [Header("Camera Follow Settings")]
    [SerializeField] private Transform player;
    [SerializeField] private Vector3 offset = new(0, 10, -5);
    [SerializeField] private float smoothSpeed = 0.125f;

    [Header("Camera Bounds Settings")]
    [SerializeField] private bool useBounds = false;
    [SerializeField] private Vector2 minBounds, maxBounds;

    private void LateUpdate()
    {
        if (player == null) return;

        Vector3 desiredPosition = player.position + offset;

        if (useBounds)
        {
            desiredPosition.x = Mathf.Clamp(desiredPosition.x, minBounds.x, maxBounds.x);
            desiredPosition.z = Mathf.Clamp(desiredPosition.z, minBounds.y, maxBounds.y);
            desiredPosition.y = transform.position.y;
        }

        Vector3 smoothedPosition = Vector3.Lerp(transform.position, desiredPosition, smoothSpeed);
        transform.SetPositionAndRotation(smoothedPosition, Quaternion.Euler(90f, 0f, 0f));
    }
}
