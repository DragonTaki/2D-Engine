/* ----- ----- ----- ----- */
// TextPaste.cs
// Do not distribute or modify
// Author: DragonTaki (https://github.com/DragonTaki)
// Create Date: 2026/10/12
// Update Date: 2026/10/12
// Version: v1.0
/* ----- ----- ----- ----- */

using System;
using System.Text;

namespace Engine.UI.Utils
{
    /// <summary>
    /// How pasted text enters a one-line field (pure, no drawing; used by the text input
    /// handlers): control characters (line breaks, tabs ...) are removed, then every character the
    /// field does not accept, and what is left is inserted at the caret - only as much as still
    /// fits under the longest length (never splitting a surrogate pair); the caret ends after it.
    /// </summary>
    public static class TextPaste
    {
        /// <summary>
        /// <paramref name="pasted"/> (null as empty) without control characters and without the
        /// characters <paramref name="accepts"/> rejects (null: every other character is kept).
        /// </summary>
        public static string Clean(string pasted, Func<char, bool> accepts = null)
        {
            if (string.IsNullOrEmpty(pasted))
                return string.Empty;

            var sb = new StringBuilder(pasted.Length);
            foreach (char c in pasted)
            {
                if (char.IsControl(c) || (accepts != null && !accepts(c)))
                    continue;
                sb.Append(c);
            }
            return sb.ToString();
        }

        /// <summary>
        /// The text and caret after pasting <paramref name="pasted"/> into <paramref name="text"/>
        /// at <paramref name="caret"/> (cleaned by <see cref="Clean"/>; cut to the room left under
        /// <paramref name="maxLength"/>). A full field, or nothing left to paste, gives the text unchanged.
        /// </summary>
        /// <param name="text">The field's text (null as empty).</param>
        /// <param name="caret">The caret (clamped into the text).</param>
        /// <param name="pasted">The clipboard text, as read.</param>
        /// <param name="maxLength">The field's longest text.</param>
        /// <param name="accepts">The field's character filter (null: everything but control characters).</param>
        public static (string Text, int Caret) Insert(string text, int caret, string pasted, int maxLength, Func<char, bool> accepts = null)
        {
            text ??= string.Empty;
            caret = Math.Clamp(caret, 0, text.Length);

            string piece = Clean(pasted, accepts);
            int room = Math.Max(0, maxLength - text.Length);
            if (piece.Length > room)
            {
                int cut = room;
                // Keep a character whole: never end on the first half of a surrogate pair.
                if (cut > 0 && char.IsHighSurrogate(piece[cut - 1]))
                    cut--;
                piece = piece.Substring(0, cut);
            }
            if (piece.Length == 0)
                return (text, caret);

            return (text.Insert(caret, piece), caret + piece.Length);
        }
    }
}
