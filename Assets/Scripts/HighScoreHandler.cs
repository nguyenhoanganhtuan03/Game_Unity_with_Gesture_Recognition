using System;
using System.Collections.Generic;
using System.IO;
using UnityEngine;

public class HighScoreHandler : MonoBehaviour
{
    public static HighScoreHandler instance;

    public HighScoreElements highScores = new HighScoreElements();
    private String saveFile;

    private void Awake()
    {
        instance = this;
        saveFile = $"{Application.persistentDataPath}/highscores.json";
    }

    public delegate void OnHighScoreListChanged(HighScoreElements highScoreElements);
    public static event OnHighScoreListChanged onHighScoreListChanged;

    private void Start()
    {
        LoadHighScores();
    }

    // Start is called before the first frame update

    public void LoadHighScores()
    {
        if (File.Exists(saveFile))
        {
            string json = File.ReadAllText(saveFile);
            var highScoreNonSorted = JsonUtility.FromJson<HighScoreElements>(json);
            highScoreNonSorted.highScoresList.Sort((highScore1, highScore2) => highScore1.score.CompareTo(highScore2.score));
            highScoreNonSorted.highScoresList.Reverse();
            highScores = highScoreNonSorted;
        }

        if (onHighScoreListChanged != null)
        {
            onHighScoreListChanged.Invoke(highScores);
        }
    }

    public void SaveHighScore(HighScoreElement highScore)
    {
        SaveTemporaryHighScore(highScore.score);
        highScores.highScoresList.Add(highScore);
        var jsonList = JsonUtility.ToJson(highScores);
        File.WriteAllText(saveFile, jsonList);

        if (onHighScoreListChanged != null)
        {
            onHighScoreListChanged.Invoke(highScores);
        }
    }

    public void ResetScores()
    {
        if (File.Exists(saveFile))
        {
            File.Delete(saveFile);
        }

        if (onHighScoreListChanged != null)
        {
            onHighScoreListChanged.Invoke(highScores);
        }
    }

    public int GetTemporaryHighScore()
    {
        return PlayerPrefs.GetInt("highScore");
    }

    public void SaveTemporaryHighScore(int score)
    {
        PlayerPrefs.SetInt("highScore", score);
    }

    public void ResetTemporaryHighScore()
    {
        PlayerPrefs.DeleteKey("highScore");
    }
}

[Serializable]
public class HighScoreElements
{
    public List<HighScoreElement> highScoresList;
}

[Serializable]
public class HighScoreElement
{
    public string playerName;
    public int score;
    public LevelSelector levelSelection;
}
