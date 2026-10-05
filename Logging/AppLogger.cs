/* ----- ----- ----- ----- */
// AppLogger.cs
// Do not distribute or modify
// Author: DragonTaki (https://github.com/DragonTaki)
// Create Date: 2025/05/06
// Update Date: 2026/10/05
// Version: v1.0
/* ----- ----- ----- ----- */

using System;
using System.Collections.Generic;
using System.Text.Json;

using Engine.Diagnostics;

namespace Engine.Logging
{
    public enum LogLevel
    {
        INIT,
        DEBUG,
        INFO,
        WARN,
        ERROR
    }

    public class LogLevelMeta
    {
        public string Label { get; set; }
        public string Color { get; set; }

        // Read-only: a public mutable static field could be reassigned or have entries
        // removed by any caller, breaking every lookup below.
        public static readonly IReadOnlyDictionary<LogLevel, LogLevelMeta> MetaMap = new Dictionary<LogLevel, LogLevelMeta>
        {
            { LogLevel.INIT,  new LogLevelMeta { Label = "INIT",  Color = null } },
            { LogLevel.DEBUG, new LogLevelMeta { Label = "DEBUG", Color = "gray" } },
            { LogLevel.INFO,  new LogLevelMeta { Label = "INFO",  Color = "white" } },
            { LogLevel.WARN,  new LogLevelMeta { Label = "WARN",  Color = "yellow" } },
            { LogLevel.ERROR, new LogLevelMeta { Label = "ERROR", Color = "red" } }
        };
    }

    public class LogRecord
    {
        public string Message { get; }
        public LogLevel Level { get; }
        public string Timestamp { get; }

        public LogRecord(string message, LogLevel level)
        {
            Message = message;
            Level = level;
            Timestamp = DateTime.Now.ToString("HH:mm:ss");
        }

        public string ToText()
        {
            var meta = LogLevelMeta.MetaMap[Level];
            return $"{Timestamp} [{meta.Label}] {Message}";
        }

        /// <summary>
        /// Serializes this record for the external logger as an array of text fragments -
        /// the same shape <see cref="AppLogger.LogWelcomeMessage"/> sends - so the
        /// receiver handles one message format (this is simply a one-fragment message).
        /// </summary>
        public string ToJson()
        {
            var meta = LogLevelMeta.MetaMap[Level];
            var payload = new[]
            {
                new
                {
                    text = ToText(),
                    color = meta.Color ?? "white",
                    tag = $"tag_{meta.Label.ToLowerInvariant()}"
                }
            };
            return JsonSerializer.Serialize(payload);
        }
    }

    public static class AppLogger
    {
#nullable enable
        private static Action<string>? _externalLogger = null;
#nullable disable
        // Pushed in by the app's composition root (Launcher/Program.cs) at
        // startup, and by the settings screen on change, from the player
        // settings — Engine must not read Game's config directly. (Whether
        // DEBUG lines are written is the DebugOptions.VerboseLog switch,
        // pushed in the same way.)
        public static string CurrentUser { get; set; } = string.Empty;

        public static void SetExternalLogger(Action<string> callback)
        {
            _externalLogger = callback;
        }

        public static void Log(string message, LogLevel level = LogLevel.INFO)
        {
            if (level == LogLevel.DEBUG && !DebugOptions.VerboseLog)
                return;

            var record = new LogRecord(message, level);
            Console.WriteLine(record.ToText());

            if (_externalLogger != null)
            {
                try
                {
                    _externalLogger(record.ToJson());
                }
                catch (Exception ex)
                {
                    Console.WriteLine($"[LOGGER ERROR] Failed to send to external logger: {ex.Message}");
                }
            }
        }

        public static void LogWelcomeMessage()
        {
            if (_externalLogger == null)
                return;

            var rainbowColors = new[]
            {
                "#FFB3BA", "#FFDFBA", "#FFFFBA", "#BAFFC9",
                "#BAE1FF", "#D5BAFF", "#FFBAED"
            };

            string greeting = string.IsNullOrEmpty(CurrentUser)
                ? "Hello!"
                : $"Hello, {CurrentUser}!";

            string welcome = "Chinese Chess v3.0";
            string author = "Author: DragonTaki";

            var greetingJson = JsonSerializer.Serialize(new[]
            {
                new {
                    text = greeting,
                    color = "lightgreen",
                    bold = true,
                    tag = "greeting_tag"
                }
            });
            var welcomeList = new List<object>();
            for (int i = 0; i < welcome.Length; i++)
            {
                welcomeList.Add(new
                {
                    text = welcome[i].ToString(),
                    color = rainbowColors[i % rainbowColors.Length],
                    bold = true,
                    tag = $"rainbow_{i}"
                });
            }
            var welcomeJson = JsonSerializer.Serialize(welcomeList);
            var authorJson = JsonSerializer.Serialize(new[]
            {
                new {
                    text = author,
                    color = "cyan",
                    italic = true,
                    tag = "author_tag"
                }
            });

            try
            {
                _externalLogger(greetingJson);
                _externalLogger(welcomeJson);
                _externalLogger(authorJson);
            }
            catch (Exception ex)
            {
                Console.WriteLine($"[WELCOME ERROR] Failed to send welcome message: {ex.Message}");
            }
        }
    }
}