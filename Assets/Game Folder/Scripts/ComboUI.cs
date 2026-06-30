using System;
using System.Collections.Generic;
using UnityEngine;
using TMPro;

public class ComboUI : MonoBehaviour
{
    [Serializable]
    public class ComboTier
    {
        public int minCombo = 1;
        public int maxCombo = 3;
        public string word = "Nice";
    }

    [Header("Combo Text")]
    [SerializeField] private GameObject comboRoot;
    [SerializeField] private TextMeshProUGUI comboText;
    [SerializeField] private float displayDuration = 1f;

    [Header("Praise Text")]
    [SerializeField] private GameObject praiseRoot;
    [SerializeField] private TextMeshProUGUI praiseText;
    [SerializeField] private float praiseDuration = 1f;
    [SerializeField]
    private List<ComboTier> tiers = new List<ComboTier>
    {
        new ComboTier { minCombo = 1, maxCombo = 3, word = "Nice" },
        new ComboTier { minCombo = 4, maxCombo = 7, word = "Awesome" },
        new ComboTier { minCombo = 8, maxCombo = 14, word = "Great" },
        new ComboTier { minCombo = 15, maxCombo = 1000, word = "Incredible" },
    };

    private float hideTimer;
    private bool comboVisible;
    private int lastComboCount;

    private float praiseTimer;
    private bool praiseVisible;

    private void OnEnable()
    {
        ScoreManager.OnComboChanged += UpdateCombo;
    }

    private void OnDisable()
    {
        ScoreManager.OnComboChanged -= UpdateCombo;
    }

    private void Start()
    {
        SetComboVisible(false);
        SetPraiseVisible(false);
    }

    private void Update()
    {
        if (comboVisible)
        {
            hideTimer -= Time.deltaTime;
            if (hideTimer <= 0f)
            {
                SetComboVisible(false);
                ShowPraise(lastComboCount);
            }
        }

        if (praiseVisible)
        {
            praiseTimer -= Time.deltaTime;
            if (praiseTimer <= 0f)
                SetPraiseVisible(false);
        }
    }

    private void UpdateCombo(int comboCount, float multiplier)
    {
        if (comboCount > 1)
        {
            lastComboCount = comboCount;
            comboText.text = $"x{comboCount}";
            SetComboVisible(true);
            hideTimer = displayDuration;
        }
        else
        {
            SetComboVisible(false);
        }
    }

    private void ShowPraise(int comboCount)
    {
        string word = GetWordForCombo(comboCount);
        if (string.IsNullOrEmpty(word)) return;

        praiseText.text = word.ToUpperInvariant();
        SetPraiseVisible(true);
        praiseTimer = praiseDuration;
    }

    private string GetWordForCombo(int comboCount)
    {
        for (int i = 0; i < tiers.Count; i++)
        {
            ComboTier tier = tiers[i];
            if (comboCount >= tier.minCombo && comboCount <= tier.maxCombo)
                return tier.word;
        }
        return null;
    }

    private void SetComboVisible(bool visible)
    {
        comboVisible = visible;
        if (comboRoot != null)
            comboRoot.SetActive(visible);
    }

    private void SetPraiseVisible(bool visible)
    {
        praiseVisible = visible;
        if (praiseRoot != null)
            praiseRoot.SetActive(visible);
    }
}