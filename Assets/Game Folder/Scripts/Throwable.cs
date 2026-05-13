using UnityEngine;

public class Throwable : MonoBehaviour
{
    [SerializeField] protected ThrowableData throwableData;

    public bool gravityEnabled = false;
    public bool isLaunched = false;

    [Header("Move Settings")]
    public float moveSpeed = 15f;

    private Rigidbody rb;
    private Collider col;

    protected virtual void Awake()
    {
        rb = GetComponent<Rigidbody>();
        col = GetComponent<Collider>();

        col.enabled = false;
    }
    private void FixedUpdate()
    {
        if (!isLaunched || gravityEnabled) return;

        Vector3 toCenter = PlayerController.Instance.GetCenter() - rb.position;
        toCenter.y = 0f;
        toCenter.Normalize();

        rb.linearVelocity = toCenter * moveSpeed;
    }
    public void Launch(Vector3 direction)
    {
        isLaunched = true;
        gravityEnabled = false;

        rb.linearVelocity = Vector3.zero;
        rb.angularVelocity = Vector3.zero;

        col.enabled = true;
    }

    protected virtual void OnCollisionEnter(Collision collision)
    {
        if (!gravityEnabled)
        {
            gravityEnabled = true;

            rb.linearVelocity = Vector3.zero;
            rb.angularVelocity = Vector3.zero;

            PlayerController.Instance.RegisterFruit(rb);
        }
    }
}