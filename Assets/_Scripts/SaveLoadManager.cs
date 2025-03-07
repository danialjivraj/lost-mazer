using UnityEngine;
using System.IO;

public static class SaveLoadManager
{
    private static string saveFilePath = Application.persistentDataPath + "/savegame.json";

    public static void SaveGame(GameStateData data)
    {
        string json = JsonUtility.ToJson(data, true);
        File.WriteAllText(saveFilePath, json);
        Debug.Log("Game saved at: " + saveFilePath);
    }

    public static GameStateData LoadGame()
    {
        if (File.Exists(saveFilePath))
        {
            string json = File.ReadAllText(saveFilePath);
            GameStateData data = JsonUtility.FromJson<GameStateData>(json);
            return data;
        }
        else
        {
            //Debug.LogWarning("Save file not found!");
            return null;
        }
    }

    public static void DeleteSave()
    {
        if (File.Exists(saveFilePath))
        {
            File.Delete(saveFilePath);
            Debug.Log("Save file deleted.");
        }
    }

    public static bool SaveExists()
    {
        return File.Exists(saveFilePath);
    }
}
