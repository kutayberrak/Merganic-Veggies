using System.Collections.Generic;
using UnityEngine;

public class ObjectPoolManager : MonoBehaviour
{
    public static ObjectPoolManager Instance { get; private set; }

    [Header("Pool Settings")]
    [SerializeField] private List<Throwable> throwablePrefabs;

    [SerializeField] private int poolSizePerPrefab = 15;

    private Transform pooledObjectsParent;

    private Dictionary<Throwable, Queue<Throwable>> pools =
        new Dictionary<Throwable, Queue<Throwable>>();

    private void Awake()
    {
        if (Instance != null && Instance != this)
        {
            Destroy(gameObject);
            return;
        }

        Instance = this;

        if (pooledObjectsParent == null)
        {
            GameObject root = new GameObject("PooledObjects");
            pooledObjectsParent = root.transform;
        }

        InitializePools();
    }

    void InitializePools()
    {
        foreach (Throwable prefab in throwablePrefabs)
        {
            Queue<Throwable> pool = new Queue<Throwable>();

            for (int i = 0; i < poolSizePerPrefab; i++)
            {
                Throwable obj = Instantiate(prefab, pooledObjectsParent);

                obj.gameObject.SetActive(false);

                pool.Enqueue(obj);
            }

            pools.Add(prefab, pool);
        }
    }
    public Throwable GetRandomObject(Vector3 position, Quaternion rotation)
    {
        if (throwablePrefabs.Count == 0)
        {
            Debug.LogError("No throwable prefabs found.");
            return null;
        }

        int randomIndex = Random.Range(0, throwablePrefabs.Count);

        Throwable selectedPrefab = throwablePrefabs[randomIndex];

        return GetObject(selectedPrefab, position, rotation);
    }

    public Throwable GetObject(Throwable prefab, Vector3 position, Quaternion rotation)
    {
        if (!pools.ContainsKey(prefab))
        {
            Debug.LogError($"Pool not found: {prefab.name}");
            return null;
        }

        Queue<Throwable> pool = pools[prefab];

        if (pool.Count == 0)
        {
            ExpandPool(prefab);
        }

        Throwable obj = pool.Dequeue();

        obj.transform.SetPositionAndRotation(position, rotation);

        obj.gameObject.SetActive(true);
        /*
        obj.ResetForSpawn();

        obj.SetOriginalPrefab(prefab);
        */
        return obj;
    }
    void ExpandPool(Throwable prefab)
    {
        Throwable obj = Instantiate(prefab);

        obj.transform.SetParent(pooledObjectsParent);

        obj.gameObject.SetActive(false);

        pools[prefab].Enqueue(obj);

        Debug.Log($"Expanded pool: {prefab.name}");
    }

    public void ReturnObject(Throwable obj)
    { /*
        obj.ResetForPool();

        obj.transform.SetParent(pooledObjectsParent);

        obj.gameObject.SetActive(false);

        pools[obj.OriginalPrefab].Enqueue(obj)*/
    }
}