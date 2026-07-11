using UnityEngine;

public class CameraFollow : MonoBehaviour
{
    // Drag your player GameObject into this slot in the Inspector
    public Transform target;

    // Adjust the distance and height of the camera relative to the player
    public Vector3 offset = new Vector3(0f, 5f, -10f);

    // Lower values mean smoother/slower tracking; higher values mean tighter tracking
    public float smoothSpeed = 10f;

    // LateUpdate runs after all standard Update functions, preventing camera jitter
    void LateUpdate()
    {
        // Ensure a target exists to prevent errors
        if (target == null) return;

        // Calculate where the camera wants to go
        Vector3 desiredPosition = target.position + offset;

        // Smoothly interpolate from the current position to the desired position
        Vector3 smoothedPosition = Vector3.Lerp(transform.position, desiredPosition, smoothSpeed * Time.deltaTime);

        // Apply the smoothed position to the camera
        transform.position = smoothedPosition;
    }
}