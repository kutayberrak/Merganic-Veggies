using System.Collections.Generic;
using UnityEngine;

public class PlayerController : MonoBehaviour
{
    [Header("References")]
    public Rigidbody platformRb;
    [SerializeField] private Transform throwableContainer;
    public GameObject gravityCenter;

    [Header("Rotation Settings")]
    public float sensitivity = 10f;
    public float maxSpeed = 180f;
    public float smoothness = 10f;
    public float damping = 0.95f;

    private float currentSpeed;
    private float targetSpeed;

    private Vector2 lastTouchPos;
    private bool isDragging;

    [Header("Throw Settings")]
    [SerializeField] private GameObject throwablePrefab;
    [SerializeField] private Transform spawnPoint;

    private Throwable currentThrowable;
    private List<Rigidbody> activeFruits = new List<Rigidbody>();


    public static PlayerController Instance { get; private set; }
    void Awake()
    {
        if (Instance != null && Instance != this)
        {
            Destroy(gameObject);
            return;
        }

        Instance = this;
    }

    void Start()
    {
        SpawnNewThrowable();
    }

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
            Throw();
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

        throwableContainer.RotateAround(gravityCenter.transform.position, Vector3.up, rotation);
    }
    void Throw()
    {
        if (currentThrowable == null) return;

        Vector3 dir = GetCenter() - spawnPoint.position;
        dir.y = 0f;
        dir.Normalize();


        currentThrowable.enabled = true;
        currentThrowable.Launch(dir);

        currentThrowable = null;

        SpawnNewThrowable();
    }

    void SpawnNewThrowable()
    {
        GameObject obj = Instantiate(throwablePrefab, spawnPoint.position, Quaternion.identity);
        obj.transform.SetParent(null); // parent'ı kesinlikle kır

        if (obj.TryGetComponent<Throwable>(out var throwable))
        {
            currentThrowable = throwable;
            currentThrowable.enabled = false;
        }
    }

    public void RegisterFruit(Rigidbody rb)
    {
        if (!activeFruits.Contains(rb))
        {
            activeFruits.Add(rb);

            rb.transform.SetParent(throwableContainer);
        }
    }

    public Vector3 GetCenter()
    {
        return gravityCenter.transform.position;
    }
}