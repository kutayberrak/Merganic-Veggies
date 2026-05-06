using UnityEngine;

public class GravityCenter : MonoBehaviour
{
    public float pullSpeed = 5f;      // merkeze çekilme hızı
    public float steeringForce = 2f; // yön düzeltme gücü
    public float maxDistance = 5f;

    private void FixedUpdate()
    {
        Collider[] cols = Physics.OverlapSphere(transform.position, maxDistance);

        foreach (var col in cols)
        {
            Rigidbody rb = col.attachedRigidbody;
            if (rb == null || !rb.CompareTag("Fruit")) continue;

            if (!rb.TryGetComponent<Throwable>(out var obj)) continue;

            if (!obj.gravityEnabled) continue;

            Vector3 dir = transform.position - rb.position;

            // sadece XZ düzleminde
            dir.y = 0f;

            float dist = dir.magnitude;
            if (dist < 0.05f) continue;

            Vector3 targetDir = dir.normalized;

            Vector3 velocity = rb.linearVelocity;

            Vector3 targetVelocity = targetDir * pullSpeed;

            Vector3 steering = (targetVelocity - velocity) * 0.3f;

            rb.AddForce(steering * steeringForce, ForceMode.Acceleration);
        }
    }
}