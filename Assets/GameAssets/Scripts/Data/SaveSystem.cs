using UnityEngine;
using System.IO;

public static class SaveSystem
{
    private static string savePath = Path.Combine(Application.persistentDataPath, "gameData.json");

    // save
    public static void SaveGame(GameData data)
    {
        string json = JsonUtility.ToJson(data, true);
        File.WriteAllText(savePath, json);
        Debug.Log($"✅ Game saved: {savePath}");
    }

    // load
    public static GameData LoadGame()
    {
        if (File.Exists(savePath))
        {
            string json = File.ReadAllText(savePath);
            GameData data = JsonUtility.FromJson<GameData>(json);
            Debug.Log(" Game loaded successfully!");
            return data;
        }
        else
        {
            Debug.LogWarning(" No save file found, creating new game data.");
            return new GameData();
        }
    }

    public static bool SaveFileExists()
    {
        return File.Exists(savePath);
    }
}
