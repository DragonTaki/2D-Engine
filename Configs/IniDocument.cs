/* ----- ----- ----- ----- */
// IniDocument.cs
// Do not distribute or modify
// Author: DragonTaki (https://github.com/DragonTaki)
// Create Date: 2026/10/01
// Update Date: 2026/10/01
// Version: v1.0
/* ----- ----- ----- ----- */

using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Engine.Configs
{
    /// <summary>
    /// A plain-text <c>key = value</c> settings file in memory, kept line by line so it can
    /// be written back with the user's comments, blank lines and ordering intact. Generic
    /// (no game types, only <c>System.*</c>): what the keys mean is up to the caller.
    /// <para>
    /// Format, one item per line (surrounding whitespace ignored):
    /// blank; a comment starting with <c>#</c> or <c>;</c>; a section header
    /// <c>[name]</c>; or <c>key = value</c> (split at the first <c>=</c>; the value may be
    /// empty and may itself contain <c>=</c> or <c>#</c> - there are no inline comments).
    /// Entries before the first header belong to the unnamed section <c>""</c>. Section and
    /// key names compare case-insensitively; when a key repeats, the last one wins.
    /// Anything else (a line without <c>=</c>, an empty key, a broken header, NUL
    /// characters) makes the whole text malformed.
    /// </para>
    /// </summary>
    public sealed class IniDocument
    {
        private enum LineKind { Blank, Comment, Section, Entry }

        private sealed class Line
        {
            public LineKind Kind;
            public string Text;     // Written back as-is unless the entry's value changes.
            public string Section;  // Section the line is in (a header: its own name).
            public string Key;      // Entry only.
            public string Value;    // Entry only.
        }

        private static readonly StringComparer NameComparer = StringComparer.OrdinalIgnoreCase;

        private readonly List<Line> _lines = new();

        /// <summary>An empty document (no lines).</summary>
        public IniDocument() { }

        #region Parsing

        /// <summary>
        /// Parses <paramref name="text"/>. Returns false (with the first problem, by line
        /// number, in <paramref name="error"/>) when it is malformed.
        /// </summary>
        public static bool TryParse(string text, out IniDocument document, out string error)
        {
            document = null;
            error = null;
            if (text == null)
            {
                error = "no text";
                return false;
            }
            if (text.IndexOf('\0') >= 0)
            {
                error = "contains NUL characters (not a text file)";
                return false;
            }

            if (text.Length > 0 && text[0] == '﻿')
                text = text.Substring(1);

            var doc = new IniDocument();
            string section = string.Empty;
            string[] rawLines = text.Replace("\r\n", "\n").Replace('\r', '\n').Split('\n');
            // A trailing newline is not an extra blank line.
            int count = rawLines.Length;
            if (count > 0 && rawLines[count - 1].Length == 0)
                count--;

            for (int i = 0; i < count; i++)
            {
                string raw = rawLines[i];
                string trimmed = raw.Trim();
                var line = new Line { Text = raw, Section = section };

                if (trimmed.Length == 0)
                {
                    line.Kind = LineKind.Blank;
                }
                else if (trimmed[0] == '#' || trimmed[0] == ';')
                {
                    line.Kind = LineKind.Comment;
                }
                else if (trimmed[0] == '[')
                {
                    string name = trimmed.Length >= 2 && trimmed[trimmed.Length - 1] == ']'
                        ? trimmed.Substring(1, trimmed.Length - 2).Trim()
                        : null;
                    if (string.IsNullOrEmpty(name) || name.IndexOfAny(new[] { '[', ']' }) >= 0)
                    {
                        error = $"line {i + 1}: invalid section header \"{trimmed}\"";
                        return false;
                    }
                    line.Kind = LineKind.Section;
                    line.Section = section = name;
                }
                else
                {
                    int eq = trimmed.IndexOf('=');
                    string key = eq > 0 ? trimmed.Substring(0, eq).Trim() : null;
                    if (string.IsNullOrEmpty(key))
                    {
                        error = $"line {i + 1}: expected \"key = value\", got \"{trimmed}\"";
                        return false;
                    }
                    line.Kind = LineKind.Entry;
                    line.Key = key;
                    line.Value = trimmed.Substring(eq + 1).Trim();
                }

                doc._lines.Add(line);
            }

            document = doc;
            return true;
        }

        /// <summary>Parses <paramref name="text"/>; throws <see cref="FormatException"/> when it is malformed.</summary>
        public static IniDocument Parse(string text)
        {
            if (!TryParse(text, out var document, out string error))
                throw new FormatException(error);
            return document;
        }

        #endregion

        #region Reading

        /// <summary>Every entry in file order (a repeated key appears once per occurrence).</summary>
        public IEnumerable<(string Section, string Key, string Value)> Entries =>
            _lines.Where(l => l.Kind == LineKind.Entry).Select(l => (l.Section, l.Key, l.Value));

        /// <summary>The value of <paramref name="key"/> in <paramref name="section"/> (the last occurrence when repeated).</summary>
        public bool TryGetValue(string section, string key, out string value)
        {
            var line = FindEntry(section, key);
            value = line?.Value;
            return line != null;
        }

        /// <summary>Whether <paramref name="key"/> appears in <paramref name="section"/> more than once.</summary>
        public bool IsRepeated(string section, string key) =>
            _lines.Count(l => IsEntry(l, section, key)) > 1;

        #endregion

        #region Editing

        /// <summary>
        /// Sets the value of an existing entry (the last occurrence, the one that counts),
        /// rewriting only that line; adds the entry when it is missing.
        /// </summary>
        public void SetValue(string section, string key, string value)
        {
            ValidateName(key, nameof(key));
            ValidateValue(value);
            var line = FindEntry(section, key);
            if (line == null)
            {
                AddEntry(section, key, value);
                return;
            }
            line.Value = value;
            line.Text = EntryText(line.Key, value);
        }

        /// <summary>
        /// Adds <c>key = value</c> to the end of <paramref name="section"/> (after its last
        /// non-blank line), preceded by <paramref name="comments"/> as <c>#</c> lines. A
        /// missing section is appended to the end of the document, header first.
        /// </summary>
        public void AddEntry(string section, string key, string value, IEnumerable<string> comments = null)
        {
            section ??= string.Empty;
            if (section.Length > 0)
                ValidateName(section, nameof(section));
            ValidateName(key, nameof(key));
            ValidateValue(value);

            var block = new List<Line>();
            if (comments != null)
            {
                foreach (string c in comments)
                    block.Add(CommentLine(section, c));
            }
            block.Add(new Line { Kind = LineKind.Entry, Section = section, Key = key, Value = value, Text = EntryText(key, value) });

            int insertAt = FindSectionEnd(section);
            if (insertAt < 0)
            {
                // New section at the end, separated from what is above by a blank line.
                if (_lines.Count > 0 && _lines[^1].Kind != LineKind.Blank)
                    _lines.Add(new Line { Kind = LineKind.Blank, Text = string.Empty, Section = LastSection() });
                _lines.Add(new Line { Kind = LineKind.Section, Section = section, Text = $"[{section}]" });
                _lines.AddRange(block);
                return;
            }

            // Keep a commented entry visually separate from the entry above it.
            if (comments != null && insertAt > 0 && _lines[insertAt - 1].Kind == LineKind.Entry)
                block.Insert(0, new Line { Kind = LineKind.Blank, Text = string.Empty, Section = section });
            // ... and from a section header right below it.
            if (insertAt < _lines.Count && _lines[insertAt].Kind == LineKind.Section)
                block.Add(new Line { Kind = LineKind.Blank, Text = string.Empty, Section = section });
            _lines.InsertRange(insertAt, block);
        }

        /// <summary>Inserts a <c>#</c> comment line right above the entry that counts for <paramref name="key"/>; false when the key is missing.</summary>
        public bool InsertCommentBefore(string section, string key, string comment)
        {
            var line = FindEntry(section, key);
            if (line == null)
                return false;
            _lines.Insert(_lines.LastIndexOf(line), CommentLine(line.Section, comment));
            return true;
        }

        /// <summary>Appends a <c>#</c> comment line to the end of the document (blank text: a blank line).</summary>
        public void AppendComment(string comment)
        {
            if (string.IsNullOrEmpty(comment))
                _lines.Add(new Line { Kind = LineKind.Blank, Text = string.Empty, Section = LastSection() });
            else
                _lines.Add(CommentLine(LastSection(), comment));
        }

        #endregion

        #region Writing

        /// <summary>The document as text, one line per item, each ending with <see cref="Environment.NewLine"/>.</summary>
        public override string ToString()
        {
            var sb = new StringBuilder();
            foreach (var line in _lines)
                sb.Append(line.Text).Append(Environment.NewLine);
            return sb.ToString();
        }

        #endregion

        #region Helpers

        private static bool IsEntry(Line l, string section, string key) =>
            l.Kind == LineKind.Entry &&
            NameComparer.Equals(l.Section, section ?? string.Empty) &&
            NameComparer.Equals(l.Key, key);

        private Line FindEntry(string section, string key) =>
            _lines.LastOrDefault(l => IsEntry(l, section, key));

        private string LastSection() => _lines.Count == 0 ? string.Empty : _lines[^1].Section;

        /// <summary>
        /// Index just after the last non-blank line of the first block of
        /// <paramref name="section"/> (where a new entry goes), or -1 when the section has
        /// no header. The unnamed section always exists: it ends before the first header.
        /// </summary>
        private int FindSectionEnd(string section)
        {
            int start;
            if (section.Length == 0)
            {
                start = 0;
            }
            else
            {
                int header = _lines.FindIndex(l => l.Kind == LineKind.Section && NameComparer.Equals(l.Section, section));
                if (header < 0)
                    return -1;
                start = header + 1;
            }

            int end = start;
            for (int i = start; i < _lines.Count && _lines[i].Kind != LineKind.Section; i++)
            {
                if (_lines[i].Kind != LineKind.Blank)
                    end = i + 1;
            }
            return end;
        }

        private static string EntryText(string key, string value) =>
            value.Length == 0 ? key + " =" : $"{key} = {value}";

        private static Line CommentLine(string section, string comment) =>
            new Line { Kind = LineKind.Comment, Section = section, Text = "# " + (comment ?? string.Empty).Replace("\r", " ").Replace("\n", " ") };

        private static void ValidateName(string name, string paramName)
        {
            if (string.IsNullOrWhiteSpace(name) || name.IndexOfAny(new[] { '=', '[', ']', '\r', '\n' }) >= 0 || name.Trim() != name)
                throw new ArgumentException($"Invalid INI name \"{name}\"", paramName);
        }

        private static void ValidateValue(string value)
        {
            if (value == null || value.IndexOfAny(new[] { '\r', '\n' }) >= 0)
                throw new ArgumentException("An INI value must be a single line", nameof(value));
        }

        #endregion
    }
}
