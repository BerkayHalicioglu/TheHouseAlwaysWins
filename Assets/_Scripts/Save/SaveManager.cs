using System.IO;
using UnityEngine;

public static class SaveManager
{
    private static string SavePath =>
        Path.Combine(Application.persistentDataPath, "savegame.json");

    public static bool HasSave() => File.Exists(SavePath);

    public static void Save(SaveData data)
    {
        File.WriteAllText(SavePath, JsonUtility.ToJson(data, true));
        Debug.Log($"[SaveManager] Saved — Day {data.currentDay}, Bankroll ${data.currentBankroll:N0}");
    }

    public static SaveData Load()
    {
        if (!HasSave()) return null;
        SaveData data = JsonUtility.FromJson<SaveData>(File.ReadAllText(SavePath));
        Debug.Log($"[SaveManager] Loaded — Day {data.currentDay}, Bankroll ${data.currentBankroll:N0}");
        return data;
    }

    public static void DeleteSave()
    {
        if (!File.Exists(SavePath)) return;
        File.Delete(SavePath);
        Debug.Log("[SaveManager] Save deleted.");
    }
}
