using System.Collections.Generic;
using UnityEngine;

public class FruitRegistry : MonoBehaviour
{
    public static FruitRegistry Instance { get; private set; }

    private readonly List<Rigidbody> activeFruits = new List<Rigidbody>(70);
    public List<Rigidbody> ActiveFruits => activeFruits;

    void Awake()
    {
        if (Instance != null && Instance != this) { Destroy(gameObject); return; }
        Instance = this;
    }

    public void Register(Rigidbody rb)
    {
        if (!activeFruits.Contains(rb))
            activeFruits.Add(rb);
    }

    public void Unregister(Rigidbody rb)
    {
        activeFruits.Remove(rb);
    }
}