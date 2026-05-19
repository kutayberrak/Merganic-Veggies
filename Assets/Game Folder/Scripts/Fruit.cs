using System;
using UnityEngine;

public class Fruit : Throwable
{
    public static event Action<int> OnMerge;

    protected override void OnCollisionEnter(Collision collision)
    {
        bool wasLanded = gravityEnabled;

        base.OnCollisionEnter(collision);

        if (!wasLanded) return;
        if (!collision.gameObject.TryGetComponent<Fruit>(out var other)) return;
        if (!other.gravityEnabled) return;
        if (other.throwableData.tier != throwableData.tier) return;

        // HigherID starts the merge
        if (GetInstanceID() < other.GetInstanceID()) return;

        Merge(other);
    }

    protected override void Merge(Throwable other)
    {
        int nextTier = throwableData.tier + 1;
        int mergeScore = throwableData.score;
        Vector3 mergePosition = (rb.position + other.Position) / 2f;

        ObjectPoolManager.Instance.ReturnObject(this);
        ObjectPoolManager.Instance.ReturnObject(other);

        Throwable merged = ObjectPoolManager.Instance.GetByTier(nextTier, mergePosition, Quaternion.identity);
        merged?.ActivateAsLanded();

        OnMerge?.Invoke(mergeScore);
    }
}