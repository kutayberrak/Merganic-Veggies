using System.Collections.Generic;
using System.Collections;
using UnityEngine;

public class PlayerController : MonoBehaviour
{
    [Header("References")]
    [SerializeField] private Transform orbitRoot;
    [SerializeField] private Transform throwableContainer;
    [SerializeField] private Transform gravityCenter;

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
    [SerializeField] private Transform spawnPoint;
    [SerializeField] private float throwCooldown = 0.5f;
    private bool canThrow = true;


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
        currentSpeed = Mathf.Lerp(currentSpeed,targetSpeed,Time.deltaTime * smoothness);

        // Damping
        if (!isDragging)
        {
            targetSpeed *= damping;
        }
    }

    void ApplyRotation()
    {
        float rotation = currentSpeed * Time.deltaTime;

        orbitRoot.Rotate(0f, rotation, 0f, Space.World);
    }

    void Throw()
    {
        if (currentThrowable == null || !canThrow) return;

        canThrow = false;

        Vector3 dir = GetCenter() - spawnPoint.position;

        dir.y = 0f;

        dir.Normalize();

        currentThrowable.enabled = true;

        currentThrowable.Launch(dir);

        currentThrowable = null;

        SpawnNewThrowable();

        StartCoroutine(ThrowCooldownRoutine());
    }

    void SpawnNewThrowable()
    {
        currentThrowable = ObjectPoolManager.Instance.GetRandomObject(spawnPoint.position,Quaternion.identity);

        if (currentThrowable == null) return;

        currentThrowable.transform.SetParent(null);

        currentThrowable.enabled = false;
    }

    public void RegisterFruit(Rigidbody rb)
    {
        if (!activeFruits.Contains(rb))
        {
            activeFruits.Add(rb);

            rb.transform.SetParent(throwableContainer);
        }
    }

    IEnumerator ThrowCooldownRoutine()
    {
        yield return new WaitForSeconds(throwCooldown);

        canThrow = true;
    }
    public Vector3 GetCenter()
    {
        return gravityCenter.position;
    }
}