using UnityEngine;
using UnityEngine.SceneManagement;

public static class LevelManager
{
    private static string lastLevel;

    public static void SetLastLevel(string levelName)
    {
        lastLevel = levelName;
    }

    public static string GetLastLevel()
    {
        return lastLevel;
    }
}