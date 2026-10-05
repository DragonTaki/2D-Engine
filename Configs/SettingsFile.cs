/* ----- ----- ----- ----- */
// SettingsFile.cs
// Do not distribute or modify
// Author: DragonTaki (https://github.com/DragonTaki)
// Create Date: 2026/10/05
// Update Date: 2026/10/05
// Version: v1.0
/* ----- ----- ----- ----- */

using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;

using Engine.Logging;

namespace Engine.Configs
{
    /// <summary>
    /// The single entry for the player's settings file (plain-text INI, <see cref="IniDocument"/>):
    /// it reads the file once and hands each registered area (<see cref="ISettingsArea"/>) its
    /// values, then lets every area put them into effect; saving writes every area's values back.
    /// It only knows sections, keys and values - what they mean is each area's business. The app's
    /// composition root registers the areas, in file order, before <see cref="Load"/>.
    /// <para>
    /// Load rule - the file wins over the code defaults:
    /// missing file: created from the defaults;
    /// malformed (unparsable) file: copied to <c>&lt;file&gt;.bak</c> (older backups rotate:
    /// .bak -> .bak2 -> .bak3 ..., so .bak is always the newest), then recreated from the defaults;
    /// a key missing or with an invalid value: that key uses its default, a warning is
    /// logged, and the file is written back with the key added / the value reset - every
    /// other line (the player's valid values, comments, unknown keys) is kept as it was.
    /// </para>
    /// </summary>
    public sealed class SettingsFile
    {
        private readonly List<ISettingsArea> _areas = new();
        private readonly string[] _header;

        /// <param name="path">The file.</param>
        /// <param name="header">Comment lines at the top of a freshly written file.</param>
        public SettingsFile(string path, IEnumerable<string> header)
        {
            ArgumentException.ThrowIfNullOrWhiteSpace(path);
            FilePath = path;
            _header = header?.ToArray() ?? Array.Empty<string>();
        }

        /// <summary>The file.</summary>
        public string FilePath { get; }

        /// <summary>The registered areas, in file order.</summary>
        public IReadOnlyList<ISettingsArea> Areas => _areas;

        /// <summary>Every key of every area, in file order.</summary>
        public IEnumerable<SettingsKey> Keys => _areas.SelectMany(area => area.Keys);

        /// <summary>Every key as (section, key), in file order.</summary>
        public IReadOnlyList<(string Section, string Key)> KeyNames => Keys.Select(k => (k.Section, k.Name)).ToList();

        /// <summary>Adds <paramref name="area"/> after the areas registered so far (its keys follow theirs in the file).</summary>
        public void Register(ISettingsArea area)
        {
            ArgumentNullException.ThrowIfNull(area);
            _areas.Add(area);
        }

        /// <summary>Lets every area put its current values into effect, in file order.</summary>
        public void ApplyAll()
        {
            foreach (var area in _areas)
                area.Apply();
        }

        #region Load / save

        /// <summary><see cref="Read"/>, then <see cref="ApplyAll"/>: the startup entry.</summary>
        public void Load(ICollection<string> warnings = null)
        {
            Read(warnings);
            ApplyAll();
        }

        /// <summary>
        /// Reads the file by the load rule (see the class summary) into the areas (which should
        /// hold their defaults), repairing or creating the file as needed; nothing is applied.
        /// Never throws for file problems: anything that cannot be read or written is logged and
        /// the defaults are used for it. Each warning is logged (WARN) and also added to
        /// <paramref name="warnings"/> if given.
        /// </summary>
        public void Read(ICollection<string> warnings = null)
        {
            void Warn(string message)
            {
                AppLogger.Log($"(Settings) {message}", LogLevel.WARN);
                warnings?.Add(message);
            }

            ReadInto(Warn);
        }

        private void ReadInto(Action<string> warn)
        {
            string path = FilePath;
            if (!File.Exists(path))
            {
                warn($"{path} not found: created it with the default settings");
                TryWrite(CreateDocument(), warn);
                return;
            }

            string text;
            try
            {
                text = File.ReadAllText(path);
            }
            catch (Exception ex) when (ex is IOException || ex is UnauthorizedAccessException)
            {
                warn($"cannot read {path} ({ex.Message}): using the default settings, file left as is");
                return;
            }

            if (!IniDocument.TryParse(text, out var doc, out string error))
            {
                string backup = TryBackup(warn);
                if (backup == null)
                {
                    warn($"{path} is malformed ({error}) and could not be backed up: using the default settings, file left as is");
                    return;
                }
                warn($"{path} is malformed ({error}): backed up to {backup}, recreated with the default settings");
                TryWrite(CreateDocument(), warn);
                return;
            }

            if (Dispatch(doc, warn))
                TryWrite(doc, warn);
        }

