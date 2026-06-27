using System.Collections.Generic;
using UnityEngine;

public class PlatformRotator : MonoBehaviour
{
    [SerializeField] private Transform orbitRoot;

    public float CurrentSpeed { get; set; }

    private void FixedUpdate()
    {
        float rotation = CurrentSpeed * Time.fixedDeltaTime;

        // Rotate platform
        orbitRoot.Rotate(0f, rotation, 0f, Space.World);

        if (Mathf.Approximately(rotation, 0f)) return;
    }
}