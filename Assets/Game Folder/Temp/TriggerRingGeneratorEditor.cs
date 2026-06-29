using UnityEditor;
using UnityEngine;

[CustomEditor(typeof(TriggerRingGenerator))]
public class TriggerRingGeneratorEditor : Editor
{
    public override void OnInspectorGUI()
    {
        DrawDefaultInspector();

        if (GUILayout.Button("Generate"))
        {
            Generate((TriggerRingGenerator)target);
        }
    }

    private void Generate(TriggerRingGenerator ring)
    {
        float angleStep = 360f / ring.segments;
        float length = 2f * ring.radius * Mathf.Tan(Mathf.PI / ring.segments);

        for (int i = 0; i < ring.segments; i++)
        {
            float angle = angleStep * i;

            GameObject segment = new GameObject($"Segment_{i}");
            segment.transform.SetParent(ring.transform);

            segment.transform.localPosition =
                Quaternion.Euler(0, angle, 0) * Vector3.forward * ring.radius;

            segment.transform.localRotation =
                Quaternion.Euler(0, angle, 0);

            BoxCollider box = segment.AddComponent<BoxCollider>();
            box.isTrigger = true;
            box.size = new Vector3(ring.thickness, ring.height, length);
        }
    }
}