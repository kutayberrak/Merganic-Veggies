using UnityEngine;

public class PlayerController : MonoBehaviour
{
    [Header("References")]
    public Rigidbody platformRb;

    [Header("Rotation Settings")]
    public float sensitivity = 10f;
    public float maxSpeed = 180f;
    public float smoothness = 10f;
    public float damping = 0.95f;

    private float currentSpeed;
    private float targetSpeed;

    private Vector2 lastTouchPos;
    private bool isDragging;

    void Update()
    {
        HandleInput();
    }

    void FixedUpdate()
    {
        ApplyRotation();
    }

    void HandleInput()
    {
        if (Input.GetMouseButtonDown(0))
        {
            isDragging = true;
            lastTouchPos = Input.mousePosition;
        }
        else if (Input.GetMouseButtonUp(0))
        {
            isDragging = false;
        }

        if (isDragging)
        {
            Vector2 currentPos = Input.mousePosition;
            Vector2 delta = currentPos - lastTouchPos;

            targetSpeed = delta.x * sensitivity;

            lastTouchPos = currentPos;
        }

        // Clamp speed
        targetSpeed = Mathf.Clamp(targetSpeed, -maxSpeed, maxSpeed);

        // Smooth transition
        currentSpeed = Mathf.Lerp(currentSpeed, targetSpeed, Time.deltaTime * smoothness);

        // Damping
        if (!isDragging)
        {
            targetSpeed *= damping;
        }
    }

    void ApplyRotation()
    {
        float rotation = currentSpeed * Time.fixedDeltaTime;

        Quaternion deltaRotation = Quaternion.Euler(0f, rotation, 0f);
        platformRb.MoveRotation(platformRb.rotation * deltaRotation);
    }
}