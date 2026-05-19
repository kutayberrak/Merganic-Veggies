using UnityEngine;

public class Throwable : MonoBehaviour
{
    [SerializeField] protected ThrowableData throwableData;
    public ThrowableData Data => throwableData;
    public Vector3 Position => rb.position;

    public bool gravityEnabled = false;
    public bool isLaunched = false;

    [Header("Move Settings")]
    public float moveSpeed = 15f;

    protected Rigidbody rb;
    private Collider col;

    protected virtual void Awake()
    {
        rb = GetComponent<Rigidbody>();
        col = GetComponent<Collider>();
        col.enabled = false;
    }

    public void Launch(Vector3 direction)
    {
        isLaunched = true;
        gravityEnabled = false;
        rb.linearVelocity = Vector3.zero;
        rb.angularVelocity = Vector3.zero;
        col.enabled = true;
    }

    private void FixedUpdate()
    {
        if (!isLaunched || gravityEnabled) return;

        Vector3 toCenter = PlayerController.Instance.GetCenter() - rb.position;
        toCenter.y = 0f;
        toCenter.Normalize();

        rb.linearVelocity = toCenter * moveSpeed;
    }

    protected virtual void OnCollisionEnter(Collision collision)
    {
        if (!gravityEnabled)
        {
            gravityEnabled = true;
            rb.angularVelocity = Vector3.zero;
            FruitRegistry.Instance.Register(rb);
        }
    }
    public virtual void ResetForSpawn()
    {
        isLaunched = false;
        gravityEnabled = false;
        rb.linearVelocity = Vector3.zero;
        rb.angularVelocity = Vector3.zero;
        col.enabled = false;
    }
    public virtual void ResetForPool()
    {
        isLaunched = false;
        gravityEnabled = false;
        rb.linearVelocity = Vector3.zero;
        rb.angularVelocity = Vector3.zero;
        col.enabled = false;
        FruitRegistry.Instance.Unregister(rb);
    }
    public void ActivateAsLanded()
    {
        isLaunched = true;
        gravityEnabled = true;
        col.enabled = true;
        rb.linearVelocity = Vector3.zero;
        rb.angularVelocity = Vector3.zero;
        FruitRegistry.Instance.Register(rb);
    }

    protected virtual void Merge(Throwable other) { }
}