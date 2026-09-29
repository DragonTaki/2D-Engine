/* ----- ----- ----- ----- */
// LogFileManager.cs
// Do not distribute or modify
// Author: DragonTaki (https://github.com/DragonTaki)
// Create Date: 2025/05/06
// Update Date: 2025/05/06
// Version: v1.0
/* ----- ----- ----- ----- */

using System;
using System.IO;

namespace Engine.Logging
{
    public static class LogFileManager
    {
        private static readonly string LogPath = Path.Combine(AppDomain.CurrentDomain.BaseDirectory, "log.txt");
        private static readonly object _lock = new();

        public static void SaveLog(string message)
        {
            // Serialized: concurrent AppendAllText calls on the same file throw IOException.
            // A logging failure (disk full, read-only folder) must not crash the caller.
            try
            {
                lock (_lock)
                    File.AppendAllText(LogPath, $"{DateTime.Now:yyyy/MM/dd HH:mm:ss} > {message}{Environment.NewLine}");
            }
            catch (Exception ex) when (ex is IOException || ex is UnauthorizedAccessException)
            {
                Console.WriteLine($"[LogFileManager] Failed to write {LogPath}: {ex.Message}");
            }
        }
    }
}