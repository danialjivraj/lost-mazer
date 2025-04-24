using UnityEngine;
using System.IO;
using System.Security.Cryptography;
using System.Text;
using System;

public static class SaveLoadManager
{
    private static string saveFilePath = Application.persistentDataPath + "/savegame.json";
    
    private static bool useEncryption = false; // did encryption because it looks cool, setting it to true encrypts the savegame.json
    
    private static string encryptionKey = "1234567890123456";

    public static void SaveGame(GameStateData data)
    {
        string json = JsonUtility.ToJson(data, true);

        if (useEncryption)
        {
            json = EncryptString(json, encryptionKey);
        }

        File.WriteAllText(saveFilePath, json);
        Debug.Log("Game saved at: " + saveFilePath);
    }

    public static GameStateData LoadGame()
    {
        if (File.Exists(saveFilePath))
        {
            string json = File.ReadAllText(saveFilePath);

            if (useEncryption)
            {
                json = DecryptString(json, encryptionKey);
            }

            GameStateData data = JsonUtility.FromJson<GameStateData>(json);
            return data;
        }
        else
        {
            return null;
        }
    }

    public static void DeleteSave()
    {
        if (File.Exists(saveFilePath))
        {
            File.Delete(saveFilePath);
            Debug.Log("Save file deleted");
        }
    }

    public static bool SaveExists()
    {
        return File.Exists(saveFilePath);
    }

    private static string EncryptString(string plainText, string key)
    {
        byte[] iv = new byte[16];
        byte[] encrypted;
        using (Aes aes = Aes.Create())
        {
            aes.Key = Encoding.UTF8.GetBytes(key);
            aes.IV = iv;
            ICryptoTransform encryptor = aes.CreateEncryptor(aes.Key, aes.IV);
            
            using (MemoryStream ms = new MemoryStream())
            {
                using (CryptoStream cs = new CryptoStream(ms, encryptor, CryptoStreamMode.Write))
                {
                    using (StreamWriter sw = new StreamWriter(cs))
                    {
                        sw.Write(plainText);
                    }
                    encrypted = ms.ToArray();
                }
            }
        }
        return Convert.ToBase64String(encrypted);
    }

    private static string DecryptString(string cipherText, string key)
    {
        byte[] iv = new byte[16];
        byte[] buffer = Convert.FromBase64String(cipherText);
        using (Aes aes = Aes.Create())
        {
            aes.Key = Encoding.UTF8.GetBytes(key);
            aes.IV = iv;
            ICryptoTransform decryptor = aes.CreateDecryptor(aes.Key, aes.IV);
            
            using (MemoryStream ms = new MemoryStream(buffer))
            {
                using (CryptoStream cs = new CryptoStream(ms, decryptor, CryptoStreamMode.Read))
                {
                    using (StreamReader sr = new StreamReader(cs))
                    {
                        return sr.ReadToEnd();
                    }
                }
            }
        }
    }
}
