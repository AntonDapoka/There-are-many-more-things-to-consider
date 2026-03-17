
using UnityEngine;

public class CameraFollowScript : MonoBehaviour
{
    [SerializeField] private Transform targetTransform;
    [SerializeField] private Transform cameraTransform;

    [Header("Follow Settings")]
    [SerializeField] private Vector3 offset = new(0, 0, -10);
    [SerializeField] private float smoothTime = 0.2f;

    [Header("Bounds (optional)")]
    public bool useBounds = false;
    [SerializeField] private Vector2 minBounds;
    [SerializeField] private Vector2 maxBounds;

    private Vector3 velocity = Vector3.zero;

    private void LateUpdate()
    {
        if (targetTransform == null || cameraTransform == null) return;

        Vector3 desiredPosition = targetTransform.position + offset;

        Vector3 smoothed = Vector3.SmoothDamp(
            cameraTransform.position,
            desiredPosition,
            ref velocity,
            smoothTime
        );

        if (useBounds)
        {
            smoothed.x = Mathf.Clamp(smoothed.x, minBounds.x, maxBounds.x);
            smoothed.y = Mathf.Clamp(smoothed.y, minBounds.y, maxBounds.y);
        }

        cameraTransform.position = smoothed;
    }
}