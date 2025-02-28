using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;
using BackendlessAPI;
using BackendlessAPI.Persistence;
using System.Threading.Tasks;
using System;

public class ScoreManager : MonoBehaviour
{
    public static ScoreManager instance;
    public TMP_Text scoreText;
    int score = 0;
    List<int> highScores = new List<int>();

    private string playerId;

    void Awake()
    {
        instance = this;

        //playerId = PlayerIdManager.instance.GetPlayerId(); // use this if you want to create the game object in the levels and drag the playerid script onto it

        LoadHighScores().ContinueWith(task =>
        {
            if (task.IsFaulted)
            {
                Debug.LogError("Error loading high scores: " + task.Exception);
            }
        });
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
        await LoadHighScores();

        highScores.Add(score);

        highScores.Sort((a, b) => b.CompareTo(a));

        // keeps only the top 10 scores
        if (highScores.Count > 10)
        {
            // removes the lowest score from the database
            int lowestScore = highScores[highScores.Count - 1];
            await DeleteScore(lowestScore);

            highScores = highScores.GetRange(0, 10);
        }

        if (highScores.Contains(score))
        {
            await SaveCurrentScore(score);
        }
        else
        {
            Debug.Log("Score did not make it into the top 10. Not saving to database.");
        }
    }

    async Task DeleteScore(int scoreToDelete)
    {
        try
        {
            var dataStore = Backendless.Data.Of("HighScores");

            var query = DataQueryBuilder.Create();
            query.SetWhereClause($"playerId = '{playerId}' AND score = {scoreToDelete}");

            var deleteResult = await dataStore.RemoveAsync(query.GetWhereClause());
            Debug.Log($"Deleted score {scoreToDelete} for player {playerId}. Result: {deleteResult}");
        }
        catch (System.Exception ex)
        {
            Debug.LogError($"Error deleting score: {ex.Message}");
        }
    }

    async Task SaveCurrentScore(int newScore)
    {
        var dataStore = Backendless.Data.Of("HighScores");

        // saves the new score with the player ID
        var highScoreDict = new Dictionary<string, object>
        {
            { "score", newScore },
            { "playerId", playerId }
        };

        await dataStore.SaveAsync(highScoreDict);
        Debug.Log($"Score {newScore} saved successfully for player {playerId}.");
    }

    async Task LoadHighScores()
    {
        highScores.Clear();

        var dataStore = Backendless.Data.Of("HighScores");
        var query = DataQueryBuilder.Create();
        query.SetWhereClause($"playerId = '{playerId}'");
        query.SetPageSize(10).SetSortBy(new List<string> { "score DESC" });

        var result = await dataStore.FindAsync(query);

        foreach (var item in result)
        {
            if (item.ContainsKey("score"))
            {
                highScores.Add(Convert.ToInt32(item["score"]));
            }
        }

        Debug.Log($"Top 10 high scores loaded successfully for player {playerId}.");
    }

    public void LogScores()
    {
        Debug.Log("=== Top 10 Scores ===");
        for (int i = 0; i < highScores.Count; i++)
        {
            Debug.Log("Score " + (i + 1) + ": " + highScores[i]);
        }
    }
}

public class HighScore
{
    public string objectId { get; set; }
    public int score { get; set; }
    public string playerId { get; set; }
}