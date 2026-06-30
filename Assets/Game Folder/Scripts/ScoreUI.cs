using UnityEngine;
using TMPro;

public class ScoreUI : MonoBehaviour
{
    [SerializeField] private TextMeshProUGUI currentScoreText;

    private void OnEnable()
    {
        ScoreManager.OnScoreChanged += UpdateScoreText;
        UpdateScoreText(ScoreManager.Instance.CurrentScore);
    }

    private void OnDisable()
    {
        ScoreManager.OnScoreChanged -= UpdateScoreText;
    }

    private void Start()
    {
        if (ScoreManager.Instance != null)
            UpdateScoreText(ScoreManager.Instance.CurrentScore);
    }

    private void UpdateScoreText(int score)
    {
        currentScoreText.text = score.ToString();
    }
}