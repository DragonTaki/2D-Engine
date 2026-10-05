/* ----- ----- ----- ----- */
// SettingsKey.cs
// Do not distribute or modify
// Author: DragonTaki (https://github.com/DragonTaki)
// Create Date: 2026/10/05
// Update Date: 2026/10/05
// Version: v1.0
/* ----- ----- ----- ----- */

using System;
using System.Collections.Generic;
using System.Globalization;
using System.IO;
using System.Linq;

namespace Engine.Configs
{
    /// <summary>
    /// One key of the settings file (<see cref="SettingsFile"/>), bound to the value it stands
    /// for in a settings area (<see cref="ISettingsArea"/>): where it lives (section and name),
    /// its comment, how the value is written, and how a text from the file is checked and taken.
    /// Built with the typed factories (<see cref="Bool"/>, <see cref="Int"/>...), which also keep
    /// the key's default.
    /// </summary>
    public sealed class SettingsKey
    {
        private static readonly string[] TrueWords = { "true", "yes", "on", "1" };
        private static readonly string[] FalseWords = { "false", "no", "off", "0" };

        private readonly Func<string> _format;
        private readonly Func<string, bool> _tryApply;

        private SettingsKey(string section, string name, string[] comment, Func<string> format, Func<string, bool> tryApply, string defaultText)
        {
            Section = section ?? throw new ArgumentNullException(nameof(section));
            Name = name ?? throw new ArgumentNullException(nameof(name));
            Comment = comment ?? Array.Empty<string>();
            _format = format;
            _tryApply = tryApply;
            DefaultText = defaultText;
        }

        /// <summary>The key's section (e.g. <c>debug</c>).</summary>
        public string Section { get; }

        /// <summary>The key's name in its section.</summary>
        public string Name { get; }

        /// <summary>The comment lines written above the key (the default is added by <see cref="SettingsFile"/>).</summary>
        public IReadOnlyList<string> Comment { get; }

        /// <summary>The default value, as written in the file.</summary>
        public string DefaultText { get; }

        /// <summary>The current value, as written in the file.</summary>
        public string Format() => _format();

        /// <summary>Checks <paramref name="text"/> (a value from the file) and, when valid, sets the value.</summary>
        /// <returns>Whether the text was valid (and taken).</returns>
        public bool TryApply(string text) => _tryApply(text ?? string.Empty);

        /// <summary>Sets the value back to <see cref="DefaultText"/>.</summary>
        public void ResetToDefault() => _tryApply(DefaultText);

        #region Factories

        /// <summary>An on/off key (<c>true</c>/<c>false</c>; also yes/no, on/off, 1/0 when read).</summary>
        public static SettingsKey Bool(string section, string name, Func<bool> get, Action<bool> set, bool defaultValue, params string[] comment)
        {
            static string Text(bool v) => v ? "true" : "false";
            return new SettingsKey(section, name, comment, () => Text(get()), text =>
            {
                if (TrueWords.Contains(text, StringComparer.OrdinalIgnoreCase)) { set(true); return true; }
                if (FalseWords.Contains(text, StringComparer.OrdinalIgnoreCase)) { set(false); return true; }
                return false;
            }, Text(defaultValue));
        }

        /// <summary>An integer key in [<paramref name="min"/>, <paramref name="max"/>].</summary>
        public static SettingsKey Int(string section, string name, Func<int> get, Action<int> set, int defaultValue, int min, int max, params string[] comment)
        {
            static string Text(int v) => v.ToString(CultureInfo.InvariantCulture);
            return new SettingsKey(section, name, comment, () => Text(get()), text =>
            {
                if (!int.TryParse(text, NumberStyles.Integer, CultureInfo.InvariantCulture, out int v) || v < min || v > max)
                    return false;
                set(v);
                return true;
            }, Text(defaultValue));
        }

        /// <summary>An integer key that must be one of <paramref name="allowed"/>.</summary>
        public static SettingsKey IntOneOf(string section, string name, Func<int> get, Action<int> set, int defaultValue, IReadOnlyList<int> allowed, params string[] comment)
        {
            static string Text(int v) => v.ToString(CultureInfo.InvariantCulture);
            return new SettingsKey(section, name, comment, () => Text(get()), text =>
            {
                if (!int.TryParse(text, NumberStyles.Integer, CultureInfo.InvariantCulture, out int v) || !allowed.Contains(v))
                    return false;
                set(v);
                return true;
            }, Text(defaultValue));
        }

        /// <summary>A number key in [<paramref name="min"/>, <paramref name="max"/>] (written with up to 3 decimals).</summary>
        public static SettingsKey Float(string section, string name, Func<float> get, Action<float> set, float defaultValue, float min, float max, params string[] comment)
        {
            static string Text(float v) => v.ToString("0.###", CultureInfo.InvariantCulture);
            return new SettingsKey(section, name, comment, () => Text(get()), text =>
            {
                if (!float.TryParse(text, NumberStyles.Float, CultureInfo.InvariantCulture, out float v) ||
                    float.IsNaN(v) || v < min || v > max)
                    return false;
                set(v);
                return true;
            }, Text(defaultValue));
        }

        /// <summary>An enum key, written and read by the value's name (not its number).</summary>
        public static SettingsKey Enum<T>(string section, string name, Func<T> get, Action<T> set, T defaultValue, params string[] comment)
            where T : struct, System.Enum
        {
            return new SettingsKey(section, name, comment, () => get().ToString(), text =>
            {
                // Names only: Enum.TryParse would also take any number.
                string match = System.Enum.GetNames(typeof(T)).FirstOrDefault(n => string.Equals(n, text, StringComparison.OrdinalIgnoreCase));
                if (match == null)
                    return false;
                set(System.Enum.Parse<T>(match));
                return true;
            }, defaultValue.ToString());
        }

        /// <summary>A text key of at most <paramref name="maxLength"/> characters (may be empty).</summary>
        public static SettingsKey Text(string section, string name, Func<string> get, Action<string> set, string defaultValue, int maxLength, params string[] comment)
        {
            return new SettingsKey(section, name, comment, () => get() ?? string.Empty, text =>
            {
                if (text.Length > maxLength)
                    return false;
                set(text);
                return true;
            }, defaultValue ?? string.Empty);
        }

        /// <summary>
        /// A folder / file path key (may be empty): rejected when it has invalid path characters
        /// or cannot be resolved (environment variables expanded, relative to <paramref name="baseFolder"/>).
        /// </summary>
        public static SettingsKey Path(string section, string name, Func<string> get, Action<string> set, string defaultValue, Func<string> baseFolder,
            params string[] comment)
        {
            ArgumentNullException.ThrowIfNull(baseFolder);
            return new SettingsKey(section, name, comment, () => get() ?? string.Empty, text =>
            {
                if (text.Length > 0)
                {
                    if (text.IndexOfAny(System.IO.Path.GetInvalidPathChars()) >= 0)
                        return false;
                    try
                    {
                        System.IO.Path.GetFullPath(Environment.ExpandEnvironmentVariables(text), baseFolder());
                    }
                    catch (Exception ex) when (ex is ArgumentException || ex is NotSupportedException || ex is PathTooLongException)
                    {
                        return false;
                    }
                }
                set(text);
                return true;
            }, defaultValue ?? string.Empty);
        }

        #endregion
    }
}
