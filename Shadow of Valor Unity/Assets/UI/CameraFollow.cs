using UnityEngine;

public class CameraFollow : MonoBehaviour
{
    public Transform target;       // The player/character to follow
    public Vector3 offset;         // Offset from the player
    public float smoothSpeed = 0.125f; // How smoothly the camera follows

    void LateUpdate()
    {
        if (target == null) return;

        // Desired position = target position + offset
        Vector3 desiredPosition = target.position + offset;

        // Smoothly move camera towards desired position
        Vector3 smoothedPosition = Vector3.Lerp(transform.position, desiredPosition, smoothSpeed);

        transform.position = smoothedPosition;
    }
}
