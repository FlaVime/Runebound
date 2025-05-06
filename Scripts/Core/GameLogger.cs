using System;
using System.IO;
using UnityEngine;

public static class GameLogger
{
    private static readonly string logFilePath = Path.Combine(Application.persistentDataPath, "game_log.txt");

    public static void Log(string message)
    {
        string timestamp = DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss");
        string fullMessage = $"[{timestamp}] {message}";

        try
        {
            File.AppendAllText(logFilePath, fullMessage + Environment.NewLine);
        }
        catch (Exception e)
        {
            Debug.LogWarning($"[GameLogger] Failed to write log: {e.Message}");
        }

        Debug.Log(fullMessage);
    }
}
