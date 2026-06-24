using System.Collections.Generic;
using UnityEngine;

public class PlatformRotator : MonoBehaviour
{
    [SerializeField] private Transform orbitRoot;
    [SerializeField] private Transform gravityCenter;

    public float CurrentSpeed { get; set; }

    private void FixedUpdate()
    {
        float rotation = CurrentSpeed * Time.fixedDeltaTime;

        // Rotate platform
        orbitRoot.Rotate(0f, rotation, 0f, Space.World);

        if (Mathf.Approximately(rotation, 0f)) return;

        // Rotate fruits
        Quaternion deltaRot = Quaternion.Euler(0f, rotation, 0f);
        Vector3 pivot = gravityCenter.position;

        List<Rigidbody> fruits = FruitRegistry.Instance.ActiveFruits;
        for (int i = 0; i < fruits.Count; i++)
        {
            Rigidbody rb = fruits[i];
            if (rb == null) continue;

            Vector3 newPos = pivot + (deltaRot * (rb.position - pivot));
            rb.MovePosition(newPos);
            rb.MoveRotation(rb.rotation * deltaRot);

            rb.linearVelocity = deltaRot * rb.linearVelocity;
            rb.angularVelocity = deltaRot * rb.angularVelocity;
        }
    }
}