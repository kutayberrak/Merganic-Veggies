using System.Collections.Generic;
using UnityEngine;

public class GravityCenter : MonoBehaviour
{
    [Header("Pull Settings")]
    public float pullSpeed = 5f;
    public float falloffRadius = 1.5f; // Smoothly reduces pull force near the center
    public float steeringForce = 5f;

    [Header("Stability")]
    public float maxSpeed = 3f; // Prevents sudden velocity spikes from collisions

    [Header("Damping")]
    public float baseDamping = 0.5f; // Low damping while objects are moving
    public float settledDamping = 2f; // Higher damping near center to reduce jitter

    private void FixedUpdate()
    {
        List<Rigidbody> fruits = FruitRegistry.Instance.ActiveFruits;

        for (int i = 0; i < fruits.Count; i++)
        {
            Rigidbody rb = fruits[i];
            if (rb == null) continue;

            Vector3 dir = transform.position - rb.position;
            dir.y = 0f;

            float dist = dir.magnitude;

            // Smooth pull curve
            // dist=0 -> falloff=0, dist>=falloffRadius -> falloff=1
            float falloff = Mathf.SmoothStep(0f, 1f, Mathf.Clamp01(dist / falloffRadius));

            Vector3 targetVelocity = dist > 0.0001f
                ? (dir / dist) * pullSpeed * falloff
                : Vector3.zero;

            Vector3 currentVel = rb.linearVelocity;
            currentVel.y = 0f;

            Vector3 steering = (targetVelocity - currentVel) * steeringForce;
            rb.AddForce(steering, ForceMode.Acceleration);

            ClampHorizontalSpeed(rb);

            rb.linearDamping = Mathf.Lerp(settledDamping, baseDamping, falloff);
        }
    }

    // Limits sudden speed increases caused by collisions
    private void ClampHorizontalSpeed(Rigidbody rb)
    {
        Vector3 vel = rb.linearVelocity;
        float yVel = vel.y;
        vel.y = 0f;

        if (vel.sqrMagnitude > maxSpeed * maxSpeed)
        {
            vel = vel.normalized * maxSpeed;
            rb.linearVelocity = new Vector3(vel.x, yVel, vel.z);
        }
    }
}