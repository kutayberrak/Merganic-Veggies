using System.Collections.Generic;
using UnityEngine;

public class GravityCenter : MonoBehaviour
{
    [Header("Pull Settings")]
    public float pullSpeed = 5f;
    public float steeringForce = 5f;
    public float stopDistance = 0.05f;

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
            if (dist < stopDistance) continue;

            Vector3 targetVelocity = dir.normalized * pullSpeed;

            Vector3 currentVel = rb.linearVelocity;
            currentVel.y = 0f;

            Vector3 steering = (targetVelocity - currentVel) * steeringForce;
            rb.AddForce(steering, ForceMode.Acceleration);
        }
    }
}