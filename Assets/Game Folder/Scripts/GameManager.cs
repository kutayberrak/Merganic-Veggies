using System;
using System.Collections.Generic;
using UnityEngine;

public class GameManager : MonoBehaviour
{
    public static GameManager Instance { get; private set; }

    [Header("References")]
    [SerializeField] private PlayerController playerController;

    public bool IsGameOver { get; private set; }
    public bool IsPaused { get; private set; }

    public static event Action OnGameOver;
    public static event Action OnWin;
    public static event Action OnPause;
    public static event Action OnResume;

    private void Awake()
    {
        if (Instance != null && Instance != this) { Destroy(gameObject); return; }
        Instance = this;
    }

    private void OnEnable()
    {
        BoundaryWatcher.OnBoundaryExceeded += TriggerLose;
    }

    private void OnDisable()
    {
        BoundaryWatcher.OnBoundaryExceeded -= TriggerLose;
    }

    private void TriggerLose()
    {
        if (IsGameOver) return;

        IsGameOver = true;
        OnGameOver?.Invoke();
        Pause();
    }

    public void TriggerWin()
    {
        if (IsGameOver) return;

        IsGameOver = true;
        OnWin?.Invoke();
        Pause();
    }

    public void Pause()
    {
        if (IsPaused) return;

        IsPaused = true;
        Time.timeScale = 0f;

        if (playerController != null)
            playerController.enabled = false;

        OnPause?.Invoke();
    }

    public void Resume()
    {
        if (!IsPaused) return;

        IsPaused = false;
        Time.timeScale = 1f;

        if (playerController != null)
            playerController.enabled = true;

        OnResume?.Invoke();
    }

    public void RestartGame()
    {
        ClearBoard();

        ScoreManager.Instance?.ResetScore();
        BoundaryWatcher.Instance?.Reset();

        IsGameOver = false;
        Resume();
    }

    private void ClearBoard()
    {
        List<Rigidbody> fruits = FruitRegistry.Instance.ActiveFruits;

        List<Rigidbody> snapshot = new List<Rigidbody>(fruits);

        for (int i = 0; i < snapshot.Count; i++)
        {
            Rigidbody rb = snapshot[i];
            if (rb == null) continue;

            if (rb.TryGetComponent<Throwable>(out var throwable))
                ObjectPoolManager.Instance.ReturnObject(throwable);
        }
    }
}