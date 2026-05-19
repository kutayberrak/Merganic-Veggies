using System.Collections;
using UnityEngine;

public class PlayerController : MonoBehaviour
{
    public static PlayerController Instance { get; private set; }

    [Header("References")]
    [SerializeField] private PlatformRotator platformRotator;
    [SerializeField] private Transform gravityCenter;
    [SerializeField] private Transform spawnPoint;

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
    [SerializeField] private float throwCooldown = 0.5f;
    private bool canThrow = true;
    private Throwable currentThrowable;

    void Awake()
    {
        if (Instance != null && Instance != this) { Destroy(gameObject); return; }
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

        targetSpeed = Mathf.Clamp(targetSpeed, -maxSpeed, maxSpeed);
        currentSpeed = Mathf.Lerp(currentSpeed, targetSpeed, Time.deltaTime * smoothness);

        if (!isDragging)
            targetSpeed *= damping;

        platformRotator.CurrentSpeed = currentSpeed;
    }

    void Throw()
    {
        if (currentThrowable == null || !canThrow) return;

        canThrow = false;

        Vector3 dir = gravityCenter.position - spawnPoint.position;
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
        currentThrowable = ObjectPoolManager.Instance.GetRandomObject(spawnPoint.position, Quaternion.identity);
        if (currentThrowable == null) return;
        //currentThrowable.transform.SetParent(null);
        currentThrowable.enabled = false;
    }

    IEnumerator ThrowCooldownRoutine()
    {
        yield return new WaitForSeconds(throwCooldown);
        canThrow = true;
    }

    public Vector3 GetCenter() => gravityCenter.position;
}