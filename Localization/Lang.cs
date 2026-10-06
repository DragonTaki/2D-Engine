/* ----- ----- ----- ----- */
// Lang.cs
// Do not distribute or modify
// Author: DragonTaki (https://github.com/DragonTaki)
// Create Date: 2026/10/06
// Update Date: 2026/10/06
// Version: v1.0
/* ----- ----- ----- ----- */

using System;
using System.Collections.Generic;
using System.Globalization;
using System.IO;
using System.Linq;
using System.Text.Json;

using Engine.Logging;

namespace Engine.Localization
{
    /// <summary>
    /// The engine's localization: language files (<c>&lt;code&gt;.json</c>, e.g. <c>zh-TW.json</c>) of
    /// flat key → text pairs, keys dotted by area (<c>game.over.win</c>; like Minecraft's 1.13+ files),
    /// read with <c>System.Text.Json</c> allowing <c>//</c> comments and trailing commas. Texts take
    /// numbered parameters <c>{0}</c>, <c>{1}</c> (each language may reorder them); a list of any
    /// length takes one parameter, joined with the language's <c>common.list_separator</c>
    /// (<see cref="JoinList"/>). A key missing from the current language falls back to
    /// <see cref="FallbackLanguage"/>, then to the key itself, with one WARN per key. The logic layer
    /// builds every on-screen sentence with it; the display layer only shows the strings it gets.
    /// </summary>
    public static class Lang
    {
        /// <summary>The language every text exists in; the last stop before the key itself.</summary>
        public const string FallbackLanguage = "zh-TW";

        /// <summary>The key of the separator a language joins lists with (Chinese 、).</summary>
        public const string ListSeparatorKey = "common.list_separator";

        private static readonly JsonSerializerOptions JsonOptions = new()
        {
            ReadCommentHandling = JsonCommentHandling.Skip,
            AllowTrailingCommas = true,
        };

        private static readonly object Sync = new();
        private static IReadOnlyDictionary<string, string> _current = new Dictionary<string, string>();
        private static IReadOnlyDictionary<string, string> _fallback = new Dictionary<string, string>();
        private static readonly HashSet<string> _warned = new();

        /// <summary>The language in use (its code, e.g. <c>zh-TW</c>).</summary>
        public static string CurrentLanguage { get; private set; } = FallbackLanguage;

        /// <summary>The folder the language files were loaded from (null before <see cref="Load"/>).</summary>
        public static string Folder { get; private set; }

        /// <summary>
        /// Loads <paramref name="language"/> and <see cref="FallbackLanguage"/> from
        /// <paramref name="folder"/>. A missing or unreadable file is logged (ERROR for the
        /// fallback, WARN for another language) and counts as empty, so texts fall back.
        /// </summary>
        /// <param name="folder">The language files' folder (e.g. <c>Assets/Lang</c>).</param>
        /// <param name="language">The language to use; null or empty for <see cref="FallbackLanguage"/>.</param>
        public static void Load(string folder, string language = null)
        {
            ArgumentNullException.ThrowIfNull(folder);
            language = string.IsNullOrWhiteSpace(language) ? FallbackLanguage : language;
            var fallback = ReadFile(folder, FallbackLanguage, LogLevel.ERROR);
            var current = language == FallbackLanguage ? fallback : ReadFile(folder, language, LogLevel.WARN);
            lock (Sync)
            {
                Folder = folder;
                CurrentLanguage = language;
                _fallback = fallback;
                _current = current;
                _warned.Clear();
            }
        }

        /// <summary>
        /// Uses the given texts instead of files (tests): <paramref name="current"/> as the language in use,
        /// <paramref name="fallback"/> (default: the same) as the fallback.
        /// </summary>
        public static void Use(string language, IReadOnlyDictionary<string, string> current, IReadOnlyDictionary<string, string> fallback = null)
        {
            ArgumentNullException.ThrowIfNull(current);
            lock (Sync)
            {
                CurrentLanguage = language ?? FallbackLanguage;
                _current = current;
                _fallback = fallback ?? current;
                _warned.Clear();
            }
        }

        /// <summary>Whether <paramref name="key"/> has a text in the current or the fallback language.</summary>
        public static bool Has(string key) => _current.ContainsKey(key) || _fallback.ContainsKey(key);

        /// <summary>
        /// The text of <paramref name="key"/> with <paramref name="args"/> put in its <c>{0}</c>,
        /// <c>{1}</c>... (invariant formatting); the key itself when no language has it.
        /// </summary>
        public static string Get(string key, params object[] args)
        {
            string text = Lookup(key);
            if (args == null || args.Length == 0)
                return text;
            try
            {
                return string.Format(CultureInfo.InvariantCulture, text, args);
            }
            catch (FormatException)
            {
                WarnOnce(key, $"(Lang) Text of '{key}' does not fit its {args.Length} parameter(s): \"{text}\"");
                return text;
            }
        }

        /// <summary>The items joined with the current language's <see cref="ListSeparatorKey"/>.</summary>
        public static string JoinList(IEnumerable<string> items) =>
            string.Join(Lookup(ListSeparatorKey), items ?? Enumerable.Empty<string>());

        private static string Lookup(string key)
        {
            if (key == null)
                return string.Empty;
            if (_current.TryGetValue(key, out var text) || _fallback.TryGetValue(key, out text))
                return text;
            WarnOnce(key, $"(Lang) Missing text: {key} ({CurrentLanguage})");
            return key;
        }

        private static void WarnOnce(string key, string message)
        {
            lock (Sync)
            {
                if (!_warned.Add(key))
                    return;
            }
            AppLogger.Log(message, LogLevel.WARN);
        }

        private static IReadOnlyDictionary<string, string> ReadFile(string folder, string language, LogLevel missingLevel)
        {
            string path = Path.Combine(folder, language + ".json");
            try
            {
                using var stream = File.OpenRead(path);
                return JsonSerializer.Deserialize<Dictionary<string, string>>(stream, JsonOptions)
                    ?? new Dictionary<string, string>();
            }
            catch (Exception ex) when (ex is IOException || ex is UnauthorizedAccessException || ex is JsonException)
            {
                AppLogger.Log($"(Lang) Cannot read the language file {path}: {ex.Message}", missingLevel);
                return new Dictionary<string, string>();
            }
        }
    }
}
