using UnityEngine;

public static class SaveManager
{
    private const string ScoreKey = "PlayerScore";


    // --- Score ---
    public static void SaveScore(int score)
    {
        PlayerPrefs.SetInt(ScoreKey, score);
        PlayerPrefs.Save();
    }

    public static int GetScore()
    {
        return PlayerPrefs.GetInt(ScoreKey, 0);
    }


    // Test/debug 
    public static void ClearAll()
    {
        PlayerPrefs.DeleteAll();
    }
}