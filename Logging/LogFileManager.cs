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

        public static void SaveLog(string message)
        {
            File.AppendAllText(LogPath, $"{DateTime.Now:yyyy/MM/dd HH:mm:ss} > {message}\n");
        }
    }
}