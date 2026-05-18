using UnityEngine;

/// <summary>
/// Allows the player to interact with the character (e.g., hair/clothes) using KawaiiPhysics
/// by mapping mouse cursor position to a virtual 3D collider. Also detects basic petting.
/// </summary>
public class MouseInteraction : MonoBehaviour
{
    [Header("Interaction Settings")]
    public float interactionDepth = 1.0f; // Distance from camera to interaction plane
    public float colliderRadius = 0.1f;
    public LayerMask characterLayer;

    private GameObject interactionSphere;
    private SphereCollider sphereCollider;

    [Header("Petting Detection")]
    public float pettingSpeedThreshold = 1.0f;
    public float pettingTimeRequired = 1.5f;

    private CharacterExpressions expressions;
    private Vector3 lastMousePos;
    private float pettingTimer = 0f;

    void Start()
    {
        // Create an invisible sphere that follows the mouse
        interactionSphere = new GameObject("MouseInteractionCollider");
        sphereCollider = interactionSphere.AddComponent<SphereCollider>();
        sphereCollider.radius = colliderRadius;

        // Add Rigidbody to interact with physics
        Rigidbody rb = interactionSphere.AddComponent<Rigidbody>();
        rb.isKinematic = true;

        // Set layer (should ideally match a layer that Kawaii Physics collides with)
        interactionSphere.layer = LayerMask.NameToLayer("Default");

        expressions = GetComponent<CharacterExpressions>();
        lastMousePos = Input.mousePosition;
    }

    void Update()
    {
        UpdateColliderPosition();
        DetectPetting();
    }

    private void UpdateColliderPosition()
    {
        if (Camera.main != null)
        {
            // Cast ray from mouse to screen
            Ray ray = Camera.main.ScreenPointToRay(Input.mousePosition);

            // Raycast against character to find exact surface depth, otherwise use default
            if (Physics.Raycast(ray, out RaycastHit hit, 10f, characterLayer))
            {
                interactionSphere.transform.position = hit.point;
            }
            else
            {
                Vector3 virtualPoint = ray.GetPoint(interactionDepth);
                interactionSphere.transform.position = virtualPoint;
            }
        }
    }

    private void DetectPetting()
    {
        // If holding left click and moving mouse quickly over character
        if (Input.GetMouseButton(0))
        {
            float mouseSpeed = (Input.mousePosition - lastMousePos).magnitude / Time.deltaTime;

            Ray ray = Camera.main.ScreenPointToRay(Input.mousePosition);
            if (Physics.Raycast(ray, out RaycastHit hit, 10f, characterLayer))
            {
                // Check if hitting head/hair region (simplified check by height relative to character root)
                if (hit.point.y > transform.position.y + 1.4f && mouseSpeed > pettingSpeedThreshold)
                {
                    pettingTimer += Time.deltaTime;
                    if (pettingTimer > pettingTimeRequired)
                    {
                        if (expressions != null)
                        {
                            expressions.SetEmotion("smile");
                        }
                        pettingTimer = 0f; // Reset
                    }
                }
                else
                {
                    pettingTimer = Mathf.Max(0, pettingTimer - Time.deltaTime);
                }
            }
        }
        else
        {
            pettingTimer = 0f;
        }

        lastMousePos = Input.mousePosition;
    }
}
