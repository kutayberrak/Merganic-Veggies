using UnityEngine;
using TMPro;

public class LosePageUI : MonoBehaviour
{
    [SerializeField] private TextMeshProUGUI finalScoreText;

    private void OnEnable()
    {
        if (ScoreManager.Instance != null && finalScoreText != null)
            finalScoreText.text = ScoreManager.Instance.CurrentScore.ToString();
    }
}