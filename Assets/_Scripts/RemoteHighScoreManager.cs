using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Networking;

public class RemoteHighScoreManager : MonoBehaviour
{
    public static RemoteHighScoreManager Instance { get; private set; }

    void Awake()
    {
        if (Instance == null)
        {
            Instance = this;
            DontDestroyOnLoad(gameObject);
        }
        else Destroy(gameObject);
    }

    public IEnumerator GetHighScores(
        string playerId,
        int level,
        Action<List<HighScoreRow>> onComplete,
        int pageSize = 10
    )
    {
        string where = UnityWebRequest.EscapeURL(
            $"playerId = '{playerId}' AND level = {level}"
        );

        string url = $"{BackendlessConfig.BASE_URL}/api/data/"
                   + $"{BackendlessConfig.TABLE_NAME}"
                   + $"?where={where}"
                   + $"&pageSize={pageSize}"
                   + $"&sortBy=score%20DESC";

        using var req = UnityWebRequest.Get(url);
        BackendlessConfig.ApplyAuthHeaders(req);
        yield return req.SendWebRequest();

        if (req.result != UnityWebRequest.Result.Success)
        {
            Debug.LogError($"[GET HS] {req.error}");
            onComplete?.Invoke(new List<HighScoreRow>());
        }
        else
        {
            var rows = JsonHelper.FromJson<HighScoreRow>(req.downloadHandler.text);
            onComplete?.Invoke(new List<HighScoreRow>(rows));
        }
    }

    public IEnumerator PostHighScore(HighScoreRow row, Action<bool> onComplete)
    {
        string url = $"{BackendlessConfig.BASE_URL}/api/data/{BackendlessConfig.TABLE_NAME}";
        string body = JsonUtility.ToJson(row);
        byte[] bytes = System.Text.Encoding.UTF8.GetBytes(body);

        using var req = new UnityWebRequest(url, "POST")
        {
            uploadHandler = new UploadHandlerRaw(bytes),
            downloadHandler = new DownloadHandlerBuffer()
        };
        BackendlessConfig.ApplyAuthHeaders(req);
        req.uploadHandler.contentType = "application/json";

        yield return req.SendWebRequest();
        onComplete?.Invoke(req.result == UnityWebRequest.Result.Success);
    }

    public IEnumerator DeleteHighScore(string objectId, Action<bool> onComplete)
    {
        string url = $"{BackendlessConfig.BASE_URL}/api/data/{BackendlessConfig.TABLE_NAME}/{objectId}";
        using var req = UnityWebRequest.Delete(url);
        BackendlessConfig.ApplyAuthHeaders(req);
        yield return req.SendWebRequest();

        bool success = req.result == UnityWebRequest.Result.Success;
        if (!success)
            Debug.LogError($"[DEL HS] failed to delete {objectId}: {req.error}");
        onComplete?.Invoke(success);
    }

    [Serializable]
    public class HighScoreRow
    {
        public string objectId;
        public string playerId;
        public int level;
        public int score;
    }

    static class JsonHelper
    {
        [Serializable]
        private class Wrapper<T> { public T[] Items; }

        public static T[] FromJson<T>(string json)
        {
            string wrapped = $"{{\"Items\":{json}}}";
            var w = JsonUtility.FromJson<Wrapper<T>>(wrapped);
            return w.Items;
        }
    }
}