        /// <summary>
        /// Hands the parsed <paramref name="doc"/>'s values to the areas' keys: valid values are
        /// taken; a missing key is added to the document with its comment and default, an invalid
        /// value is reset to the default with a comment recording the old value. Returns whether
        /// the document changed (and so should be written back).
        /// </summary>
        private bool Dispatch(IniDocument doc, Action<string> warn)
        {
            var keys = Keys.ToList();
            bool changed = false;

            foreach (var key in keys)
            {
                if (!doc.TryGetValue(key.Section, key.Name, out string value))
                {
                    warn($"[{key.Section}] {key.Name} missing: using the default ({key.DefaultText}), added to the file");
                    doc.AddEntry(key.Section, key.Name, key.DefaultText, CommentFor(key));
                    changed = true;
                    continue;
                }

                if (doc.IsRepeated(key.Section, key.Name))
                    warn($"[{key.Section}] {key.Name} appears more than once: the last one ({value}) is used");

                if (!key.TryApply(value))
                {
                    string def = key.DefaultText;
                    warn($"[{key.Section}] {key.Name} = \"{value}\" is invalid: using the default ({def}), reset in the file");
                    doc.InsertCommentBefore(key.Section, key.Name, $"原本的值「{value}」無效，已改回預設值 {def}。");
                    doc.SetValue(key.Section, key.Name, def);
                    changed = true;
                }
            }

            foreach (var (section, name, _) in doc.Entries)
            {
                if (!keys.Any(k => string.Equals(k.Section, section, StringComparison.OrdinalIgnoreCase) &&
                                   string.Equals(k.Name, name, StringComparison.OrdinalIgnoreCase)))
                    warn($"unknown key [{section}] {name}: ignored (kept in the file)");
            }

            return changed;
        }

        /// <summary>
        /// Writes every area's current values to the file. A missing file is created fresh. A
        /// parsable file keeps every line it has (comments, layout, unknown keys) with only the
        /// values updated and any missing key added to its section. A malformed file is first
        /// backed up (<c>.bak</c> rotation, as in <see cref="Load"/>) and then replaced by a fresh
        /// one. The write goes through a temp file, so a crash never leaves half a file. Returns
        /// false (logged) when the file cannot be read, backed up or written; the existing file is
        /// then left as it was.
        /// </summary>
        public bool Save()
        {
            void Warn(string message) => AppLogger.Log($"(Settings) {message}", LogLevel.WARN);

            string path = FilePath;
            IniDocument doc = null;
            if (File.Exists(path))
            {
                string text;
                try
                {
                    text = File.ReadAllText(path);
                }
                catch (Exception ex) when (ex is IOException || ex is UnauthorizedAccessException)
                {
                    Warn($"cannot read {path} ({ex.Message}): not saved, file left as is");
                    return false;
                }

                if (IniDocument.TryParse(text, out var existing, out string error))
                {
                    doc = existing;
                }
                else
                {
                    string backup = TryBackup(Warn);
                    if (backup == null)
                    {
                        Warn($"{path} is malformed ({error}) and could not be backed up: not saved, file left as is");
                        return false;
                    }
                    Warn($"{path} is malformed ({error}): backed up to {backup}, replaced with the saved settings");
                }
            }

            if (doc == null)
            {
                doc = CreateDocument();
            }
            else
            {
                foreach (var key in Keys)
                {
                    if (doc.TryGetValue(key.Section, key.Name, out _))
                        doc.SetValue(key.Section, key.Name, key.Format());
                    else
                        doc.AddEntry(key.Section, key.Name, key.Format(), CommentFor(key));
                }
            }

            return TryWrite(doc, Warn);
        }

        /// <summary>The full text of a fresh file holding the areas' current values, every key commented.</summary>
        public string Generate() => CreateDocument().ToString();

        private IniDocument CreateDocument()
        {
            var doc = new IniDocument();
            foreach (string line in _header)
                doc.AppendComment(line);
            foreach (var key in Keys)
                doc.AddEntry(key.Section, key.Name, key.Format(), CommentFor(key));
            return doc;
        }

        private static IEnumerable<string> CommentFor(SettingsKey key)
        {
            string def = key.DefaultText;
            return key.Comment.Append($"預設值：{(def.Length == 0 ? "（空白）" : def)}");
        }

        private bool TryWrite(IniDocument doc, Action<string> warn)
        {
            string path = FilePath;
            string temp = path + ".tmp";
            try
            {
                string dir = Path.GetDirectoryName(Path.GetFullPath(path));
                if (!string.IsNullOrEmpty(dir))
                    Directory.CreateDirectory(dir);
                // Written next to the target, then moved over it: a crash mid-write
                // never leaves a half-written settings file.
                File.WriteAllText(temp, doc.ToString());
                File.Move(temp, path, overwrite: true);
                return true;
            }
            catch (Exception ex) when (ex is IOException || ex is UnauthorizedAccessException || ex is NotSupportedException)
            {
                warn($"cannot write {path}: {ex.Message}");
                try { if (File.Exists(temp)) File.Delete(temp); } catch (Exception) { }
                return false;
            }
        }

        /// <summary>
        /// Copies the file to <c>&lt;file&gt;.bak</c>, which is always the newest backup: existing
        /// ones rotate first (logrotate style) - .bakN -> .bak(N+1) from the highest down, then
        /// .bak -> .bak2 - so no backup is ever lost or overwritten. Returns the backup path, or
        /// null when rotating or copying failed.
        /// </summary>
        private string TryBackup(Action<string> warn)
        {
            string path = FilePath;
            string backup = path + ".bak";
            try
            {
                string Numbered(int n) => n == 1 ? backup : backup + n;

                int highest = 0;
                while (File.Exists(Numbered(highest + 1)))
                    highest++;

                for (int n = highest; n >= 1; n--)
                    File.Move(Numbered(n), Numbered(n + 1));

                File.Copy(path, backup, overwrite: false);
                return backup;
            }
            catch (Exception ex) when (ex is IOException || ex is UnauthorizedAccessException)
            {
                warn($"cannot back up {path} to {backup}: {ex.Message}");
                return null;
            }
        }

        #endregion
    }
}
