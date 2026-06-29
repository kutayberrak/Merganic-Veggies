using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BoundaryWatcher : MonoBehaviour
{
    public static BoundaryWatcher Instance { get; private set; }

    [SerializeField] private float graceDuration = 5f;
    [SerializeField] private bool debugLogs = true;
    [SerializeField] private bool drawGizmos = true;
    public static event Action OnBoundaryExceeded;

    private readonly Dictionary<Throwable, int> contactCounts = new();
    private readonly Dictionary<Throwable, Coroutine> runningTimers = new();

    private bool triggered;

    private void Awake()
    {
        if (Instance != null && Instance != this)
        {
            Destroy(gameObject);
            return;
        }

        Instance = this;
    }

    public void OnSegmentEnter(Collider other)
    {
        if (triggered) return;

        if (!other.TryGetComponent(out Throwable throwable))
            return;

        if (!throwable.gravityEnabled)
            return;

        if (!contactCounts.TryGetValue(throwable, out int count))
            count = 0;

        count++;
        contactCounts[throwable] = count;

        if (debugLogs)
        {
            Debug.Log($"[Boundary] ENTER -> {throwable.name} | Contacts: {count}");
        }

        if (count == 1)
        {
            if (debugLogs)
            {
                Debug.Log($"[Boundary] {throwable.name} danger timer started.");
            }

            Coroutine timer = StartCoroutine(DangerTimer(throwable));
            runningTimers.Add(throwable, timer);
        }
    }
    public void OnSegmentExit(Collider other)
    {
        if (!other.TryGetComponent(out Throwable throwable))
            return;

        if (!throwable.gravityEnabled)
            return;

        if (!contactCounts.TryGetValue(throwable, out int count))
            return;

        count--;

        if (debugLogs)
        {
            Debug.Log($"[Boundary] EXIT -> {throwable.name} | Contacts: {Mathf.Max(count, 0)}");
        }

        if (count <= 0)
        {
            contactCounts.Remove(throwable);

            if (runningTimers.TryGetValue(throwable, out Coroutine timer))
            {
                StopCoroutine(timer);
                runningTimers.Remove(throwable);

                if (debugLogs)
                {
                    Debug.Log($"[Boundary] {throwable.name} returned inside. Timer cancelled.");
                }
            }
        }
        else
        {
            contactCounts[throwable] = count;
        }
    }

    private IEnumerator DangerTimer(Throwable throwable)
    {
        if (debugLogs)
        {
            Debug.Log($"[Boundary] Waiting {graceDuration:F1}s for {throwable.name}");
        }

        yield return new WaitForSeconds(graceDuration);

        if (contactCounts.ContainsKey(throwable) && !triggered)
        {
            if (debugLogs)
            {
                Debug.Log($"[Boundary] GAME OVER -> {throwable.name} stayed outside for {graceDuration:F1}s");
            }

            triggered = true;
            OnBoundaryExceeded?.Invoke();
        }
    }

    public void Reset()
    {
        if (debugLogs)
        {
            Debug.Log("[Boundary] Reset");
        }

        triggered = false;

        foreach (var timer in runningTimers.Values)
            StopCoroutine(timer);

        runningTimers.Clear();
        contactCounts.Clear();
    }
    public void RemoveThrowable(Throwable throwable)
    {
        if (contactCounts.Remove(throwable))
        {
            if (runningTimers.TryGetValue(throwable, out Coroutine timer))
            {
                StopCoroutine(timer);
                runningTimers.Remove(throwable);
            }
        }
    }

#if UNITY_EDITOR
    private void OnDrawGizmos()
    {
        if (!drawGizmos)
            return;

        Gizmos.color = triggered ? Color.red : Color.green;

        Gizmos.DrawSphere(transform.position, 0.2f);

        foreach (Transform child in transform)
        {
            Gizmos.DrawLine(transform.position, child.position);
        }
    }
#endif
}