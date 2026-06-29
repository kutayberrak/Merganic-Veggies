using UnityEngine;

public class BoundarySegment : MonoBehaviour
{
    private BoundaryWatcher watcher;

    private void Awake()
    {
        watcher = GetComponentInParent<BoundaryWatcher>();
    }

    private void OnTriggerEnter(Collider other)
    {
        Debug.Log($"{name} Trigger Enter : {other.name}");
        watcher.OnSegmentEnter(other);
    }

    private void OnTriggerExit(Collider other)
    {
        Debug.Log($"{name} Trigger Exit : {other.name}");

        watcher.OnSegmentExit(other);
    }
}