using UnityEngine.Networking;

public static class BackendlessConfig
{
    public const string BASE_URL = "https://";
    public const string APP_ID = "";
    public const string REST_API_KEY = "";

    public const string TABLE_NAME = "HighScores";

    public static void ApplyAuthHeaders(UnityWebRequest req)
    {
        req.SetRequestHeader("application-id", APP_ID);
        req.SetRequestHeader("secret-key", REST_API_KEY);
        req.SetRequestHeader("Content-Type", "application/json");
    }
}
