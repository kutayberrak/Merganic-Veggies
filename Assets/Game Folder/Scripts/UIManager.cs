using UnityEngine;

public class UIManager : MonoBehaviour
{
    public static UIManager Instance { get; private set; }

    [Header("Pages")]
    [SerializeField] private GameObject gameplayPage;
    [SerializeField] private GameObject winPage;
    [SerializeField] private GameObject losePage;

    private void Awake()
    {
        if (Instance != null && Instance != this) { Destroy(gameObject); return; }
        Instance = this;
    }

    private void OnEnable()
    {
        GameManager.OnGameOver += ShowLose;
        GameManager.OnWin += ShowWin;
    }

    private void OnDisable()
    {
        GameManager.OnGameOver -= ShowLose;
        GameManager.OnWin -= ShowWin;
    }

    private void Start()
    {
        ShowGameplay();
    }

    public void ShowGameplay()
    {
        SetActiveSafe(gameplayPage, true);
        SetActiveSafe(winPage, false);
        SetActiveSafe(losePage, false);
    }

    public void ShowWin()
    {
        SetActiveSafe(gameplayPage, false);
        SetActiveSafe(winPage, true);
        SetActiveSafe(losePage, false);
    }

    public void ShowLose()
    {
        SetActiveSafe(gameplayPage, false);
        SetActiveSafe(winPage, false);
        SetActiveSafe(losePage, true);
    }

    public void RestartAndShowGameplay()
    {
        GameManager.Instance?.RestartGame();
        ShowGameplay();
    }

    private void SetActiveSafe(GameObject obj, bool active)
    {
        if (obj != null) obj.SetActive(active);
    }
}