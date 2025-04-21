using UnityEngine.Networking;

public static class BackendlessConfig
{
    public const string APP_ID = "70CF2EE3-D68E-4464-8689-9C5E7391BF1E";
    public const string REST_API_KEY = "126FC3D3-87F5-47C9-9C99-811BE01A1622";

    public const string BASE_URL = "https://zootychicken-us.backendless.app";

    public const string TABLE_NAME = "HighScores";

    public static void ApplyAuthHeaders(UnityWebRequest req)
    {
        req.SetRequestHeader("application-id", APP_ID);
        req.SetRequestHeader("secret-key", REST_API_KEY);
        req.SetRequestHeader("Content-Type", "application/json");
    }
}
