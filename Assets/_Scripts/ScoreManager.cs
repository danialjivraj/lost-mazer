using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;
using BackendlessAPI;
using BackendlessAPI.Persistence;
using System.Threading.Tasks;
using System;
using System.Linq;

public class ScoreManager : MonoBehaviour
{
    public static ScoreManager instance;
    public TMP_Text scoreText;

    private int score = 0;

    private List<Dictionary<string, object>> levelHighScoreRows = new List<Dictionary<string, object>>(); // stores the entire row data for the current level's high scores

    private string playerId;

    void Awake()
    {
        instance = this;
        playerId = PlayerIdManager.instance.GetPlayerId();
    }

    void Start()
    {
        scoreText.text = score.ToString();
    }

    public void AddPoint()
    {
        score += 1;
        scoreText.text = score.ToString();
    }

    public async void SaveScore()
    {
        int currentLevel = PlayerPrefs.GetInt("CurrentLevel", 1);

        await LoadHighScoresForLevel(currentLevel);

        SortScoreRowsDescending();

        bool shouldSave = false;
        if (levelHighScoreRows.Count < 10)
        {
            // fewer than 10 rows, so it should save
            shouldSave = true;
        }
        else
        {
            // this compares the score with the 10th best (index 9)
            int tenthBestScore = GetScoreFromRow(levelHighScoreRows[9]);
            if (score > tenthBestScore)
                shouldSave = true;
        }

        if (!shouldSave)
        {
            Debug.Log("Score did not make it into the top 10. Not saving to database.");
            return;
        }

        // saves the new score first
        await SaveCurrentScore(score, currentLevel);
        Debug.Log("New score inserted. Now we trim to top 10 if needed...");

        // re-query's the DB for top 11
        await LoadHighScoresForLevel(currentLevel);
        SortScoreRowsDescending();

        if (levelHighScoreRows.Count > 10)
        {
            // and then, identifies the single lowest row (the 11th in sorted order)
            var lowestRow = levelHighScoreRows[10];
            await DeleteScoreRow(lowestRow);
            levelHighScoreRows.RemoveAt(10);
        }
    }

    private void SortScoreRowsDescending()
    {
        levelHighScoreRows.Sort((rowA, rowB) =>
        {
            int scoreA = GetScoreFromRow(rowA);
            int scoreB = GetScoreFromRow(rowB);
            return scoreB.CompareTo(scoreA);
        });
    }

    private int GetScoreFromRow(Dictionary<string, object> row)
    {
        if (row.TryGetValue("score", out object value))
            return Convert.ToInt32(value);
        return 0;
    }

    private async Task DeleteScoreRow(Dictionary<string, object> row)
    {
        try
        {
            var dataStore = Backendless.Data.Of("HighScores");
            await dataStore.RemoveAsync(row);

            Debug.Log($"Deleted row with objectId {row["objectId"]} and score {row["score"]} for player {playerId}.");
        }
        catch (Exception ex)
        {
            Debug.LogError($"Error deleting score: {ex.Message}");
        }
    }

    private async Task SaveCurrentScore(int newScore, int currentLevel)
    {
        var dataStore = Backendless.Data.Of("HighScores");

        var highScoreDict = new Dictionary<string, object>
        {
            { "score", newScore },
            { "playerId", playerId },
            { "level", currentLevel }
        };

        await dataStore.SaveAsync(highScoreDict);
        Debug.Log($"Score {newScore} for level {currentLevel} saved successfully for player {playerId}.");
    }

    private async Task LoadHighScoresForLevel(int selectedLevel)
    {
        levelHighScoreRows.Clear();

        var dataStore = Backendless.Data.Of("HighScores");
        var query = DataQueryBuilder.Create();
        query.SetWhereClause($"playerId = '{playerId}' AND level = {selectedLevel}");
        query.SetPageSize(100).SetSortBy(new List<string> { "score DESC" });

        var result = await dataStore.FindAsync(query);

        foreach (var row in result)
        {
            levelHighScoreRows.Add(row);
        }

        Debug.Log($"Loaded {levelHighScoreRows.Count} high scores for player {playerId}, level {selectedLevel}.");
    }
}