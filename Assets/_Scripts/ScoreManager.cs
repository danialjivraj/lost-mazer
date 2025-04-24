using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;

public class ScoreManager : MonoBehaviour
{
    public static ScoreManager instance;
    public TMP_Text scoreText;

    private int score = 0;
    public  int CurrentScore => score;

    private string playerId;

    void Awake()
    {
        instance = this;
        playerId = PlayerIdManager.instance.GetPlayerId();
    }

    void Start()
    {
        var data = SaveLoadManager.LoadGame();
        if (data != null) score = data.score;
        UpdateScoreUI();
    }

    void UpdateScoreUI()
    {
        scoreText.text = score.ToString();
    }

    public void AddPoint()
    {
        score++;
        UpdateScoreUI();
    }

    public void SaveScore()
    {
        int currentLevel = PlayerPrefs.GetInt("CurrentLevel", 1);
        StartCoroutine( UploadTop10Routine(currentLevel) );
    }

    private IEnumerator UploadTop10Routine(int level)
    {
        List<RemoteHighScoreManager.HighScoreRow> rows = null;
        yield return StartCoroutine(
            RemoteHighScoreManager.Instance.GetHighScores(
                playerId, level,
                list => rows = list
            )
        );

        rows.Sort((a,b) => b.score.CompareTo(a.score));

        bool qualifies = rows.Count < 10 || score > rows[rows.Count - 1].score;
        if (!qualifies)
        {
            Debug.Log("Did not beat top‑10, skipping it");
            yield break;
        }

        Debug.Log($"Score {score} qualifies for top‑10 on level {level}");

        if (rows.Count >= 10)
        {
            string toDelete = rows[rows.Count - 1].objectId;
            bool delOk = false;
            yield return StartCoroutine(
                RemoteHighScoreManager.Instance.DeleteHighScore(
                    toDelete, ok => delOk = ok
                )
            );
            if (delOk)
                Debug.Log($"Deleted old low‐score row {toDelete}");
            else
                Debug.LogError($"Failed to delete old low‐score row {toDelete}");
        }

        var newRow = new RemoteHighScoreManager.HighScoreRow {
            playerId = playerId,
            level = level,
            score = score
        };

        bool postOk = false;
        yield return StartCoroutine(
            RemoteHighScoreManager.Instance.PostHighScore(
                newRow, ok => postOk = ok
            )
        );
        if (postOk)
            Debug.Log("Posted new high score successfully");
        else
            Debug.LogError("Failed to POST new high score");
    }
}
