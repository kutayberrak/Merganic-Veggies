using System;
using UnityEngine;

public class Fruit : Throwable
{
    public static event Action<int> OnMerge;

    protected override void OnCollisionEnter(Collision collision)
    {
        if (hasMerged) return;

        base.OnCollisionEnter(collision);

        if (!collision.gameObject.TryGetComponent<Fruit>(out var other)) return;
        if (other.hasMerged) return;
        if (other.throwableData.tier != throwableData.tier) return;

        // Sadece büyük ID'li olan merge'i başlatır — çift tetiklenmeyi engeller
        if (GetInstanceID() < other.GetInstanceID()) return;

        hasMerged = true;
        other.hasMerged = true;

        Merge(other);
    }

    protected override void Merge(Throwable other)
    {
        int sourceTier = throwableData.tier;
        int nextTier = sourceTier + 1;
        Vector3 mergePosition = (rb.position + other.Position) / 2f;

        ObjectPoolManager.Instance.ReturnObject(this);
        ObjectPoolManager.Instance.ReturnObject(other);

        Throwable merged = ObjectPoolManager.Instance.GetByTier(nextTier, mergePosition, Quaternion.identity);
        merged?.ActivateAsLanded();

        OnMerge?.Invoke(sourceTier);
    }
}