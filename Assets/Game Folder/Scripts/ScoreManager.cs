using System;
using UnityEngine;

public class ScoreManager : MonoBehaviour
{
    public static ScoreManager Instance { get; private set; }

    [Header("Combo Settings")]
    [SerializeField] private float comboWindow = 1f;

    public int CurrentScore { get; private set; }
    public int ComboCount { get; private set; }
    public float ComboMultiplier { get; private set; } = 1f;

    // UI bağlamak için — toplam skor ve combo değişince tetiklenir
    public static event Action<int> OnScoreChanged;
    public static event Action<int, float> OnComboChanged;

    private float lastMergeTime = -999f;

    private void Awake()
    {
        if (Instance != null && Instance != this) { Destroy(gameObject); return; }
        Instance = this;
    }

    private void OnEnable()
    {
        Fruit.OnMerge += HandleMerge;
    }

    private void OnDisable()
    {
        Fruit.OnMerge -= HandleMerge;
    }

    private void Start()
    {
        CurrentScore = SaveManager.GetScore();
        OnScoreChanged?.Invoke(CurrentScore);
    }

    private void HandleMerge(int tier)
    {
        bool withinComboWindow = (Time.time - lastMergeTime) <= comboWindow;

        if (withinComboWindow)
        {
            ComboCount++;
            ComboMultiplier += 0.5f;
        }
        else
        {
            ComboCount = 1;
            ComboMultiplier = 1f;
        }

        lastMergeTime = Time.time;

        // base = tier * 2 + 2  →  example tier5(32) = 12, tier6(64) = 14
        int baseScore = tier * 2 + 2;
        int finalScore = Mathf.RoundToInt(baseScore * ComboMultiplier);

        CurrentScore += finalScore;

        SaveManager.SaveScore(CurrentScore);

        OnScoreChanged?.Invoke(CurrentScore);
        OnComboChanged?.Invoke(ComboCount, ComboMultiplier);
    }
}