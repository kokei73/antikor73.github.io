using UnityEngine;

/// <summary>
/// A simple orbital camera controller.
/// Allows rotating around the target using the right mouse button and zooming with the scroll wheel.
/// </summary>
public class CameraController : MonoBehaviour
{
    [Header("Target Settings")]
    public Transform target;
    public Vector3 targetOffset = new Vector3(0, 1.2f, 0); // Focus around chest/head level by default

    [Header("Orbit Settings")]
    public float distance = 3.0f;
    public float minDistance = 0.5f;
    public float maxDistance = 7.0f;

    [Header("Sensitivity Settings")]
    public float rotationSpeed = 3.0f;
    public float zoomSpeed = 2.0f;
    public float smoothing = 10.0f; // Smoothness of camera interpolation

    [Header("Rotation Limits")]
    public float yMinLimit = -20f;
    public float yMaxLimit = 80f;

    private float currentX = 0.0f;
    private float currentY = 0.0f;
    private float desiredDistance;
    private Vector3 currentRotation;
    private Vector3 desiredRotation;

    void Start()
    {
        Vector3 angles = transform.eulerAngles;
        currentX = angles.y;
        currentY = angles.x;
        desiredDistance = distance;

        desiredRotation = new Vector3(currentY, currentX, 0);
        currentRotation = desiredRotation;
    }

    void LateUpdate()
    {
        if (target == null) return;

        // Rotate only when holding the Right Mouse Button (1)
        if (Input.GetMouseButton(1))
        {
            currentX += Input.GetAxis("Mouse X") * rotationSpeed;
            currentY -= Input.GetAxis("Mouse Y") * rotationSpeed;

            // Clamp Y axis to prevent camera from flipping upside down
            currentY = Mathf.Clamp(currentY, yMinLimit, yMaxLimit);
        }

        // Handle Zoom via Scroll Wheel
        float scrollAmount = Input.GetAxis("Mouse ScrollWheel");
        if (Mathf.Abs(scrollAmount) > 0.01f)
        {
            desiredDistance -= scrollAmount * zoomSpeed;
            desiredDistance = Mathf.Clamp(desiredDistance, minDistance, maxDistance);
        }

        // Smooth rotation and distance
        desiredRotation = new Vector3(currentY, currentX, 0);
        currentRotation = Vector3.Lerp(currentRotation, desiredRotation, Time.deltaTime * smoothing);
        distance = Mathf.Lerp(distance, desiredDistance, Time.deltaTime * smoothing);

        // Apply Position and Rotation
        Quaternion rotation = Quaternion.Euler(currentRotation);
        Vector3 focusPoint = target.position + targetOffset;
        Vector3 position = focusPoint - (rotation * Vector3.forward * distance);

        transform.rotation = rotation;
        transform.position = position;
    }
}
