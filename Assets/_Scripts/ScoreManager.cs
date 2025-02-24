using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class ScoreManager : MonoBehaviour
{
    public static ScoreManager instance;
    public TMP_Text scoreText;
    int score = 0;
    List<int> highScores = new List<int>();

    void Awake(){
        instance = this;
        LoadHighScores();
    }

    void Start(){
        scoreText.text = score.ToString();
    }

    public void AddPoint(){
        score += 1;
        scoreText.text = score.ToString();
    }

    public void SaveScore(){
        highScores.Add(score);
        highScores.Sort((a, b) => b.CompareTo(a));

        if (highScores.Count > 10){
            highScores = highScores.GetRange(0, 10);
        }

        SaveHighScores();
    }

    void LoadHighScores(){
        highScores.Clear();
        for (int i = 0; i < 10; i++){
            int loadedScore = PlayerPrefs.GetInt("HighScore" + i, 0);
            if (loadedScore > 0){
                highScores.Add(loadedScore);
            }
        }
    }

    void SaveHighScores(){
        for (int i = 0; i < highScores.Count; i++){
            PlayerPrefs.SetInt("HighScore" + i, highScores[i]);
        }
        PlayerPrefs.Save();
    }

    public void LogScores(){
        Debug.Log("=== Top 10 Scores ===");
        for (int i = 0; i < 10; i++){
            int score = PlayerPrefs.GetInt("HighScore" + i, 0);
            Debug.Log("Score " + (i + 1) + ": " + score);
        }
    }
}